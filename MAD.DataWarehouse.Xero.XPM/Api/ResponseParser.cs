using MIFCore.Hangfire.APIETL;
using MIFCore.Hangfire.APIETL.Transform;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MAD.DataWarehouse.Xero.XPM.Api
{
    [ApiEndpointSelector(".*")]
    internal class ResponseParser : IParseResponse
    {
        public async Task<IEnumerable<IDictionary<string, object>>> OnParse(ParseResponseArgs args)
        {
            var xml = XDocument.Parse(args.ApiData.Data);
            var jsonRaw = JsonConvert.SerializeObject(xml);
            var json = JsonConvert.DeserializeObject<ExpandoObject>(jsonRaw) as IDictionary<string, object>;

            var listProperty = args.Endpoint.Name switch
            {
                "cost.api/list" => "Costs",
                "category.api/list" => "Categories",
                "clientgroup.api/list" => "Groups",
                "staff.api/list" => "StaffList",
                "task.api/list" => "TaskList",
                "job.api/list" => "Jobs",
                "invoice.api/list" => "Invoices",
                "client.api/list" => "Clients",
                "" or _ => throw new NotImplementedException()
            };

            var listContainer = (json["Response"] as IDictionary<string, object>)[listProperty] as IDictionary<string, object>;

            if (listContainer != null
                && listContainer.Any())
            {
                // This will either be a list or a single item
                var firstValue = listContainer.First().Value;

                if (firstValue is IEnumerable<object> array)
                {
                    var result = array
                        .Cast<IDictionary<string, object>>()
                        .ToList();

                    result.FlattenGraph();

                    return result;
                }
                else if (firstValue is ExpandoObject expando)
                {
                    expando.FlattenGraph();

                    return new[] { expando };
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
            else
            {
                return null;
            }
        }
    }
}
