using AdminBackend.Domain.Entities;
using AutoMapper;
using FluentValidation;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;

namespace AdminBackend.Application.Features.SearchEngineManagement.SearchEngineTypeHandlers
{
    public record UpdateSearchEngineTypeCommand(long Id, string Identifier) : ICommand<UpdateSearchEngineTypeCommandResult>;
    public record UpdateSearchEngineTypeCommandResult(long Id, string Identifier);

    public class UpdateSearchEngineTypeCommandValidator : AbstractValidator<UpdateSearchEngineTypeCommand>
    {
        public UpdateSearchEngineTypeCommandValidator()
        {
            //TODO : Unique check
            RuleFor(x => x.Identifier).NotEmpty().MaximumLength(50);
        }
    }
    internal class UpdateSearchEngineTypeCommandHandler(IRepository<SearchEngineType> repository, IMapper mapper)
        : ICommandHandler<UpdateSearchEngineTypeCommand, UpdateSearchEngineTypeCommandResult>
    {
        private readonly IRepository<SearchEngineType> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<UpdateSearchEngineTypeCommandResult> Handle(UpdateSearchEngineTypeCommand request, CancellationToken cancellationToken)
        {
            var entitiy = mapper.Map<SearchEngineType>(request);
            repository.Update(entitiy);
            await repository.SaveChangesAsync(cancellationToken);
            var result = mapper.Map<UpdateSearchEngineTypeCommandResult>(entitiy);
            return result;
        }
    }
}
