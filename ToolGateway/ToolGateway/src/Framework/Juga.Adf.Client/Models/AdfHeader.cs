using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Juga.Adf.Client.Models
{
    [DataContract]
    public class AdfHeader
    {
        [DataMember]
        public Guid CorrelationId { get; set; }

        [DataMember]
        public long SessionId { get; set; }

        [DataMember]
        public string UserName { get; set; }

        [DataMember]
        public long UserId { get; set; }

        [DataMember]
        public string UserIp { get; set; }

        [DataMember]
        public string MachineName { get; set; }

        [DataMember]
        public string UserCulture { get; set; }

        [DataMember]
        public long CurrentModuleId { get; set; }

        [DataMember]
        public long CurrentViewId { get; set; }

        [DataMember]
        public string ModuleName { get; set; }

        [DataMember]
        public long UserIdentity { get; set; }

        [DataMember]
        public long UserUnitId { get; set; }
        [DataMember]
        public string FullName { get; set; }

        [DataMember]
        public ApplicationSandbox ApplicationSandbox { get; set; }
    }
    public enum ApplicationSandbox
    {
        UNKNOWN = 0,
        WCF = 1,
        MVC = 2,
        WebAPI = 3,
        WPF = 4,
        INTEGRATION = 5
    }

}
