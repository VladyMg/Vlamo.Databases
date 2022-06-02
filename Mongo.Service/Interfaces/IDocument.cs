using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;

namespace Mongo.Domain.Interfaces
{
    public interface IDocument
    {

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [JsonIgnore]
        public string Id { get; set; }

        [BsonElement(nameof(CreatedDate))]
        [JsonIgnore]
        public DateTime CreatedDate { get; set; }

        [BsonElement(nameof(LastUpdate))]
        [JsonIgnore]
        public DateTime LastUpdate { get; set; }
    }
}
