using AdminBackend.Domain.Entities;
using AutoMapper;
using FluentValidation;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;

namespace AdminBackend.Application.Features.SearchEngineManagement.SearchEngineHandlers
{
    public record UpdateSearchEngineCommand(long Id, long SearchEngineTypeId, string Identifier, string Url) : ICommand<UpdateSearchEngineCommandResult>;
    public record UpdateSearchEngineCommandResult(long Id, long SearchEngineTypeId, string Identifier, string Url);

    public class UpdateSearchEngineCommandValidator : AbstractValidator<UpdateSearchEngineCommand>
    {
        public UpdateSearchEngineCommandValidator()
        {
            //TODO : Unique check
            RuleFor(x => x.Identifier).NotEmpty().MaximumLength(50);
            RuleFor(x => x.Url).NotEmpty().MaximumLength(255);
        }
    }
    internal class UpdateSearchEngineCommandHandler(IRepository<SearchEngine> repository, IMapper mapper)
        : ICommandHandler<UpdateSearchEngineCommand, UpdateSearchEngineCommandResult>
    {
        private readonly IRepository<SearchEngine> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<UpdateSearchEngineCommandResult> Handle(UpdateSearchEngineCommand request, CancellationToken cancellationToken)
        {
            var entitiy = mapper.Map<SearchEngine>(request);
            repository.Update(entitiy);
            await repository.SaveChangesAsync(cancellationToken);
            var result = mapper.Map<UpdateSearchEngineCommandResult>(entitiy);
            return result;
        }
    }
}
