using System;

namespace XAPI {
    public class Agent: IActor {
        public static readonly String OBJECT_TYPE = "Agent";
        public String objectType { get  {return OBJECT_TYPE; }}
        public String name { get; set; }
        public String mbox { get; set; }
        public String mbox_sha1sum { get; set; }
        public AgentAccount account { get; set; }
    }
}