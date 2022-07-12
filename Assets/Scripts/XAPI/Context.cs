using System;
using System.Dynamic;
using System.Collections.Generic;

namespace XAPI {
    public class Context: IExtension {
        public String registration { set;get; }
        public Agent instructor { set;get; }
        public ContextActivity contextActivities { set;get; }
        public String revision { set;get; }
        public String platform { set;get; }
        public String language { set;get; }
        public StatementReference statement { set;get; }
        public Dictionary<String,ExpandoObject> extensions { set;get; }
    }
}