using System;
using System.Text;
using System.Text.Json;
using System.Net;
namespace DigiCoreLib
{
    public class Auth
    {
        public enum AuthState { NotLogin, Logined }
        public AuthState? State { get; private set; }
        public bool IsLogined { get => State == AuthState.Logined; }
        public string? jwt,refreshToken;
        private HttpClient httpClient = new();
        private string _baseUrl;
        public Auth(string baseUrl)
        {
            _baseUrl = baseUrl;
        }
        class TokenCheckJson
        {
            public string token{ get; set; }
            public TokenCheckJson(string token)
            {
                this.token = token;
            }
            public string GetJson() => JsonSerializer.Serialize(this);
        }
        public async Task<bool> CheckLogin()
        {
            if(jwt==null)return false;
            var url = _baseUrl + "/api/v1/auth/jwt/verify";
            var content = 
                new StringContent(
                    new TokenCheckJson(jwt).GetJson(), 
                    Encoding.UTF8, 
                    "application/json");  
            var response = 
                await httpClient.PostAsync(
                    url, 
                    content);
            return response.StatusCode == HttpStatusCode.OK;
        }
        class LoginJson
        {
            public string email{ get; set; }
            public string password{ get; set; }
            public LoginJson(string email, string password)
            {
                this.email = email;
                this.password = password;
            }
            public string GetJson() => JsonSerializer.Serialize(this);
        }
        class LoginResJson
        {
            public string access{ get; set; }
            public string refresh{ get; set; }
        }
        public async Task<bool> Login(string email, string password)
        {
            var url = _baseUrl + "/api/v1/auth/jwt/create/";
            var content = 
                new StringContent(
                    new LoginJson(email, password).GetJson(), 
                    Encoding.UTF8, 
                    "application/json");  
            var response = 
                await httpClient.PostAsync(
                    url, 
                    content);
            Console.WriteLine(url+"\n"+response.StatusCode.ToString());
            if(response.StatusCode != HttpStatusCode.OK)
            {
                return false;
            }
            var resJson = await response.Content.ReadAsStringAsync();
            var loginRes = JsonSerializer.Deserialize<LoginResJson>(resJson);
            if(loginRes == null)return false;
            jwt = loginRes.access;
            refreshToken = loginRes.refresh;
            return true;
        }
    }
}