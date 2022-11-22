using Hangfire;
using System.Collections.Generic;
using System.Linq;

namespace MAD.DataWarehouse.Xero.XPM.Api
{
    internal class ApiEndpointRegister
    {
        private readonly IDictionary<string, ApiEndpoint> endpoints = new Dictionary<string, ApiEndpoint>();
        private readonly IRecurringJobManager recurringJobManager;

        public ApiEndpointRegister(IRecurringJobManager recurringJobManager)
        {
            this.recurringJobManager = recurringJobManager;
        }

        public IEnumerable<ApiEndpoint> Endpoints { get => this.endpoints.Values.ToArray(); }

        public ApiEndpointRegister Register(ApiEndpoint endpoint)
        {
            this.endpoints.Add(endpoint.Name, endpoint);

            this.recurringJobManager.AddOrUpdate<EndpointExtractJob>(
                endpoint.JobName,
                job => job.Extract(endpoint.Name, null),
                Cron.Daily()
            );

            return this;
        }

        public ApiEndpoint Get(string endpointName)
        {
            return this.endpoints[endpointName];
        }
    }
}
