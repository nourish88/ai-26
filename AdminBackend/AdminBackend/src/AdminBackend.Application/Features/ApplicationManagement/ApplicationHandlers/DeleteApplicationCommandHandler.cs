using AutoMapper;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;

namespace AdminBackend.Application.Features.ApplicationManagement.ApplicationHandlers
{

    public record DeleteApplicationCommand(long Id):ICommand<DeleteApplicationCommandResult>;
    public record DeleteApplicationCommandResult(bool result);    

    internal class DeleteApplicationCommandHandler(IRepository<Domain.Entities.Application> repository, IMapper mapper) 
        : ICommandHandler<DeleteApplicationCommand, DeleteApplicationCommandResult>
    {
        private readonly IRepository<Domain.Entities.Application> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<DeleteApplicationCommandResult> Handle(DeleteApplicationCommand request, CancellationToken cancellationToken)
        {
            repository.Delete(request.Id);
            await repository.SaveChangesAsync(cancellationToken);
            return new DeleteApplicationCommandResult(true);
        }
    }
}
