using System.Dynamic;
using System.Collections.Generic;

namespace XAPI {
    public interface IExtension {
        public List<ExpandoObject> extensions {set;get;}
    }
}