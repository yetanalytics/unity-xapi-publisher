using System;
using System.Text.Json.Nodes;

namespace XAPI {
    public class ActivityDefinition : IExtension {
        public LanguageMap name {set;get;}
        public LanguageMap description {set;get;}
        public String type {set;get;}
        public String moreInfo {set;get;}
        public JsonObject extensions {set;get;}
    }
}
