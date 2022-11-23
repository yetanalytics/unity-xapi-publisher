using System;
using System.Collections.Generic;

namespace XAPI {
    interface IStatement<TActorType, TObjectType> {
        TActorType actor { get; set; }
        Verb verb { get; set; }
        TObjectType objekt { get; set; }
        Result result { get; set; }
        Context context { get; set; }
        String timestamp { get; set; }
        Agent authority { get;set; }
        List<Attachment> attachments { set;get; }

    }
}