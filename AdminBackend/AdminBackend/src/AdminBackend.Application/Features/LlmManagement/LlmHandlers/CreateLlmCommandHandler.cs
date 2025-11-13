using AdminBackend.Domain.Entities;
using AutoMapper;
using FluentValidation;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;

namespace AdminBackend.Application.Features.LlmManagement.LlmHandlers
{

    public record CreateLlmCommand(
       long LlmProviderId,
       int MaxInputTokenSize,
       int MaxOutputTokenSize,
       string Url,
       string ModelName) : ICommand<CreateLlmCommandResult>;
    public record CreateLlmCommandResult(long Id,
       long LlmProviderId,
       int MaxInputTokenSize,
       int MaxOutputTokenSize,
       string Url,
       string ModelName);

    public class CreateLlmCommandValidator : AbstractValidator<CreateLlmCommand>
    {
        public CreateLlmCommandValidator()
        {
            RuleFor(x=>x.LlmProviderId).GreaterThan(0);
            RuleFor(x=>x.Url).NotNull().NotEmpty().MaximumLength(255);
            RuleFor(x=>x.ModelName).NotNull().NotEmpty().MaximumLength(50);
            RuleFor(x => x.MaxInputTokenSize).GreaterThan(0);
            RuleFor(x => x.MaxOutputTokenSize).GreaterThan(0);


        }
    }

    internal class CreateLlmCommandHandler(IRepository<Llm> repository, IMapper mapper) 
        : ICommandHandler<CreateLlmCommand, CreateLlmCommandResult>
    {
        private readonly IRepository<Llm> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<CreateLlmCommandResult> Handle(CreateLlmCommand request, CancellationToken cancellationToken)
        {
            var entitiy = mapper.Map<Llm>(request);
            entitiy = await repository.InsertAsync(entitiy,cancellationToken);
            await repository.SaveChangesAsync(cancellationToken);
            var result = mapper.Map<CreateLlmCommandResult>(entitiy);
            return result;
        }
    }
}
