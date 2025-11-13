using AdminBackend.Domain.Entities;
using AutoMapper;
using FluentValidation;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;

namespace AdminBackend.Application.Features.ApplicationManagement.ApplicationLlmHandlers
{

    public record CreateApplicationLlmCommand(
        float TopP,
        float Temperature,
        bool EnableThinking,
        long LlmId,
        long ApplicationId) : ICommand<CreateApplicationLlmCommandResult>;
    public record CreateApplicationLlmCommandResult(
        long Id,
        float TopP,
        float Temperature,
        bool EnableThinking,
        long LlmId,
        long ApplicationId);

    public class CreateApplicationLlmCommandValidator : AbstractValidator<CreateApplicationLlmCommand>
    {
        public CreateApplicationLlmCommandValidator()
        {
            RuleFor(x => x.TopP).GreaterThanOrEqualTo(0);
            RuleFor(x => x.TopP).LessThanOrEqualTo(1);
            RuleFor(x => x.Temperature).GreaterThanOrEqualTo(0);
            RuleFor(x => x.Temperature).LessThanOrEqualTo(1);
        }
    }

    internal class CreateApplicationLlmCommandHandler(IRepository<ApplicationLlm> repository, IMapper mapper) 
        : ICommandHandler<CreateApplicationLlmCommand, CreateApplicationLlmCommandResult>
    {
        private readonly IRepository<ApplicationLlm> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<CreateApplicationLlmCommandResult> Handle(CreateApplicationLlmCommand request, CancellationToken cancellationToken)
        {
            var entitiy = mapper.Map<ApplicationLlm>(request);
            entitiy = await repository.InsertAsync(entitiy,cancellationToken);
            await repository.SaveChangesAsync(cancellationToken);
            var result = mapper.Map<CreateApplicationLlmCommandResult>(entitiy);
            return result;
        }
    }
}
