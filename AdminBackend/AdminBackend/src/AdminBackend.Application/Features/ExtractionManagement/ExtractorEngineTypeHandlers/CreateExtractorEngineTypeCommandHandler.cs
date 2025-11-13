using AdminBackend.Domain.Entities;
using AutoMapper;
using FluentValidation;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;

namespace AdminBackend.Application.Features.ExtractionManagement.ExtractorEngineTypeHandlers
{

    public record CreateExtractorEngineTypeCommand(
        string Identifier,
        bool Word,
        bool Txt,
        bool Pdf) : ICommand<CreateExtractorEngineTypeCommandResult>;
    public record CreateExtractorEngineTypeCommandResult(
        long Id,
        string Identifier,
        bool Word,
        bool Txt,
        bool Pdf);

    public class CreateExtractorEngineTypeCommandValidator : AbstractValidator<CreateExtractorEngineTypeCommand>
    {
        public CreateExtractorEngineTypeCommandValidator()
        {
            //TODO : Unique check
            RuleFor(x=>x.Identifier).NotNull().NotEmpty().MaximumLength(50);
        }
    }

    internal class CreateExtractorEngineTypeCommandHandler(IRepository<ExtractorEngineType> repository, IMapper mapper) 
        : ICommandHandler<CreateExtractorEngineTypeCommand, CreateExtractorEngineTypeCommandResult>
    {
        private readonly IRepository<ExtractorEngineType> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<CreateExtractorEngineTypeCommandResult> Handle(CreateExtractorEngineTypeCommand request, CancellationToken cancellationToken)
        {
            var entitiy = mapper.Map<ExtractorEngineType>(request);
            entitiy = await repository.InsertAsync(entitiy,cancellationToken);
            await repository.SaveChangesAsync(cancellationToken);
            var result = mapper.Map<CreateExtractorEngineTypeCommandResult>(entitiy);
            return result;
        }
    }
}
