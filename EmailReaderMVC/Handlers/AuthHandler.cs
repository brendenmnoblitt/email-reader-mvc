using Google.Apis.Auth.OAuth2;
using Google.Apis.Util;

namespace EmailReaderMVC.Handlers
{
    internal class AuthHandler
    {
        public AuthHandler() { }

        public UserCredential GetCredentials()
        {
            string clientId;
            string clientSecret;
            (clientId, clientSecret) = FetchCredsFile();

            string[] scopes = { "https://www.googleapis.com/auth/gmail.readonly" };
            var credentials = GoogleWebAuthorizationBroker.AuthorizeAsync(
                new ClientSecrets
                {
                    ClientId = clientId,
                    ClientSecret = clientSecret,
                },
                scopes, "user", CancellationToken.None).Result;

            if (credentials.Token.IsStale == true)
            {
                credentials.RefreshTokenAsync(CancellationToken.None).Wait();
            }

            return credentials;
        }

        private (string, string) FetchCredsFile()
        {
            string clientId;
            string clientSecret;
            string secretsFolder = Path.Combine(Directory.GetCurrentDirectory(), "secrets");
            string secretsFile = Path.Combine(secretsFolder, "apicreds.json");
            using (StreamReader  reader = new StreamReader(secretsFile))
            {
                string json = reader.ReadToEnd();
                dynamic secrets = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
                clientId = secrets.clientId;
                clientSecret = secrets.clientSecret;
            }

            return (clientId, clientSecret);

        }
    }
}
