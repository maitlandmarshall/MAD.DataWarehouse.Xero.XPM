using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MIFCore.Common;
using MIFCore.Settings;
using OAuthB0ner;
using OAuthB0ner.Storage.SqlServer;

namespace MAD.DataWarehouse.Xero.XPM
{
    internal class Startup
    {
        private readonly IConfiguration configuration;

        public Startup()
        {
            this.configuration = Globals.DefaultConfiguration;
        }

        public void ConfigureServices(IServiceCollection serviceDescriptors)
        {
            serviceDescriptors.AddIntegrationSettings<AppConfig>();
            serviceDescriptors.AddControllers();

            serviceDescriptors.AddOAuthB0ner(
                configureOAuthOptions: opt => this.configuration.Bind("oauth", opt),
                configureStorageOptions: opt => this.configuration.Bind("oauth:storage", opt)
            );

            serviceDescriptors.AddOAuthB0nerSqlServerStorage(opt => this.configuration.Bind(opt));
        }

        public void PostConfigure()
        {

        }
    }
}