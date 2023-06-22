using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GroundhogWeb.Models;

public class Guild
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("name")]
    public string GuildName { get; set; } = null!;
    public List<Plugin> plugins { get; set; }
}
