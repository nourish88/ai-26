from log import create_logger
from openevals.llm import create_async_llm_as_judge
from openevals.prompts import HALLUCINATION_PROMPT
from .states import OrchestratorState
from langgraph.prebuilt import InjectedState
from typing import Annotated
from langfuse import observe
from langchain_core.messages import AIMessage
from .models import create_model
from .models import LlmModel
from .prompts import OWASP_LLM_TOP_10_GUARDRAIL_PROMPT
from integrations.admin_backend import AdminBackendService,ChatDetectionType
from langchain_core.prompts import ChatPromptTemplate
from langchain_core.output_parsers import StrOutputParser

@observe(name="HallucinationDetector",as_type="evaluator")
async def detect_hallucination_hook(state: Annotated[OrchestratorState, InjectedState]):
    logger = create_logger(__name__)
    llm_model  = state.get("llm")
    if not isinstance(llm_model, LlmModel):
        return {}
    
    last_msg = state.get("messages")[-1]
    message_id = state.get("message_id")
    thread_id = state.get("thread_id")
    user_id = state.get("user_id")
    if not isinstance(last_msg, AIMessage) or last_msg.tool_calls:
        return {}
    
    if not message_id:
        logger.warning("Message id not found")
        return {}
    
    if not thread_id:
        logger.warning("Thread id not found")
        return {}

    inputs = state.get("retrieval_query") or ""
    outputs = last_msg.content
    context = "\n".join(state.get("retrieved_docs") or [])

    if not inputs or not context:
        return {}
    
    model = create_model(llmModel=llm_model)

    hallucination_evaluator = create_async_llm_as_judge(
        prompt=HALLUCINATION_PROMPT,
        feedback_key="hallucination",
        judge=model
    )   

    try:
        eval_result = await hallucination_evaluator(
            inputs=inputs,
            outputs=outputs,
            context=context,
            reference_outputs=""
        )
        score = eval_result.get("score",False)
        comment = eval_result.get("comment", "")
    except Exception as e:
        logger.error(f"Failed to detect hallucination")
        logger.exception(e)
        return {}

    if not score:
        admin_service = AdminBackendService()
        appsResponse = await admin_service.chat_detection(
            chat_detection_type=ChatDetectionType.Hallucination
            ,message_id=message_id
            ,reason=comment
            ,sources=context
            ,thread_id=thread_id
            ,user_id=user_id
            ,user_message=inputs
            )
        return {
            "messages": [
                AIMessage(f"Ürettiğim cevabın doğruluk oranı düşük. Dikkatli olunuz.")
            ],
            "goto": "END"
        }
    return {}


async def check_message_guard_rails(llm_model : LlmModel,message : str)->bool:
    logger = create_logger(__name__)
    if not message:
        logger.warning("Message not found")
        return True
    model = create_model(llmModel=llm_model)
    guardrail_prompt_template = ChatPromptTemplate.from_template(OWASP_LLM_TOP_10_GUARDRAIL_PROMPT)
    guardrail_chain = guardrail_prompt_template | model | StrOutputParser()
    
    verdict = guardrail_chain.invoke({"user_prompt": message})
    if "UNSAFE" in verdict:
        return False
    return True    