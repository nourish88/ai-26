using AdminBackend.Domain.Entities;
using AutoMapper;
using FluentValidation;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;

namespace AdminBackend.Application.Features.ApplicationManagement.ApplicationLlmHandlers
{
    public record UpdateApplicationLlmCommand(
        long Id,
        float TopP,
        float Temperature,
        bool EnableThinking,
        long LlmId,
        long ApplicationId) : ICommand<UpdateApplicationLlmCommandResult>;
    public record UpdateApplicationLlmCommandResult(
        long Id,
        float TopP,
        float Temperature,
        bool EnableThinking,
        long LlmId,
        long ApplicationId) ;

    public class UpdateApplicationLlmCommandValidator : AbstractValidator<UpdateApplicationLlmCommand>
    {
        public UpdateApplicationLlmCommandValidator()
        {
            //TODO : Unique check
            RuleFor(x => x.TopP).GreaterThanOrEqualTo(0);
            RuleFor(x => x.TopP).LessThanOrEqualTo(1);
            RuleFor(x => x.Temperature).GreaterThanOrEqualTo(0);
            RuleFor(x => x.Temperature).LessThanOrEqualTo(1);
        }
    }
    internal class UpdateApplicationLlmCommandHandler(IRepository<ApplicationLlm> repository, IMapper mapper)
        : ICommandHandler<UpdateApplicationLlmCommand, UpdateApplicationLlmCommandResult>
    {
        private readonly IRepository<ApplicationLlm> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<UpdateApplicationLlmCommandResult> Handle(UpdateApplicationLlmCommand request, CancellationToken cancellationToken)
        {
            var entitiy = mapper.Map<ApplicationLlm>(request);
            repository.Update(entitiy);
            await repository.SaveChangesAsync(cancellationToken);
            var result = mapper.Map<UpdateApplicationLlmCommandResult>(entitiy);
            return result;
        }
    }
}
