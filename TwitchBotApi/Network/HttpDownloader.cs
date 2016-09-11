using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace TwitchBotApi.Network
{
    /*
     * Helper class for web requests.
     */ 
    public static class HttpDownloader
    {
        private static HttpClient client = new HttpClient() { Timeout = TimeSpan.FromSeconds(5) };

        //Download a raw json string and deserialize it to the given type. 
        public static async Task<T> GetJsonAsync<T>(string url)
        {
            string json = await client.GetStringAsync(url);
            return JsonConvert.DeserializeObject<T>(json);
        }

        //Download a raw json string and deserialize it to the given type. 
        public static T GetJson<T>(string url)
        {
            return GetJsonAsync<T>(url).GetAwaiter().GetResult();
        }
    }
}
