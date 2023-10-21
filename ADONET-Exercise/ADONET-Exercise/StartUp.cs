// SKYNET\SQLEXPRESS
// ADONETMinionsDB


using ADONET;
using System.Data.SqlClient;
using System.Threading.Channels;

internal class Program
{
    const string _connectionString =
    @"Server=SKYNET\SQLEXPRESS;Database=ADONETMinionsDB;Integrated Security=True";
    private static async Task Main(string[] args)
    {
        //PrintVillainsWithMinions();

        int id = int.Parse(Console.ReadLine());
        await MinionNames(id);

    }

    public static void PrintVillainsWithMinions()
    {
        using (SqlConnection sqlConn = new SqlConnection(_connectionString))
        {
            sqlConn.Open();

            using (SqlCommand getVillainsCommand = new SqlCommand(SQLQueries.getVillainsWithNumberOfMinions, sqlConn))
            using (SqlDataReader sqlDataReader = getVillainsCommand.ExecuteReader())
            {
                while (sqlDataReader.Read())
                {
                    Console.WriteLine($"{sqlDataReader["Name"]} - {sqlDataReader["MinionsCount"]}");
                }
            }
        }
    }


    public async static Task MinionNames(int id)
    {
        using (SqlConnection sqlConn = new SqlConnection(_connectionString))
        {
            sqlConn.Open();

            using SqlCommand minionsNamesCommand = new SqlCommand(SQLQueries.minionsWithName, sqlConn);

            minionsNamesCommand.Parameters.AddWithValue("@Id", id);

            var result = await minionsNamesCommand.ExecuteScalarAsync();

            if (result is null)
            {
                await Console.Out.WriteLineAsync($"No villain with id {id} exist in the database.");
            }
            else
            {
                await Console.Out.WriteLineAsync($"Villain: {result}");

                using SqlCommand minionsNamesSecondCommand = new SqlCommand(SQLQueries.minionsWithNameSecond, sqlConn);

                minionsNamesSecondCommand.Parameters.AddWithValue("@Id", id);

                var minionsReader = await minionsNamesSecondCommand.ExecuteReaderAsync();

                while (await minionsReader.ReadAsync())
                {
                    await Console.Out.WriteLineAsync($"{minionsReader["RowNum"]}. " + 
                    $"{minionsReader["Name"]} {minionsReader["Age"]}");
                }
            }

            sqlConn.Close();
        }
    }
}