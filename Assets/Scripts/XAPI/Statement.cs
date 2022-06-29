using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace XAPI {
    class Statement<TActorType, TObjectType> : IStatement<TActorType, TObjectType>
        where TActorType : IActor
        where TObjectType : IObjekt
    {
        public string Serialize()
        {
            var opts = new JsonSerializerOptions();
            opts.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            return JsonSerializer.Serialize<Statement<TActorType,TObjectType>>(this, opts);
        }
        public TActorType actor { get; set; }
        public Verb verb { get; set; }
        [JsonPropertyName("object")]
        public TObjectType objekt { get; set; }
        public Result result { get; set; }
        public Context context { get; set; }
        public String timestamp { get; set; }
        public Agent authority { get;set; }
        public List<Attachment> attachments { set;get; }
    }
}