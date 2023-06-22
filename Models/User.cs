using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GroundhogWeb.Models;

public class User
{
    public string Id { get; set; }
    public string Username { get; set; }
    public string Discriminator { get; set; }
    public string Avatar { get; set; }
    public bool Bot { get; set; }
    public bool System { get; set; }
    public bool MfaEnabled { get; set; }
    public string Locale { get; set; }
    public bool Verified { get; set; }
    public string Email { get; set; }
    public int Flags { get; set; }
    public int PremiumType { get; set; }
    public int PublicFlags { get; set; }
}