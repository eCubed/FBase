using FBase.Foundations;

namespace FBase.ApiServer.OAuth
{
    internal class DummyCredentialValuesProvider : ICredentialValuesProvider
    {
        public string GenerateClientId()
        {
            return Randomizer.GenerateString(5, "ABCDEFGHJK012345") + "-" +
                Randomizer.GenerateString(7, "ABCDEFGHJKLMNPQRSTUVWXYZ0123456789abcdefghjklmnpqrstuvwxyz") + "-" +
                Randomizer.GenerateString(5, "ABCDEFGHJK012345");
        }

        public string GenerateClientSecret()
        {
            return Randomizer.GenerateString(30, "ABCDEFGHJKLMNPQRSTUVWXYZ0123456789abcdefghjklmnpqrstuvwxyz") + "-" +
                Randomizer.GenerateString(3, "ABCDEFG1234567");
        }
    }
}
