using GroundhogWeb.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace GroundhogWeb.Repositories;
public class GuildsRepository
{
    private readonly IMongoCollection<Guild> _guildsCollection;

    public GuildsRepository(
        IOptions<DatabaseSettings> databaseSettings)
    {
        var mongoClient = new MongoClient(
            databaseSettings.Value.MongoDBConnectionUri);

        var mongoDatabase = mongoClient.GetDatabase(
            databaseSettings.Value.MongoDBDatabaseName);

        _guildsCollection = mongoDatabase.GetCollection<Guild>("guilds");
    }

    public async Task<List<Guild>> GetAllAsync() =>
        await _guildsCollection.Find(_ => true).ToListAsync();

    public async Task<Guild?> GetByIdAsync(string id) =>
        await _guildsCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task CreateAsync(Guild newGuild) =>
        await _guildsCollection.InsertOneAsync(newGuild);

    public async Task UpdateAsync(string id, Guild updatedGuild) =>
        await _guildsCollection.ReplaceOneAsync(x => x.Id == id, updatedGuild);

    public async Task RemoveAsync(string id) =>
        await _guildsCollection.DeleteOneAsync(x => x.Id == id);
}
