from fastapi import FastAPI, BackgroundTasks, status, Depends
from fastapi.middleware.cors import CORSMiddleware

from src.configuration.configuration_manager import ConfigurationManager
from src.jobs.extract_chunk_job import ExtractChunkJob
from src.models.job_request import JobRequest
from contextlib import asynccontextmanager
from src.cache import init_cache_helper,close_cache_helper
from src.request import RequestContextMiddleware,CustomExceptionMiddleware, RequestContext, get_request_context
from src.server import setup_custom_openapi


@asynccontextmanager
async def lifespan(app: FastAPI):
    # Startup
    await init_cache_helper()
    yield
    # Shutdown
    await close_cache_helper()

app = FastAPI(
    title="DataManager Server",
    version="1.0",
    description="Api server using extraction and chunking",
    docs_url="/docs",
    lifespan=lifespan,
    swagger_ui_init_oauth={
        "clientId": ConfigurationManager.get_auth_audience(),
        "clientSecret": ConfigurationManager.get_auth_client_secret(),
        "usePkceWithAuthorizationCodeGrant": True,  # <-- PKCE aktif
    }
)

setup_custom_openapi(app=app)

app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
    expose_headers=["*"],
)
app.add_middleware(CustomExceptionMiddleware)
app.add_middleware(RequestContextMiddleware)

job = ExtractChunkJob()


@app.get("/", status_code=status.HTTP_200_OK)
async def check(context: RequestContext = Depends(get_request_context)):
    return "Up and running"


@app.post("/job", status_code=status.HTTP_202_ACCEPTED)
async def send_notification(model: JobRequest, background_tasks: BackgroundTasks,context: RequestContext = Depends(get_request_context)):
    background_tasks.add_task(job.execute, model)
    return {"message": "Job is accepted"}


if __name__ == "__main__":
    import uvicorn

    uvicorn.run(app, host=ConfigurationManager.get_app_host(), port=ConfigurationManager.get_app_port())
