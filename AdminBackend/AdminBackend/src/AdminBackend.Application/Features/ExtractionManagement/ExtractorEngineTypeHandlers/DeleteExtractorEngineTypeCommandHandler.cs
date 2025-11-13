using AdminBackend.Domain.Entities;
using AutoMapper;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;

namespace AdminBackend.Application.Features.ExtractionManagement.ExtractorEngineTypeHandlers
{

    public record DeleteExtractorEngineTypeCommand(long Id):ICommand<DeleteExtractorEngineTypeCommandResult>;
    public record DeleteExtractorEngineTypeCommandResult(bool result);    

    internal class DeleteExtractorEngineTypeCommandHandler(IRepository<ExtractorEngineType> repository, IMapper mapper) 
        : ICommandHandler<DeleteExtractorEngineTypeCommand, DeleteExtractorEngineTypeCommandResult>
    {
        private readonly IRepository<ExtractorEngineType> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<DeleteExtractorEngineTypeCommandResult> Handle(DeleteExtractorEngineTypeCommand request, CancellationToken cancellationToken)
        {
            repository.Delete(request.Id);
            await repository.SaveChangesAsync(cancellationToken);
            return new DeleteExtractorEngineTypeCommandResult(true);
        }
    }
}
