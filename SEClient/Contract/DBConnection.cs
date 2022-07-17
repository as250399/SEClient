using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SEClient.Contract
{
    public class DBConnection
    {
        [JsonProperty(PropertyName = "DBserverName")]
        public string DBserverName { get; set; }

        [JsonProperty(PropertyName = "DBname")]
        public string DBname { get; set; }

        [JsonProperty(PropertyName = "DBUserName")]
        public string DBUserName { get; set; }

        [JsonProperty(PropertyName = "DBUserPassword")]
        public string DBUserPassword { get; set; }
    }
}