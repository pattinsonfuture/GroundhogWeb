using GroundhogWeb.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace GroundhogWeb.Repositories;
public class UsersRepository
{
    private readonly IMongoCollection<User> _usersCollection;

    public UsersRepository(
        IOptions<DatabaseSettings> databaseSettings)
    {
        var mongoClient = new MongoClient(
            databaseSettings.Value.MongoDBConnectionUri);

        var mongoDatabase = mongoClient.GetDatabase(
            databaseSettings.Value.MongoDBDatabaseName);

        _usersCollection = mongoDatabase.GetCollection<User>("users");
    }

    public async Task<List<User>> GetAllAsync() =>
        await _usersCollection.Find(_ => true).ToListAsync();

    public async Task<User?> GetByIdAsync(string id) =>
        await _usersCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task CreateAsync(User newUser) =>
        await _usersCollection.InsertOneAsync(newUser);

    public async Task UpdateAsync(string id, User updatedUser) =>
        await _usersCollection.ReplaceOneAsync(x => x.Id == id, updatedUser);

    public async Task RemoveAsync(string id) =>
        await _usersCollection.DeleteOneAsync(x => x.Id == id);
}
