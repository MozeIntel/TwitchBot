using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace TwitchBotApi.Network
{
    /// <summary>
    /// Helper class for Http requests
    /// </summary>
    public static class HttpDownloader
    {
        private static HttpClient client = new HttpClient() { Timeout = TimeSpan.FromSeconds(5) };

        /// <summary>
        /// Download a JSON string from the URL, and deserialize it to the given type <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">The type to deserialize the JSON to</typeparam>
        /// <param name="url">The URL to download the JSON from</param>
        /// <returns>The deserialized object</returns>
        public static async Task<T> GetJsonAsync<T>(string url)
        {
            string json = await client.GetStringAsync(url);
            return JsonConvert.DeserializeObject<T>(json);
        }

        /// <summary>
        /// Download a JSON string from the URL, and deserialize it to the given type <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">The type to deserialize the JSON to</typeparam>
        /// <param name="url">The URL to download the JSON from</param>
        /// <returns>The deserialized object</returns>
        public static T GetJson<T>(string url)
        {
            return GetJsonAsync<T>(url).GetAwaiter().GetResult();
        }
    }
}
