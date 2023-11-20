using System.Text;
using System.Xml.Serialization;
using ProductShop.Data;
using ProductShop.DTOs.Export;
using ProductShop.DTOs.Import;
using ProductShop.Models;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main()
        {
            ProductShopContext context = new ProductShopContext();
            

            //1.
            var usersXML = File.ReadAllText("../../../Datasets/users.xml");
            //2.
            var productsXML = File.ReadAllText("../../../Datasets/products.xml");
            //3.
            var categoriesXML = File.ReadAllText("../../../Datasets/categories.xml");
            //4.
            var categoriesAndProductsXML = File.ReadAllText("../../../Datasets/categories-products.xml");


            Console.WriteLine(ImportProducts(context, productsXML));
        }

        //1. Import Users
        public static string ImportUsers(ProductShopContext context, string inputXml)
        {
            var usersDTO = Deserializer<ImportUsersDto[]>(inputXml, "Users");

            User[] users = usersDTO
                .Select(u => new User
                {
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Age = u.Age,
                })
                .ToArray();

            context.Users.AddRange(users);
            context.SaveChanges();

            return $"Successfully imported {users.Count()}";
        }

        //2. Import Products
        public static string ImportProducts(ProductShopContext context, string inputXml)
        {
            var productDtos = Deserializer<ImportProductsDto[]>(inputXml, "Products");

            Product[] products = productDtos
                .Select(p => new Product()
                {
                    Name = p.Name,
                    Price = p.Price,
                    BuyerId = p.BuyerId == 0 ? null : p.BuyerId,
                    SellerId = p.SellerId
                })
                .ToArray();

            context.Products.AddRange(products);
            context.SaveChanges();

            return $"Successfully imported {products.Count()}";
        }

        //3. Import Categories
        public static string ImportCategories(ProductShopContext context, string inputXml)
        {
            var categoriesXML = Deserializer<ImportCategoriesDto[]>(inputXml, "Categories");

            Category[] categories = categoriesXML
                .Where(c => c.Name != null)
                .Select(c => new Category()
                {
                    Name = c.Name,
                })
                .ToArray();

            context.Categories.AddRange(categories);
            context.SaveChanges();

            return $"Successfully imported {categories.Count()}";
        }

        //4. Import Categories and Products
        public static string ImportCategoryProducts(ProductShopContext context, string inputXml)
        {
            var cnpXML = Deserializer<ImportCategoriesAndProductsDto[]>(inputXml, "CategoryProducts");

            CategoryProduct[] cnp = cnpXML
                .Where(cnp => cnp.CategoryId != 0 && cnp.ProductId != 0)
                .Select(cnp => new CategoryProduct()
                {
                    CategoryId = cnp.CategoryId,
                    ProductId = cnp.ProductId,
                })
                .ToArray();

            context.CategoryProducts.AddRange(cnp);
            context.SaveChanges();

            return $"Successfully imported {cnp.Count()}";
        }

        //5. Export Products In Range
        public static string GetProductsInRange(ProductShopContext context)
        {
            var productsInRange = context.Products
                .Where(p => p.Price >= 500 && p.Price <= 1000)
                .OrderBy(p => p.Price)
                .Take(10)
                .Select(p => new ExportProductsInRangeDto()
                {
                    Price = p.Price,
                    Name = p.Name,
                    Buyer = p.Buyer.FirstName + " " + p.Buyer.LastName
                })
                .ToArray();

            return Serializer<ExportProductsInRangeDto[]>(productsInRange, "Products");
        }



        //SERIALIZER TO XML
        private static string Serializer<T>(T dataTransferObjects, string xmlRootAttributeName)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T), new XmlRootAttribute(xmlRootAttributeName));

            StringBuilder sb = new StringBuilder();
            using var write = new StringWriter(sb);

            XmlSerializerNamespaces xmlNamespaces = new XmlSerializerNamespaces();
            xmlNamespaces.Add(string.Empty, string.Empty);

            serializer.Serialize(write, dataTransferObjects, xmlNamespaces);

            return sb.ToString();
        }

        //DESERIALIZER TO XML
        private static T Deserializer<T>(string inputXml, string rootName)
        {
            XmlRootAttribute root = new XmlRootAttribute(rootName);
            XmlSerializer serializer = new XmlSerializer(typeof(T), root);

            using StringReader reader = new StringReader(inputXml);

            T dtos = (T)serializer.Deserialize(reader);
            return dtos;
        }
    }
}