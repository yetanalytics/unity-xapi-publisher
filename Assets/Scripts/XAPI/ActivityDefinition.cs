using System;
using System.Dynamic;
using System.Collections.Generic;

namespace XAPI {
    public class ActivityDefinition : IExtension {
        public LanguageMap name {set;get;}
        public LanguageMap description {set;get;}
        public String type {set;get;}
        public String moreInfo {set;get;}
        public Dictionary<String,ExpandoObject> extensions {set;get;}
    }
}