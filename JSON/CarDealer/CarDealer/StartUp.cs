using CarDealer.Data;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main()
        {
            var context = new Data.CarDealerContext();

            var suppliers = File.ReadAllText("../../../Datasets/suppliers.json");


        }

        // 10.ImportParts
        public static string ImportParts(CarDealerContext context, string inputJson)
        {
            return $"Successfully imported .";
        }

    }
}