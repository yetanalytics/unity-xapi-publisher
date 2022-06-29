using System;

namespace XAPI {
    class StatementReference : IObjekt {
        public static readonly String OBJECT_TYPE = "StatementRef";
        public String objectType { get {return OBJECT_TYPE; }}
        public String id { set;get; }
    }
}