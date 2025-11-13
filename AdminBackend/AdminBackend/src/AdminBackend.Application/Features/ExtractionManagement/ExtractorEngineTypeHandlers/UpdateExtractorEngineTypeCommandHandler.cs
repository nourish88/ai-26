using AdminBackend.Domain.Entities;
using AutoMapper;
using FluentValidation;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;

namespace AdminBackend.Application.Features.ExtractionManagement.ExtractorEngineTypeHandlers
{
    public record UpdateExtractorEngineTypeCommand(
        long Id,
        string Identifier,
        bool Word,
        bool Txt,
        bool Pdf) : ICommand<UpdateExtractorEngineTypeCommandResult>;
    public record UpdateExtractorEngineTypeCommandResult(
        long Id,
        string Identifier,
        bool Word,
        bool Txt,
        bool Pdf) ;

    public class UpdateExtractorEngineTypeCommandValidator : AbstractValidator<UpdateExtractorEngineTypeCommand>
    {
        public UpdateExtractorEngineTypeCommandValidator()
        {
            //TODO : Unique check
            RuleFor(x => x.Identifier).NotNull().NotEmpty().MaximumLength(50);
        }
    }
    internal class UpdateExtractorEngineTypeCommandHandler(IRepository<ExtractorEngineType> repository, IMapper mapper)
        : ICommandHandler<UpdateExtractorEngineTypeCommand, UpdateExtractorEngineTypeCommandResult>
    {
        private readonly IRepository<ExtractorEngineType> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<UpdateExtractorEngineTypeCommandResult> Handle(UpdateExtractorEngineTypeCommand request, CancellationToken cancellationToken)
        {
            var entitiy = mapper.Map<ExtractorEngineType>(request);
            repository.Update(entitiy);
            await repository.SaveChangesAsync(cancellationToken);
            var result = mapper.Map<UpdateExtractorEngineTypeCommandResult>(entitiy);
            return result;
        }
    }
}
