using AutoMapper;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace AdminBackend.Application.Features.ApplicationManagement.AgentHandlers
{
    public record GetAgentConfigurationQuery(string applicationIdentifier) : IQuery<GetAgentConfigurationQueryResult>;
    public record Model(
        string id,
        string name,
        float topP,
        float temperature,
        int maxTokens,
        bool enableThinking,
        string url
        );
    public record McpServer(string id, string identifier, string uri);
    public record Agent(
        string id,
        string name,
        string indentifier,
        string description,
        string type,
        string outputType,
        string prompt,
        string memoryType,
        bool enableGuardRails,
        bool checkHallucination,
    List<McpServer>? mcpservers,
        Model model
        );
    public record GetAgentConfigurationQueryResult(
           bool isSuccess,
           string? error,
           Agent? agent
        );
    internal class GetAgentConfigurationQueryHandler(
        ILogger<GetAgentConfigurationQueryHandler> logger
        , IRepository<Domain.Entities.Application> applicationRepository
        , IMapper mapper)
        : IQueryHandler<GetAgentConfigurationQuery, GetAgentConfigurationQueryResult>
    {
        private readonly ILogger<GetAgentConfigurationQueryHandler> logger = logger;
        private readonly IRepository<Domain.Entities.Application> applicationRepository = applicationRepository;

        public async Task<GetAgentConfigurationQueryResult> Handle(GetAgentConfigurationQuery request, CancellationToken cancellationToken)
        {
            var applicationWithLlm = await applicationRepository.GetFirstOrDefaultAsync(
            predicate: p => p.Identifier == request.applicationIdentifier,
            include: r => r.Include(i => i.ApplicationType)
                           .Include(i => i.MemoryType)
                           .Include(i => i.OutputType)
                           .Include(i => i.ApplicationMcpServers).ThenInclude(t => t.McpServer)
                           .Include(i => i.ApplicationLlm).ThenInclude(t => t.Llm),
            cancellationToken: cancellationToken);

            bool isSuccess = false;
            string? error = null;
            Agent? agent = null;
            if (applicationWithLlm == null)
            {
                error = $"Application not found for application {request.applicationIdentifier}";
                logger.LogWarning(error);
            }
            else
            {
                if (applicationWithLlm.ApplicationLlm == null)
                {
                    error = $"ApplicationLlm not found for application {request.applicationIdentifier}";
                    logger.LogWarning(error);
                }
                else
                {
                    if (applicationWithLlm.ApplicationLlm.Llm == null)
                    {
                        error = $"Llm not found for application {request.applicationIdentifier}";
                        logger.LogWarning(error);
                    }
                    else
                    {
                        List<McpServer> mcpServers = new List<McpServer>();
                        foreach (var applicationMcpServer in applicationWithLlm.ApplicationMcpServers)
                        {
                            mcpServers.Add(new McpServer(
                                applicationMcpServer.McpServer.Id.ToString()
                                , applicationMcpServer.McpServer.Identifier
                                , applicationMcpServer.McpServer.Uri));
                        }

                        var model = new Model(
                            applicationWithLlm.ApplicationLlm.Llm.Id.ToString()
                            , applicationWithLlm.ApplicationLlm.Llm.ModelName
                            , applicationWithLlm.ApplicationLlm.TopP
                            , applicationWithLlm.ApplicationLlm.Temperature
                            , applicationWithLlm.ApplicationLlm.Llm.MaxInputTokenSize
                            , applicationWithLlm.ApplicationLlm.EnableThinking
                            , applicationWithLlm.ApplicationLlm.Llm.Url
                            );
                        agent = new Agent(
                            applicationWithLlm.Id.ToString()
                            , applicationWithLlm.Name
                            , applicationWithLlm.Identifier
                            , applicationWithLlm.Description
                            , applicationWithLlm.ApplicationType.Identifier
                            , applicationWithLlm.OutputType.Identifier
                            , applicationWithLlm.SystemPrompt
                            , applicationWithLlm.MemoryType.Identifier
                            , applicationWithLlm.EnableGuardRails
                            , applicationWithLlm.CheckHallucination
                            , mcpServers
                            , model
                            );

                        isSuccess = true;
                    }
                }
            }
            return new GetAgentConfigurationQueryResult(isSuccess, error, agent);

        }
    }
}
