using OAuthB0ner.Storage;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace MAD.DataWarehouse.Xero.XPM
{
    internal class AuthenticationDelegatingHandler : DelegatingHandler
    {
        private readonly ITokenStorage tokenStorage;

        public AuthenticationDelegatingHandler(ITokenStorage tokenStorage)
        {
            this.tokenStorage = tokenStorage;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var accessToken = await this.tokenStorage.GetAccessToken();
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken.AccessToken);

            return await base.SendAsync(request, cancellationToken);
        }
    }
}