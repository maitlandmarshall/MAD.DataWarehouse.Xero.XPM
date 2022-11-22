using Microsoft.AspNetCore.Mvc;
using OAuthB0ner.Authorization;
using OAuthB0ner.Storage;
using System.Threading.Tasks;

namespace MAD.DataWarehouse.Xero.XPM.Controllers
{
    [Route("/api/oauth")]
    public class OAuthController : ControllerBase
    {
        private readonly AppConfig appConfig;
        private readonly IAuthorizeApp authorizeApp;
        private readonly ITokenStorageSink tokenStorageSink;

        public OAuthController(
            AppConfig appConfig,
            IAuthorizeApp authorizeApp,
            ITokenStorageSink tokenStorageSink)
        {
            this.appConfig = appConfig;
            this.authorizeApp = authorizeApp;
            this.tokenStorageSink = tokenStorageSink;
        }

        [HttpGet("authorize")]
        public async Task<RedirectResult> Authorize()
        {
            var loginUrl = await this.authorizeApp.GetLoginUrl(new LoginOptions { Scope = this.appConfig.Scope });
            return this.Redirect(loginUrl);
        }

        [HttpGet("callback")]
        public async Task<RedirectToActionResult> Callback(string code = null, string state = null, string error = null)
        {
            if (string.IsNullOrWhiteSpace(error) == false
                || string.IsNullOrWhiteSpace(code))
            {
                return this.RedirectToAction("failure", new { error, state });
            }

            var token = await this.authorizeApp.ExchangeCode(code, state);
            await this.tokenStorageSink.SetAccessToken(token);

            return this.RedirectToAction("success", new { state });
        }

        [HttpGet("callback/success")]
        public OkObjectResult Success(string state)
        {
            return this.Ok("Authentication successful. You can close this window now.");
        }

        [HttpGet("callback/failure")]
        public OkObjectResult Failure(string error, string state)
        {
            return this.Ok(error);
        }
    }
}