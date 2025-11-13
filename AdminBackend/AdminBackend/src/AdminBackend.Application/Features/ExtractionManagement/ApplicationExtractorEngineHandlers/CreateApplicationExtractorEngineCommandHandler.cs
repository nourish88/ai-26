using AdminBackend.Domain.Entities;
using AutoMapper;
using FluentValidation;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;

namespace AdminBackend.Application.Features.ExtractionManagement.ApplicationExtractorEngineHandlers
{

    public record CreateApplicationExtractorEngineCommand(
        long ApplicationId,
        long ExtractorEngineTypeId) : ICommand<CreateApplicationExtractorEngineCommandResult>;
    public record CreateApplicationExtractorEngineCommandResult(
        long Id,
        long ApplicationId,
        long ExtractorEngineTypeId);

    public class CreateApplicationExtractorEngineCommandValidator : AbstractValidator<CreateApplicationExtractorEngineCommand>
    {
        public CreateApplicationExtractorEngineCommandValidator()
        {
            RuleFor(x => x.ApplicationId).GreaterThan(0);
            RuleFor(x => x.ExtractorEngineTypeId).GreaterThan(0);
        }
    }

    internal class CreateApplicationExtractorEngineCommandHandler(IRepository<ApplicationExtractorEngine> repository, IMapper mapper) 
        : ICommandHandler<CreateApplicationExtractorEngineCommand, CreateApplicationExtractorEngineCommandResult>
    {
        private readonly IRepository<ApplicationExtractorEngine> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<CreateApplicationExtractorEngineCommandResult> Handle(CreateApplicationExtractorEngineCommand request, CancellationToken cancellationToken)
        {
            var entitiy = mapper.Map<ApplicationExtractorEngine>(request);
            entitiy = await repository.InsertAsync(entitiy,cancellationToken);
            await repository.SaveChangesAsync(cancellationToken);
            var result = mapper.Map<CreateApplicationExtractorEngineCommandResult>(entitiy);
            return result;
        }
    }
}
