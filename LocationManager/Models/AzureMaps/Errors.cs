using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace LocationManager.Models.AzureMaps
{
    public class ErrorResponse
    {
        [JsonProperty("error")]
        public Error ErrorObject { get; set; }
    }

    public class Error
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
