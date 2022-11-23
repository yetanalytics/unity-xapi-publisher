using System;
using System.Dynamic;
using System.Collections.Generic;

namespace XAPI {
    public interface IExtension {
        Dictionary<String,ExpandoObject> extensions {set;get;}
    }
}