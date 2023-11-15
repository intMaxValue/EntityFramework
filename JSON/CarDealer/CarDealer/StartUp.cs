using AutoMapper;
using CarDealer.Data;
using CarDealer.DTOs;
using CarDealer.Models;
using Castle.Core.Resource;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main()
        {
            var context = new Data.CarDealerContext();

            // 9.
            var suppliersJson = File.ReadAllText("../../../Datasets/suppliers.json");

            // 10.
            var partsJson = File.ReadAllText("../../../Datasets/parts.json");

            // 11.
            var carsJson = File.ReadAllText("../../../Datasets/cars.json");

            // 12.
            var customerJson = File.ReadAllText("../../../Datasets/customers.json");

            // 13.
            var salesJson = File.ReadAllText("../../../Datasets/sales.json");



            Console.WriteLine(GetSalesWithAppliedDiscount(context));
        }

         //9.ImportSuppliers
        public static string ImportSuppliers(CarDealerContext context, string inputJson)
        {
            var suppliers = JsonConvert.DeserializeObject<List<Supplier>>(inputJson);
            context.Suppliers.AddRange(suppliers);
            context.SaveChanges();
            return $"Successfully imported {suppliers.Count}.";
        }

        // 10.ImportParts
        public static string ImportParts(CarDealerContext context, string inputJson)
        {
            int[] suppliersIds = context.Suppliers.Select(x => x.Id).ToArray();

            var parts = JsonConvert
                .DeserializeObject<List<Part>>(inputJson)
                .Where(p => suppliersIds.Contains(p.SupplierId))
                .ToList();

            context.Parts.AddRange(parts);
            context.SaveChanges();
            return $"Successfully imported {parts.Count}.";
        }

        // 11.ImportCars
        public static string ImportCars(CarDealerContext context, string inputJson)
        {
            var carsAndPartsDTO = JsonConvert.DeserializeObject<List<CarDTO>>(inputJson);

            List<PartCar> parts = new List<PartCar>();
            List<Car> cars = new List<Car>();

            foreach (var dto in carsAndPartsDTO)
            {
                Car car = new Car()
                {
                    Make = dto.Make,
                    Model = dto.Model,
                    TraveledDistance = dto.TravelledDistance
                };
                cars.Add(car);

                foreach (var part in dto.PartsId.Distinct())
                {
                    PartCar partCar = new PartCar()
                    {
                        Car = car,
                        PartId = part,
                    };
                    parts.Add(partCar);
                }
            }

            context.Cars.AddRange(cars);
            context.PartsCars.AddRange(parts);
            context.SaveChanges();
            return $"Successfully imported {cars.Count}.";
        }

        // 12.ImportCustomers
        public static string ImportCustomers(CarDealerContext context, string inputJson)
        {
            var customers = JsonConvert.DeserializeObject<Customer[]>(inputJson);

            context.Customers.AddRange(customers);
            context.SaveChanges();

            return $"Successfully imported {customers.Count()}.";
        }

        // 13.ImportSales
        public static string ImportSales(CarDealerContext context, string inputJson)
        {
            var sales = JsonConvert.DeserializeObject<Sale[]>(inputJson);

            context.Sales.AddRange(sales);
            context.SaveChanges();

            return $"Successfully imported {sales.Count()}.";
        }

        // 14.ExportOrderedCustomers
        public static string GetOrderedCustomers(CarDealerContext context)
        {
            var customers = context.Customers
                .OrderBy(c => c.BirthDate)
                .ThenBy(c => c.IsYoungDriver == true)
                .Select(c => new customersDTO
                {
                    Name = c.Name,
                    BirthDate = c.BirthDate.ToString("dd/MM/yyyy"),
                    IsYoungDriver = c.IsYoungDriver
                })
                .ToList();

            var customersJson = JsonConvert.SerializeObject(customers, new JsonSerializerSettings()
            {
                //Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore,
                StringEscapeHandling = StringEscapeHandling.EscapeHtml
            });

            return customersJson;
        }

        // 15.ExportCarsFromMakeToyota
        public static string GetCarsFromMakeToyota(CarDealerContext context)
        {
            var carsToyota = context.Cars
                .Where(c => c.Make == "Toyota")
                .OrderBy(c => c.Model)
                .ThenByDescending(c => c.TraveledDistance)
                .Select(c => new carsToyotaDTO
                {
                    Id = c.Id,
                    Make = c.Make,
                    Model = c.Model,
                    TraveledDistance = c.TraveledDistance,

                })
                .ToList();

            var carsJson = JsonConvert.SerializeObject(carsToyota, new JsonSerializerSettings()
            {
                //Formatting = Formatting.Indented
            });

            return carsJson;
        }

        // 16.GetLocalSuppliers
        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var suppliers = context.Suppliers
                .Where(s => s.IsImporter == false)
                .Select(s => new
                {
                    Id = s.Id,
                    Name = s.Name,
                    PartsCount = s.Parts.Count
                })
                .ToList();

            return JsonConvert.SerializeObject(suppliers, Formatting.Indented);
        }

        // 17.ExportCarsWithTheirListOfParts
        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var cars = context.Cars
                .Where(c => c.PartsCars.Any())
                .Select(c => new
                {
                    car = new
                    {
                        Make = c.Make,
                        Model = c.Model,
                        TraveledDistance = c.TraveledDistance
                    },
                    parts = c.PartsCars
                        .Select(p => new
                        {
                            Name = p.Part.Name,
                            Price = $"{p.Part.Price:f2}",
                        })
                        .ToArray()

                })
                .ToArray();

            return JsonConvert.SerializeObject(cars, Formatting.Indented);
        }

        // 18.ExportTotalSalesByCustomer
        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            var customers = context.Customers
                .Where(c => c.Sales.Any())
                .Select(c => new
                {
                    fullName = c.Name,
                    boughtCars = c.Sales.Count,
                    salePrices = c.Sales.SelectMany(x => x.Car.PartsCars.Select(x => x.Part.Price))
                })
                .ToArray();

            var totalSalesByCustomer = customers.Select(t => new
                {
                    t.fullName,
                    t.boughtCars,
                    spentMoney = t.salePrices.Sum()
                })
                .OrderByDescending(t => t.spentMoney)
                .ThenByDescending(t => t.boughtCars)
                .ToArray();

            return JsonConvert.SerializeObject(totalSalesByCustomer, Formatting.Indented);

        }

        // 19.ExportSalesWithAppliedDiscount
        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            var sales = context.Sales
                .Take(10)
                .Select(s => new
                {
                    car = new
                    {
                        Make = s.Car.Make,
                        Model = s.Car.Model,
                        TraveledDistance = s.Car.TraveledDistance,
                    },
                    customerName = s.Customer.Name,
                    discount = $"{s.Discount:f2}",
                    price = $"{s.Car.PartsCars.Sum(p => p.Part.Price):f2}",
                    priceWithDiscount = $"{s.Car.PartsCars.Sum(p => p.Part.Price) * (1 - s.Discount / 100):f2}"
                })
                .ToArray();

            return JsonConvert.SerializeObject(sales, Formatting.Indented);
        }
    }
}