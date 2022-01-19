using System;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
namespace DigiCoreCLI
{
    public class Account : AppBase
    {
        public Account(DigiCore digiCore) : base(digiCore)
        {

        }
        public const string USER_INFO_PATH = "/v1/account/userinfo/";
        public class UserInfo : AppData
        {
            [JsonPropertyName("username")]
            public string? UserName{ get; set; }
            [JsonPropertyName("email")]
            public string? Email{ get; set; }
            [JsonPropertyName("student_id")]
            public string? StudentId{ get; set; }
            [JsonPropertyName("icon")]
            public string? IconUrl{ get; set; }
        }
    }
}