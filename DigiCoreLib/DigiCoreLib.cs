using System;
using System.Text;
using System.Text.Json;
using System.Net;

namespace DigiCoreLib
{
    public class DigiCore
    {
        public Auth Auth{ get; private set; }
        private string _baseUrl;
        public DigiCore(string baseUrl)
        {
            _baseUrl = baseUrl;
            Auth = new(baseUrl);
        }
        public string GetFullUrl(string path) 
            => _baseUrl + path;
    }
    public class Response<T>
    {
        public HttpStatusCode statusCode;
        public T? data;
        public Response(HttpStatusCode statusCode)
        {
            this.statusCode = statusCode;
        }
    }
    public class AppBase
    {
        DigiCore _digiCore;
        HttpClient httpClient;
        public AppBase(DigiCore digiCore)
        {
            _digiCore = digiCore;
            httpClient = new();

        }
        public class AppData
        {
            public int id{ get; set; }
        }
        
        public async Task<Response<T>> Get<T>(string apiPath) 
            where T : AppData
        {
            httpClient.DefaultRequestHeaders.Add("Authorization", "JWT " + _digiCore.Auth.jwt);
            var httpRes = await httpClient.GetAsync(apiPath);
            if(httpRes.StatusCode != HttpStatusCode.OK)
            {
                return new Response<T>(httpRes.StatusCode);
            }
            var json = await httpRes.Content.ReadAsStringAsync();
            T? data = JsonSerializer.Deserialize<T>(json);
            return new Response<T>(httpRes.StatusCode) { data = data };
        }
        
    }
}