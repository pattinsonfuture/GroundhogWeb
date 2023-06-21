namespace GroundhogWeb.Models;

public class DatabaseSettings
{
    public string MongoDBConnectionUri { get; set; } = null!;

    public string MongoDBDatabaseName { get; set; } = null!;

}