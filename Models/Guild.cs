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

public class Plugin
{
    public string name { get; set; }
    public bool isEnabled { get; set; }
    public string useChannel { get; set; }
}