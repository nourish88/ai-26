using AdminBackend.Domain.Entities;
using AutoMapper;
using FluentValidation;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;

namespace AdminBackend.Application.Features.LlmManagement.LlmHandlers
{
    public record UpdateLlmCommand(long Id,
       long LlmProviderId,
       int MaxInputTokenSize,
       int MaxOutputTokenSize,
       string Url,
       string ModelName) : ICommand<UpdateLlmCommandResult>;
    public record UpdateLlmCommandResult(long Id,
       long LlmProviderId,
       int MaxInputTokenSize,
       int MaxOutputTokenSize,
       string Url,
       string ModelName) ;

    public class UpdateLlmCommandValidator : AbstractValidator<UpdateLlmCommand>
    {
        public UpdateLlmCommandValidator()
        {
            RuleFor(x => x.LlmProviderId).GreaterThan(0);
            RuleFor(x => x.Url).NotNull().NotEmpty().MaximumLength(255);
            RuleFor(x => x.ModelName).NotNull().NotEmpty().MaximumLength(50);
            RuleFor(x => x.MaxInputTokenSize).GreaterThan(0);
            RuleFor(x => x.MaxOutputTokenSize).GreaterThan(0);
        }
    }
    internal class UpdateLlmCommandHandler(IRepository<Llm> repository, IMapper mapper)
        : ICommandHandler<UpdateLlmCommand, UpdateLlmCommandResult>
    {
        private readonly IRepository<Llm> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<UpdateLlmCommandResult> Handle(UpdateLlmCommand request, CancellationToken cancellationToken)
        {
            var entitiy = mapper.Map<Llm>(request);
            repository.Update(entitiy);
            await repository.SaveChangesAsync(cancellationToken);
            var result = mapper.Map<UpdateLlmCommandResult>(entitiy);
            return result;
        }
    }
}
