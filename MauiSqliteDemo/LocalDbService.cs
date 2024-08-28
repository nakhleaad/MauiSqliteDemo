using SQLite;
using MauiSqliteDemo;

public class LocalDbService
{
    private const string DB_NAME = "Comments.db3";
    private readonly SQLiteAsyncConnection _connection;

    public LocalDbService()
    {
        var dbPath = Path.Combine(FileSystem.AppDataDirectory, DB_NAME);

        _connection = new SQLiteAsyncConnection(dbPath);
        // Ensure the table for the Comment model exists in the database
        _connection.CreateTableAsync<Comment>().Wait();
    }

    public async Task<List<Comment>> GetComments()
    {
        return await _connection.Table<Comment>().ToListAsync();
    }

    public async Task<Comment> GetById(int id)
    {
        return await _connection.Table<Comment>().Where(x => x.Id == id).FirstOrDefaultAsync();
    }

    public async Task Create(Comment comment)
    {
        await _connection.InsertAsync(comment);
    }

    public async Task Update(Comment comment)
    {
        await _connection.UpdateAsync(comment);
    }

    public async Task Delete(Comment comment)
    {
        await _connection.DeleteAsync(comment);
    }

    public async Task ClearAll()
    {
        await _connection.DeleteAllAsync<Comment>();
    }

}
