namespace Amplitude.Experiment
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;

    public class ExperimentClient
    {
        private const string ApiBaseUrl = "https://api.lab.amplitude.com/v1/vardata";
        private const string HeaderApiKey = "Api-Key";

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
            if (string.IsNullOrEmpty(deploymentKey))
            {
                throw new ArgumentNullException(nameof(deploymentKey));
            }
            this.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(HeaderApiKey, deploymentKey);
        }

        internal ExperimentClient(HttpClient client, ExperimentUser experimentUser = null)
        {
            this.client = client ?? throw new ArgumentNullException(nameof(client)); 

            this.experimentUser = experimentUser;
        }

        /// <summary>
        /// Fetches variants for a user and returns the results. This function remote evaluates the user for flags associated with the deployment used to initialize the SDK client.
        /// </summary>
        /// <param name="flagKey">The flag key to fetch. Returns all flag variants when not provided.</param>
        /// <param name="experimentUser">The user to remote fetch variants for. Overrides the user set during initialization of ExperimentClient.</param>
        /// <returns>Evaluated variants for each flag configured</returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<IEnumerable<ExperimentVariant>> VariantAsync(string flagKey = null, ExperimentUser customUser = null)
        {
            // https://api.lab.amplitude.com/v1/vardata?&user_id=a&device_id=b&flag_key=f&context=c
            var user_id = customUser?.UserId ?? this.experimentUser?.UserId ?? "";
            var device_id = customUser?.DeviceId ?? this.experimentUser?.DeviceId ?? "";
            var context = customUser?.Context ?? this.experimentUser?.Context ?? "";

            var queryString = $"user_id={user_id}&device_id={device_id}&flag_key={flagKey}&context={context}";
            var uriBuilder = new UriBuilder(ApiBaseUrl)
            {
                Query = queryString
            };

            var resp = await client.GetAsync(uriBuilder.Uri);

            if (resp.IsSuccessStatusCode)
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
            else if (resp.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                throw new UnauthorizedAccessException("Invalid API Key.");
            }
            else
            {
                resp.EnsureSuccessStatusCode();
                throw new NotImplementedException("Unreachable code.");
            }
        }
    }
}