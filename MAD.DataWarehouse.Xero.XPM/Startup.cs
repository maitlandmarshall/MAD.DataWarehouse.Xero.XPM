using Hangfire;
using MAD.DataWarehouse.Xero.XPM.Database;
using MAD.DataWarehouse.Xero.XPM.Jobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MIFCore.Common;
using MIFCore.Settings;
using OAuthB0ner;
using OAuthB0ner.Storage;
using OAuthB0ner.Storage.SqlServer;
using System;

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

            serviceDescriptors.AddDbContextFactory<XeroDbContext>((svc, opt) =>
            {
                var appConfig = svc.GetRequiredService<AppConfig>();
                opt.UseSqlServer(appConfig.ConnectionString);
            });

            serviceDescriptors.AddTransient<AuthenticationDelegatingHandler>();
            serviceDescriptors
                .AddHttpClient("xero", client =>
                {
                    client.BaseAddress = new Uri("https://api.xero.com/practicemanager/3.1/");
                })
                .AddHttpMessageHandler<AuthenticationDelegatingHandler>();

            serviceDescriptors.AddScoped<ExtractApiDataJob>();
        }

        public void Configure(StorageOptions storageOptions)
        {
            // This is a requirement from Xero itself.
            // https://www.absia.asn.au/industry-standards/addon-security-standard/ABSIA-Security-Standard-for-Add-on-Marketplaces.pdf
            if (storageOptions.IsEncryptionOn == false)
                throw new XeroSecurityRequirementException("Encryption is required for Xero token storage. Please set the 'oauth:storage:isEncryptionOn' configuration value to 'true'.");
        }

        public void PostConfigure(IRecurringJobManager recurringJobManager, IDbContextFactory<XeroDbContext> dbContextFactory)
        {
            using var db = dbContextFactory.CreateDbContext();
            db.Database.Migrate();

            recurringJobManager.AddOrUpdate<ExtractApiDataJob>("CreateRecurringJobs", y => y.CreateRecurringJobs(), Cron.Daily());
        }
    }
}