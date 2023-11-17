using System.IO;
using System.Xml.Serialization;
using AutoMapper;
using CarDealer.Data;
using CarDealer.DTOs.Import;
using CarDealer.Models;
using Castle.Core.Resource;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main()
        {
            CarDealerContext context = new CarDealerContext();

            //9.
            string suppliersXML = File.ReadAllText("../../../Datasets/suppliers.xml");

            //10.
            string partsXML = File.ReadAllText("../../../Datasets/parts.xml");

            //11.
            string carsXML = File.ReadAllText("../../../Datasets/cars.xml");

            //12.
            string customersXML = File.ReadAllText("../../../Datasets/customers.xml");

            //13.
            string salesXML = File.ReadAllText("../../../Datasets/sales.xml");


            Console.WriteLine(ImportSales(context, salesXML));


        }

        private static Mapper GetMapper()
        {
            var cfg = new MapperConfiguration(c => c.AddProfile<CarDealerProfile>());
            return new Mapper(cfg);
        }

        public static string ImportSuppliers(CarDealerContext context, string inputXml)
        {
            // Create Serializer
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(supplilerDto[]), new XmlRootAttribute("Suppliers"));

            // Create StringReader
            using var reader = new StringReader(inputXml);

            supplilerDto[] importSupplilerDtos = (supplilerDto[])xmlSerializer.Deserialize(reader);

            var mapper = GetMapper();
            Supplier[] suppliers = mapper.Map<Supplier[]>(importSupplilerDtos);

            context.AddRange(suppliers); //?
            context.SaveChanges();

            return $"Successfully imported {suppliers.Length}";
        }

        public static string ImportParts(CarDealerContext context, string inputXml)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(partsDto[]), new XmlRootAttribute("Parts"));

            using var reader = new StringReader(inputXml);

            partsDto[] importPartsDtos = (partsDto[])xmlSerializer.Deserialize(reader);

            var supplierIds = context.Suppliers.Select(p => p.Id).ToArray();

            var mapper = GetMapper();

            Part[] parts = mapper.Map<Part[]>(importPartsDtos.Where(p => supplierIds.Contains(p.SupplierId)));

            context.AddRange(parts);
            context.SaveChanges();

            return $"Successfully imported {parts.Length}";
        }

        public static string ImportCars(CarDealerContext context, string inputXml)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(carsDto[]), new XmlRootAttribute("Cars"));

            using StringReader reader = new StringReader(inputXml);

            carsDto[] carDtos = (carsDto[])serializer.Deserialize(reader);

            var mapper = GetMapper();

            List<Car> cars = new List<Car>();

            foreach (var carDto in carDtos)
            {
                Car car = mapper.Map<Car>(carDto);

                int[] carPartIds = carDto.PartsIds
                    .Select(p => p.Id)
                    .Distinct()
                    .ToArray();

                var carParts = new List<PartCar>();

                foreach (var id in carPartIds)
                {
                    carParts.Add(new PartCar
                    {
                        Car = car,
                        PartId = id
                    });
                }

                car.PartsCars = carParts;
                cars.Add(car);
            }

            context.AddRange(cars);
            context.SaveChanges();

            return $"Successfully imported {cars.Count}";
        }

        public static string ImportCustomers(CarDealerContext context, string inputXml)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(customersDto[]), new XmlRootAttribute("Customers"));

            using StringReader reader = new StringReader(inputXml);

            customersDto[] customersDtos = (customersDto[])serializer.Deserialize(reader);

            var mapper = GetMapper();

            Customer[] customers = mapper.Map<Customer[]>(customersDtos);

            context.AddRange(customers);
            context.SaveChanges();

            return $"Successfully imported {customers.Length}";
        }

        public static string ImportSales(CarDealerContext context, string inputXml)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(importSalesDto[]), new XmlRootAttribute("Sales"));

            StringReader reader = new StringReader(inputXml);

            importSalesDto[] importSalesDtos = (importSalesDto[])serializer.Deserialize(reader);

            var mapper = GetMapper();

            int[] carIds = context.Cars.Select(x => x.Id).ToArray();

            Sale[] sales = mapper.Map<Sale[]>(importSalesDtos)
                .Where(s => carIds.Contains(s.CarId))
                .ToArray();

            context.AddRange(sales);
            context.SaveChanges();

            return $"Successfully imported {sales.Length}";
        }
    }
}