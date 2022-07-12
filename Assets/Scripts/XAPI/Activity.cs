using System;

namespace XAPI {
    public class Activity : IObjekt {
        public static readonly String OBJECT_TYPE = "Activity";
        public String objectType { get  {return OBJECT_TYPE; }}
        public String id { set;get; }
        public ActivityDefinition definition { set; get; }
    }
}