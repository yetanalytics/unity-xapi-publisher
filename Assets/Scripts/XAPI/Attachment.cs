using System;

namespace XAPI {
    public class Attachment {
        public String usageType { set;get; }
        public LanguageMap display { set;get; }
        public LanguageMap description { set;get; }
        public String contentType { set;get; }
        public int length { set;get; }
        public String sha2 { set;get; }
        public String fileUrl { set;get; }
    }
}