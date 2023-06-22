using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GroundhogWeb.Models;

public class Plugin
{
    public string name { get; set; }
    public bool isEnabled { get; set; }
    public string useChannel { get; set; }
}