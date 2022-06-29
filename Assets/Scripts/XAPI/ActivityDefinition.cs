using System;
using System.Dynamic;
using System.Collections.Generic;

namespace XAPI {
    class ActivityDefinition : IExtension {
        public LanguageMap name {set;get;}
        public LanguageMap description {set;get;}
        public String type {set;get;}
        public String moreInfo {set;get;}
        public List<ExpandoObject> extensions {set;get;}
    }
}