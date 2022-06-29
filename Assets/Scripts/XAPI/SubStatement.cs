using System;
using System.Collections.Generic;

namespace XAPI {
    class SubStatement<TActorType, TObjectType> : IStatement<TActorType, TObjectType>, IObjekt
        where TActorType : IActor
        where TObjectType : IObjekt
    {
        public static readonly String OBJECT_TYPE = "SubStatement";
        public String objectType { get {return OBJECT_TYPE; }}
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