namespace ApiServerLibraryTest.Data
{
    public class ScopeAppAuthorization
    {
        public int ScopeId { get; set; }
        public Scope Scope { get; set; }
        public long AppAuthorizationId { get; set; }
        public AppAuthorization AppAuthorization { get; set; }
    }
}
