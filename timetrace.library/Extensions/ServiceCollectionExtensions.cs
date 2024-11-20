using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using timetrace.library.Context;

public static class ServiceCollectionExtensions
{
    public static void AddDatabaseContextFactory(this IServiceCollection services, string connectionString)
    {

        //need to fetch encrypted password here
        //connection string should look like this
        //"Data Source={0};Password={1}"

        services.AddDbContextFactory<DatabaseContext>(options =>
        {
            options.UseSqlite(string.Format(connectionString, GetDbPath(), GetDbPassword()));
        });
    }

    private static string GetDbPassword()
    {
        return "Password12!";
    }

    private static string GetDbPath()
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        return System.IO.Path.Join(path, "timetrace.db");
    }
}
