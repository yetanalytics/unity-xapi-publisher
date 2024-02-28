using System;
using System.Collections.Generic;

namespace XAPI {
    public class ActivityDefinition : IExtension {
        public LanguageMap name {set;get;}
        public LanguageMap description {set;get;}
        public String type {set;get;}
        public String moreInfo {set;get;}
        public Dictionary<string, object> extensions {set;get;}
    }
}
