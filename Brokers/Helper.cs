using Microsoft.Extensions.Configuration;
using Npgsql;

namespace test_bot.Brokers;

internal static class Helper
{
    public static NpgsqlConnection GetConnection()
    {

        IConfigurationRoot configBuilder = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

        string connectionString = configBuilder.GetSection("ConnectionStrings").GetSection("NpgsqlConnection").Value;
        NpgsqlConnection connection = new NpgsqlConnection(connectionString);

        return  connection;
    }

}
