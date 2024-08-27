using SQLite;
using MauiSqliteDemo;

public class LocalDbService
{
    private const string DB_NAME = "Comments.db3"; // Ensure this matches the name of your database file
    private readonly SQLiteAsyncConnection _connection;

    public LocalDbService()
    {
        // Get the path to the app's local data directory
        var dbPath = Path.Combine(FileSystem.AppDataDirectory, DB_NAME);

        Console.WriteLine($"Database file path: {dbPath}");

        // Ensure the database file exists; if not, copy it from the app resources
        if (!File.Exists(dbPath))
        {
            var assembly = typeof(LocalDbService).Assembly;
            using (var stream = assembly.GetManifestResourceStream($"MauiSqliteDemo.Resources.raw.{DB_NAME}"))
            using (var fileStream = new FileStream(dbPath, FileMode.Create, FileAccess.Write))
            {
                stream.CopyTo(fileStream);
            }
        }

        // Create a connection to the database
        _connection = new SQLiteAsyncConnection(dbPath);
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

    public async Task Create(Comment Comment)
    {
        await _connection.InsertAsync(Comment);
    }

    public async Task Update(Comment Comment)
    {
        await _connection.UpdateAsync(Comment);
    }

    public async Task Delete(Comment Comment)
    {
        await _connection.DeleteAsync(Comment);
    }
    public async Task ClearAll()
    {
        await _connection.DeleteAllAsync<Comment>();
    }
}
//adb pull /data/data/com.companyname.mauisqlitedemo/databases/Comments.db3 path/to/your/local/directory
//adb pull /data/user/0/com.companyname.mauisqlitedemo/files/comments.db3 C:\Users\Nakhle\Documents\comments.db3