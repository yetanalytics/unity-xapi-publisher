using System.Collections.Generic;

namespace XAPI {
    public class ContextActivity {
        public List<Activity> parent { set; get; }
        public List<Activity> grouping { set; get; }
        public List<Activity> category { set; get; }
        public List<Activity> other { set; get; }
    }
}