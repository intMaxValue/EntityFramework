// SKYNET\SQLEXPRESS
// ADONETMinionsDB


using ADONET;
using System.Data.SqlClient;

internal class Program
{
    const string _connectionString =
    @"Server=SKYNET\SQLEXPRESS;Database=ADONETMinionsDB;Integrated Security=True";
    private static void Main(string[] args)
    {
        PrintVillainsWithMinions();
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
}