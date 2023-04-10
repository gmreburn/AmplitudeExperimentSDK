namespace Amplitude
{
    using System.Collections.Specialized;
    using System.Net.Http.Headers;
    using System.Web;
    using Newtonsoft.Json.Converters;
    using Newtonsoft.Json;
    using System.Dynamic;
    using System.Net.Http;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class ExperimentClient
    {
        private readonly HttpClient client;
        private readonly ExperimentUser experimentUser;

        /// <summary>
        /// The SDK client should be initialized in your server on startup. The deployment key argument passed into the apiKey parameter must live within the same project that you are sending analytics events to.
        /// </summary>
        /// <param name="deploymentKey">The deployment key which authorizes fetch requests and determines which flags should be evaluated for the user.</param>
        /// <param name="experimentUser">The user to remote fetch variants for.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public ExperimentClient(string deploymentKey, ExperimentUser experimentUser = null)
            : this(new HttpClient { DefaultRequestHeaders = { Authorization = new AuthenticationHeaderValue("Api-Key", deploymentKey) } }, experimentUser)
        {
            if (deploymentKey is null)
            {
                throw new ArgumentNullException(nameof(deploymentKey));
            }
        }

        internal ExperimentClient(HttpClient client, ExperimentUser experimentUser = null)
        {
            if (client is null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            this.client = client;
            this.experimentUser = experimentUser;
        }

        /// <summary>
        /// Fetches variants for a user and returns the results. This function remote evaluates the user for flags associated with the deployment used to initialize the SDK client.
        /// </summary>
        /// <param name="flagKey">The flag key to fetch. Returns all flag variants when not provided.</param>
        /// <param name="experimentUser">The user to remote fetch variants for. Overrides the user set during initialization of ExperimentClient.</param>
        /// <returns>Evaluated variants for each flag configured</returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<IEnumerable<ExperimentVariant>> VariantAsync(string flagKey = null, ExperimentUser experimentUser = null)
        {
            // https://api.lab.amplitude.com/v1/vardata?&user_id=a&device_id=b&flag_key=f&context=c
            var user_id = "";
            var device_id = "";
            var context = "";
            if (experimentUser != null)
            {
                user_id = experimentUser.UserId;
                device_id = experimentUser.DeviceId;
                context = experimentUser.Context;
            }
            else if (this.experimentUser != null)
            {
                user_id = this.experimentUser.UserId;
                device_id = this.experimentUser.DeviceId;
                context = this.experimentUser.Context;
            }

            var qs = QueryStringHelper(new NameValueCollection{
                { "user_id", user_id },
                { "device_id", device_id },
                { "flag_key", flagKey },
                { "context", context }
            });
            var uriBuilder = new UriBuilder("https", "api.lab.amplitude.com", 443, "/v1/vardata") { Query = qs };
            var resp = await client.GetAsync(uriBuilder.Uri);

            if (resp.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var converter = new ExpandoObjectConverter();
                var content = await resp.Content.ReadAsStringAsync();
                dynamic obj = JsonConvert.DeserializeObject<ExpandoObject>(content, converter);
                var variants = new List<ExperimentVariant>();

                foreach (var prop in obj)
                {
                    variants.Add(new ExperimentVariant(prop.Key, prop.Value.key));
                }

                return variants;
            }
            else if (resp.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                throw new NotImplementedException("400 - Bad request");
            }
            else if (resp.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                throw new NotImplementedException("401 - Unauthorized");
            }
            else
            {
                throw new NotImplementedException(resp.StatusCode.ToString());
            }
        }

        static string QueryStringHelper(NameValueCollection query)
        {
            // omitted argument checking

            // sneaky way to get a HttpValueCollection (which is internal)
            var collection = HttpUtility.ParseQueryString(string.Empty);

            foreach (var key in query.Cast<string>().Where(key => !string.IsNullOrEmpty(query[key])))
            {
                collection[key] = query[key];
            }

            return collection.ToString();
        }
    }
}