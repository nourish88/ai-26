using AdminBackend.Domain.Entities;
using AutoMapper;
using FluentValidation;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;

namespace AdminBackend.Application.Features.SearchEngineManagement.SearchEngineTypeHandlers
{

    public record CreateSearchEngineTypeCommand(string Identifier) :ICommand<CreateSearchEngineTypeCommandResult>;
    public record CreateSearchEngineTypeCommandResult(long Id,string Identifier);

    public class CreateSearchEngineTypeCommandValidator : AbstractValidator<CreateSearchEngineTypeCommand>
    {
        public CreateSearchEngineTypeCommandValidator()
        {
            //TODO : Unique check
            RuleFor(x=>x.Identifier).NotEmpty().MaximumLength(50);
        }
    }

    internal class CreateSearchEngineTypeCommandHandler(IRepository<SearchEngineType> repository, IMapper mapper) 
        : ICommandHandler<CreateSearchEngineTypeCommand, CreateSearchEngineTypeCommandResult>
    {
        private readonly IRepository<SearchEngineType> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<CreateSearchEngineTypeCommandResult> Handle(CreateSearchEngineTypeCommand request, CancellationToken cancellationToken)
        {
            var entitiy = mapper.Map<SearchEngineType>(request);
            entitiy = await repository.InsertAsync(entitiy,cancellationToken);
            await repository.SaveChangesAsync(cancellationToken);
            var result = mapper.Map<CreateSearchEngineTypeCommandResult>(entitiy);
            return result;
        }
    }
}
