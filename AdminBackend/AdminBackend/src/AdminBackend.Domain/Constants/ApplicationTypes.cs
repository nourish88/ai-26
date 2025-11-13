namespace AdminBackend.Domain.Constants
{
    public enum ApplicationTypes : long
    {
        REACT = 1,//mcp server
        CHATBOT = 2,//No Search, no mcp server
        AGENTIC_RAG = 3,//search
        REFLECTIVE_RAG = 4,
        MCP_POWERED_AGENTIC_RAG= 5,
        CUSTOM = 99
    }
}
