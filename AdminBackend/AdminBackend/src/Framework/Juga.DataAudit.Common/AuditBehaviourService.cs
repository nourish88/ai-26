using Juga.Abstractions.Data.AuditLog;

namespace Juga.DataAudit.Common
{
    public class AuditBehaviourService : IAuditBehaviourService
    {
        private AuditBehaviour auditBehaviour;
        public AuditBehaviourService()
        {
            auditBehaviour = AuditBehaviour.Enabled;
        }
        public AuditBehaviour AuditBehaviour
        {
            get { return auditBehaviour; }
            set { auditBehaviour = value; }
        }
    }
}
