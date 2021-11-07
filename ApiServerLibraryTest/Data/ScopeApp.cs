namespace ApiServerLibraryTest.Data
{
    public class ScopeApp
    {
        public int ScopeId { get; set; }
        public Scope Scope { get; set; }
        public long AppId { get; set; }
        public App App { get; set; }
    }
}
