using System;
using System.Text.Json.Nodes;

namespace XAPI {
    public class Result : IExtension {
        public Score score { get; set; }
        public Boolean success { get; set; }
        public Boolean completion { get; set; }
        public String response { get; set; }
        public String duration { get; set; }
        public JsonObject extensions {set;get;}

    }
}
