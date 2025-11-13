using AdminBackend.Domain.Entities;
using AutoMapper;
using FluentValidation;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;

namespace AdminBackend.Application.Features.LlmManagement.LlmProviderHandlers
{
    public record UpdateLlmProviderCommand(long Id, string Name) : ICommand<UpdateLlmProviderCommandResult>;
    public record UpdateLlmProviderCommandResult(long Id, string Name);

    public class UpdateLlmProviderCommandValidator : AbstractValidator<UpdateLlmProviderCommand>
    {
        public UpdateLlmProviderCommandValidator()
        {
            //TODO : Unique check
            RuleFor(x => x.Name).NotEmpty().MaximumLength(50);
        }
    }
    internal class UpdateLlmProviderCommandHandler(IRepository<LlmProvider> repository, IMapper mapper)
        : ICommandHandler<UpdateLlmProviderCommand, UpdateLlmProviderCommandResult>
    {
        private readonly IRepository<LlmProvider> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<UpdateLlmProviderCommandResult> Handle(UpdateLlmProviderCommand request, CancellationToken cancellationToken)
        {
            var entitiy = mapper.Map<LlmProvider>(request);
            repository.Update(entitiy);
            await repository.SaveChangesAsync(cancellationToken);
            var result = mapper.Map<UpdateLlmProviderCommandResult>(entitiy);
            return result;
        }
    }
}
