using System;
using System.Collections.Generic;

namespace XAPI {
    public class Group: IActor {
        public static readonly String OBJECT_TYPE = "Group";
        public String objectType { get  {return OBJECT_TYPE; }}
        public String name {set;get;}
        public List<Agent> member {set;get;}
    }
}