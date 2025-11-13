using AdminBackend.Domain.Entities;
using AutoMapper;
using FluentValidation;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;

namespace AdminBackend.Application.Features.ExtractionManagement.ApplicationExtractorEngineHandlers
{
    public record UpdateApplicationExtractorEngineCommand(
        long Id,
        long ApplicationId,
        long ExtractorEngineTypeId) : ICommand<UpdateApplicationExtractorEngineCommandResult>;
    public record UpdateApplicationExtractorEngineCommandResult(
        long Id,
        long ApplicationId,
        long ExtractorEngineTypeId) ;

    public class UpdateApplicationExtractorEngineCommandValidator : AbstractValidator<UpdateApplicationExtractorEngineCommand>
    {
        public UpdateApplicationExtractorEngineCommandValidator()
        {
            RuleFor(x => x.ApplicationId).GreaterThan(0);
            RuleFor(x => x.ExtractorEngineTypeId).GreaterThan(0);
        }
    }
    internal class UpdateApplicationExtractorEngineCommandHandler(IRepository<ApplicationExtractorEngine> repository, IMapper mapper)
        : ICommandHandler<UpdateApplicationExtractorEngineCommand, UpdateApplicationExtractorEngineCommandResult>
    {
        private readonly IRepository<ApplicationExtractorEngine> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<UpdateApplicationExtractorEngineCommandResult> Handle(UpdateApplicationExtractorEngineCommand request, CancellationToken cancellationToken)
        {
            var entitiy = mapper.Map<ApplicationExtractorEngine>(request);
            repository.Update(entitiy);
            await repository.SaveChangesAsync(cancellationToken);
            var result = mapper.Map<UpdateApplicationExtractorEngineCommandResult>(entitiy);
            return result;
        }
    }
}
