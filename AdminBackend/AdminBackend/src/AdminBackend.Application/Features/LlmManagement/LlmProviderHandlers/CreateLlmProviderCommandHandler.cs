using AdminBackend.Domain.Entities;
using AutoMapper;
using FluentValidation;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;

namespace AdminBackend.Application.Features.LlmManagement.LlmProviderHandlers
{

    public record CreateLlmProviderCommand(string Name):ICommand<CreateLlmProviderCommandResult>;
    public record CreateLlmProviderCommandResult(long Id,string Name);

    public class CreateLlmProviderCommandValidator : AbstractValidator<CreateLlmProviderCommand>
    {
        public CreateLlmProviderCommandValidator()
        {
            //TODO : Unique check
            RuleFor(x=>x.Name).NotEmpty().MaximumLength(50);
        }
    }

    internal class CreateLlmProviderCommandHandler(IRepository<LlmProvider> repository, IMapper mapper) 
        : ICommandHandler<CreateLlmProviderCommand, CreateLlmProviderCommandResult>
    {
        private readonly IRepository<LlmProvider> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<CreateLlmProviderCommandResult> Handle(CreateLlmProviderCommand request, CancellationToken cancellationToken)
        {
            var entitiy = mapper.Map<LlmProvider>(request);
            entitiy = await repository.InsertAsync(entitiy,cancellationToken);
            await repository.SaveChangesAsync(cancellationToken);
            var result = mapper.Map<CreateLlmProviderCommandResult>(entitiy);
            return result;
        }
    }
}
