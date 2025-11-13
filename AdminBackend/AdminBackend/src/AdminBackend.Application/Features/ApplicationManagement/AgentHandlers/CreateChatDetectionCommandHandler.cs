using AdminBackend.Domain.Constants;
using AdminBackend.Domain.Entities;
using AutoMapper;
using FluentValidation;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;

namespace AdminBackend.Application.Features.ApplicationManagement.AgentHandlers
{

    public record CreateChatDetectionRequest(
        ChatDetectionTypes ChatDetectionTypeId,
        string ThreadId,
        string MessageId,
        string UserMessage,
        string? Sources,
        string? Reason);

    public record CreateChatDetectionCommand(
        string ApplicationIdentifier,
        ChatDetectionTypes ChatDetectionTypeId,
        string ThreadId,
        string MessageId,
        string UserMessage,
        string? Sources,
        string? Reason) : ICommand<CreateChatDetectionCommandResult>;
    public record CreateChatDetectionCommandResult(
        long Id,
        string ApplicationIdentifier,
        ChatDetectionTypes ChatDetectionTypeId,
        string ThreadId,
        string MessageId,
        string UserMessage,
        string? Sources,
        string? Reason);

    public class CreateChatDetectionCommandValidator : AbstractValidator<CreateChatDetectionCommand>
    {
        public CreateChatDetectionCommandValidator()
        {
            RuleFor(x => x.ApplicationIdentifier).NotNull().NotEmpty().MaximumLength(50);
            RuleFor(x => x.ThreadId).NotNull().NotEmpty().MaximumLength(50);
            RuleFor(x => x.MessageId).NotNull().NotEmpty().MaximumLength(50);
            RuleFor(x => x.UserMessage).NotNull().NotEmpty();
        }
    }

    internal class CreateChatDetectionCommandHandler(IRepository<ChatDetection> repository, IMapper mapper) 
        : ICommandHandler<CreateChatDetectionCommand, CreateChatDetectionCommandResult>
    {
        private readonly IRepository<ChatDetection> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<CreateChatDetectionCommandResult> Handle(CreateChatDetectionCommand request, CancellationToken cancellationToken)
        {
            var entitiy = mapper.Map<ChatDetection>(request);
            entitiy = await repository.InsertAsync(entitiy,cancellationToken);
            await repository.SaveChangesAsync(cancellationToken);
            var result = mapper.Map<CreateChatDetectionCommandResult>(entitiy);
            return result;
        }
    }
}
