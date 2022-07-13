using System;
using System.Text.Json.Serialization;

namespace XAPI {
    public class LanguageMap {
        [JsonPropertyName("en-US")]
        public String enUS {set; get;}
    }
}