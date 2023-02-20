using Hangfire;
using Hangfire.Common;
using MAD.DataWarehouse.Xero.XPM.Jobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MIFCore.Common;
using MIFCore.Hangfire.APIETL;
using MIFCore.Hangfire.APIETL.SqlServer;
using MIFCore.Settings;
using OAuthB0ner;
using OAuthB0ner.Storage;
using OAuthB0ner.Storage.SqlServer;
using System;
using System.Threading.Tasks;

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

            serviceDescriptors.AddTransient<AuthenticationDelegatingHandler>();
            serviceDescriptors
                .AddHttpClient(string.Empty, client =>
                {
                    client.BaseAddress = new Uri("https://api.xero.com/practicemanager/3.1/");
                })
                .AddHttpMessageHandler<AuthenticationDelegatingHandler>();

            serviceDescriptors.AddScoped<ApiEndpointRegisterJob>();

            serviceDescriptors.AddApiEndpointsToExtract();
            serviceDescriptors.AddApiEndpointsSqlServerDestination();
        }

        public void Configure(StorageOptions storageOptions)
        {
            // This is a requirement from Xero itself.
            // https://www.absia.asn.au/industry-standards/addon-security-standard/ABSIA-Security-Standard-for-Add-on-Marketplaces.pdf
            if (storageOptions.IsEncryptionOn == false)
                throw new XeroSecurityRequirementException("Encryption is required for Xero token storage. Please set the 'oauth:storage:isEncryptionOn' configuration value to 'true'.");

            JobFilterProviders.Providers.Add(new JobFilterProvider());
        }

        public async Task PostConfigure(IRecurringJobManager recurringJobManager, ApiEndpointRegisterJob apiEndpointRegisterJob)
        {
            recurringJobManager.AddOrUpdate<ApiEndpointRegisterJob>("Ensure Endpoints are Registered", y => y.EnsureEndpointsAreRegistered(), Cron.Daily());
            await apiEndpointRegisterJob.EnsureEndpointsAreRegistered();
        }
    }
}