using AdminBackend.Domain.Entities;
using AutoMapper;
using FluentValidation;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;

namespace AdminBackend.Application.Features.SearchEngineManagement.SearchEngineHandlers
{

    public record CreateSearchEngineCommand(string Identifier, string Url) :ICommand<CreateSearchEngineCommandResult>;
    public record CreateSearchEngineCommandResult(long Id,string Identifier, string Url);

    public class CreateSearchEngineCommandValidator : AbstractValidator<CreateSearchEngineCommand>
    {
        public CreateSearchEngineCommandValidator()
        {
            //TODO : Unique check
            RuleFor(x=>x.Identifier).NotEmpty().MaximumLength(50);
            RuleFor(x=>x.Url).NotEmpty().MaximumLength(255);
        }
    }

    internal class CreateSearchEngineCommandHandler(IRepository<SearchEngine> repository, IMapper mapper) 
        : ICommandHandler<CreateSearchEngineCommand, CreateSearchEngineCommandResult>
    {
        private readonly IRepository<SearchEngine> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<CreateSearchEngineCommandResult> Handle(CreateSearchEngineCommand request, CancellationToken cancellationToken)
        {
            var entitiy = mapper.Map<SearchEngine>(request);
            entitiy = await repository.InsertAsync(entitiy,cancellationToken);
            await repository.SaveChangesAsync(cancellationToken);
            var result = mapper.Map<CreateSearchEngineCommandResult>(entitiy);
            return result;
        }
    }
}
