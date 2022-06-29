using System;
using System.Collections.Generic;

namespace XAPI {
    interface IStatement<TActorType, TObjectType> {
        public TActorType actor { get; set; }
        public Verb verb { get; set; }
        public TObjectType objekt { get; set; }
        public Result result { get; set; }
        public Context context { get; set; }
        public String timestamp { get; set; }
        public Agent authority { get;set; }
        public List<Attachment> attachments { set;get; }

    }
}