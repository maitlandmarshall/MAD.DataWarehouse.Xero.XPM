using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MIFCore.Common;
using MIFCore.Settings;
using OAuthB0ner;
using OAuthB0ner.Storage;
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

        public void Configure(StorageOptions storageOptions)
        {
            // This is a requirement from Xero itself.
            // https://www.absia.asn.au/industry-standards/addon-security-standard/ABSIA-Security-Standard-for-Add-on-Marketplaces.pdf
            if (storageOptions.IsEncryptionOn == false)
                throw new XeroSecurityRequirementException("Encryption is required for Xero token storage. Please set the 'oauth:storage:isEncryptionOn' configuration value to 'true'.");
        }

        public void PostConfigure()
        {

        }
    }
}