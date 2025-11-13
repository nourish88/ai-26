namespace AdminBackend.Api.Policies
{
    public class PolicyNames
    {
        public const string AdminPolicyName = "RequireAdmin";
        public const string ApplicationPolicyName = "RequireAppHeaderAndRole";
        public const string EitherPolicy = "EitherPolicy";
    }
}
