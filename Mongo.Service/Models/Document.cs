using Mongo.Domain.Interfaces;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;

namespace Mongo.Domain.Models
{
    public class Document : IDocument
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [JsonIgnore]
        public string Id { get; set; }

        [BsonElement(nameof(CreatedDate))]
        [JsonIgnore]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [BsonElement(nameof(LastUpdate))]
        [JsonIgnore]
        public DateTime LastUpdate { get; set; } = DateTime.UtcNow;
    }
}
