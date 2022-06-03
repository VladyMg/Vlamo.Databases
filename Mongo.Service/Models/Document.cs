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

        [BsonElement(nameof(CreatedBy))]
        [BsonIgnoreIfNull]
        [BsonIgnoreIfDefault]
        [JsonIgnore]
        public string CreatedBy { get; set; }

        [BsonElement(nameof(CreatedDate))]
        [BsonIgnoreIfNull]
        [BsonIgnoreIfDefault]
        [JsonIgnore]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [BsonElement(nameof(ModifiedBy))]
        [BsonIgnoreIfNull]
        [BsonIgnoreIfDefault]
        [JsonIgnore]
        public string ModifiedBy { get; set; }

        [BsonElement(nameof(ModifiedDate))]
        [BsonIgnoreIfNull]
        [BsonIgnoreIfDefault]
        [JsonIgnore]
        public DateTime ModifiedDate { get; set; } = DateTime.UtcNow;

        [BsonElement(nameof(Status))]
        [BsonIgnoreIfNull]
        [BsonIgnoreIfDefault]
        [JsonIgnore]
        public bool Status { get; set; } = true;
    }
}
