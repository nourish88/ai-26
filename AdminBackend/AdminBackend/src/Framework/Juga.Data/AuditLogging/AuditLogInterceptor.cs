namespace Juga.Data.AuditLogging;
public class AuditLogInterceptor : SaveChangesInterceptor
{
    private readonly UnitOfWorkOptions _unitOfWorkOptions;
    private readonly IAuditEventCreator _auditEventCreator;
    private readonly IAuditLogStore _auditLogStore;
    private readonly IAuditBehaviourService _auditBehaviourService;
    private readonly IUserContextProvider _clientInfoProvider;

    public AuditLogInterceptor(IOptions<UnitOfWorkOptions> options
        , IAuditEventCreator auditEventCreator
        , IAuditLogStore auditLogStore
        , IAuditBehaviourService auditBehaviourService
        , IUserContextProvider clientInfoProvider)
    {
        _unitOfWorkOptions = options.Value;
        _auditEventCreator = auditEventCreator;
        _auditLogStore = auditLogStore;
        _auditBehaviourService = auditBehaviourService;
        _clientInfoProvider = clientInfoProvider;
    }

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData
        , InterceptionResult<int> result
        , CancellationToken cancellationToken = default
    )
    {
        SavingChangesInternal(eventData);
        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        SavingChangesInternal(eventData);

        return base.SavingChanges(eventData, result);
    }

    private void SavingChangesInternal(DbContextEventData eventData)
    {
        var enableDataAudit = eventData.Context is not null && _unitOfWorkOptions.EnableDataAudit && _auditBehaviourService.AuditBehaviour == AuditBehaviour.Enabled;

        if (enableDataAudit)
        {
            _auditEventCreator.SetChanges(eventData.Context);
        }
    }

    public override int SavedChanges(SaveChangesCompletedEventData eventData, int result)
    {
        SavedChangesInternal(eventData, result);
        return base.SavedChanges(eventData, result);
    }

    public override ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result, CancellationToken cancellationToken = default)
    {
        SavedChangesInternal(eventData, result);
        return base.SavedChangesAsync(eventData, result, cancellationToken);
    }

    private void SavedChangesInternal(SaveChangesCompletedEventData eventData, int result)
    {
        var enableDataAudit = result > 0 && _unitOfWorkOptions.EnableDataAudit && _auditBehaviourService.AuditBehaviour == AuditBehaviour.Enabled;

        if (enableDataAudit)
        {
            var timestamp = DateTime.Now;
            var events = _auditEventCreator.GetEvents(eventData.Context, _clientInfoProvider, timestamp);
            _auditLogStore.StoreAuditEvents(events);
        }
    }
}