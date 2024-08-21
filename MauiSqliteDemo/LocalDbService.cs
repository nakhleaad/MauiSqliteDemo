using SQLite;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Maui.Storage;
using MauiSqliteDemo;

public class LocalDbService
{
    private const string DB_NAME = "customers.db3"; // Ensure this matches the name of your database file
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
        _connection.CreateTableAsync<Customer>().Wait();
    }

    public async Task<List<Customer>> GetCustomers()
    {
        return await _connection.Table<Customer>().ToListAsync();
    }

    public async Task<Customer> GetById(int id)
    {
        return await _connection.Table<Customer>().Where(x => x.Id == id).FirstOrDefaultAsync();
    }

    public async Task Create(Customer customer)
    {
        await _connection.InsertAsync(customer);
    }

    public async Task Update(Customer customer)
    {
        await _connection.UpdateAsync(customer);
    }

    public async Task Delete(Customer customer)
    {
        await _connection.DeleteAsync(customer);
    }
    public async Task ClearAll()
    {
        await _connection.DeleteAllAsync<Customer>();
    }
}
//adb pull /data/data/com.companyname.mauisqlitedemo/databases/customers.db3 path/to/your/local/directory
