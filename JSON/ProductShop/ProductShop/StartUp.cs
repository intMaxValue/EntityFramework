using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ProductShop.Data;
using ProductShop.Models;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main()
        {
            ProductShopContext context = new ProductShopContext();

            // 1.ImportUsers
            string userJson = File.ReadAllText("../../../Datasets/users.json");

            // 2.ImportProducts
            string productJson = File.ReadAllText("../../../Datasets/products.json");

            // 3.ImportCategories
            string categoryJson = File.ReadAllText("../../../Datasets/categories.json");

            // 4.ImportCategoryProduct
            string categoryProductJson = File.ReadAllText("../../../Datasets/categories-products.json");




            Console.WriteLine(GetUsersWithProducts(context));
        }

        // 1.ImportUsers
        public static string ImportUsers(ProductShopContext context, string inputJson)
        {
            var users = JsonConvert.DeserializeObject<User[]>(inputJson);

            context.Users.AddRange(users);

            context.SaveChanges();

            return $"Successfully imported {users.Length}";
        }

        // 2.ImportProducts
        public static string ImportProducts(ProductShopContext context, string inputJson)
        {
            var products = JsonConvert.DeserializeObject<Product[]>(inputJson);

            context.Products.AddRange(products);

            context.SaveChanges();

            return $"Successfully imported {products.Length}";
        }

        // 3.ImportCategories
        public static string ImportCategories(ProductShopContext context, string inputJson)
        {
            var categories = JsonConvert.DeserializeObject<Category[]>(inputJson);
            var count = 0;

            foreach (var c in categories.Where(c => c.Name != null))
            {
                context.Categories.Add(c);
                count++;
            }

            context.SaveChanges();

            return $"Successfully imported {count}";
        }

        // 4.ImportCategoryProducts
        public static string ImportCategoryProducts(ProductShopContext context, string inputJson)
        {
            var cp = JsonConvert.DeserializeObject<CategoryProduct[]>(inputJson);

            context.CategoriesProducts.AddRange(cp);

            context.SaveChanges();

            return $"Successfully imported {cp.Length}";
        }

        // 5.ExportProductsInRange
        public static string GetProductsInRange(ProductShopContext context)
        {
            var products = context.Products
                .Where(p => p.Price >= 500 && p.Price <= 1000)
                .Select(p => new
                {
                    name = p.Name,
                    price = p.Price,
                    seller = $"{p.Seller.FirstName} {p.Seller.LastName}"
                })
                .OrderBy(p => p.price)
                .ToList();

            var json = JsonConvert.SerializeObject(products, Formatting.Indented);

            return json;
        }

        // 6.ExportSoldProducts
        public static string GetSoldProducts(ProductShopContext context)
        {
            var users = context.Users
                .Where(u => u.ProductsSold.Any(p => p.BuyerId != null))
                .OrderBy(u => u.LastName)
                    .ThenBy(u => u.FirstName)
                .Select(u => new
                {
                    firstName = u.FirstName,
                    lastName = u.LastName,
                    soldProducts = u.ProductsSold
                        .Select(p => new
                        {
                            name = p.Name,
                            price = p.Price,
                            buyerFirstName = p.Buyer.FirstName,
                            buyerLastName = p.Buyer.LastName
                        }).ToArray()
                })
                .ToArray();

            var json = JsonConvert.SerializeObject(users, Formatting.Indented);

            return json;
        }

        // 7.ExportCategoriesByProductsCount
        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            var categories = context.Categories
                .Select(c => new
                {
                    category = c.Name,
                    productsCount = c.CategoriesProducts.Count(),
                    averagePrice = c.CategoriesProducts.Average(cp => cp.Product.Price).ToString("f2"),
                    totalRevenue = c.CategoriesProducts.Sum(cp => cp.Product.Price).ToString("f2")
                })
                .OrderByDescending(c => c.productsCount)
                .ToArray();
                
            return JsonConvert.SerializeObject(categories, Formatting.Indented);
        }

        // 8.ExportUsersAndProducts
        public static string GetUsersWithProducts(ProductShopContext context)
        {
            var users = context.Users
                .Where(u => u.ProductsSold.Any(p => p.Buyer != null))
                .Select(u => new
                {
                    firstName = u.FirstName,
                    lastName = u.LastName,
                    age = u.Age,
                    soldProducts = u.ProductsSold
                        .Where(p => p.Buyer != null)
                        .Select(p => new
                        {
                            name = p.Name,
                            price = p.Price,
                        })
                        .ToArray()
                })
                .OrderByDescending(u => u.soldProducts.Count())
                .ToArray();

            var output = new
            {
                usersCount = users.Count(),
                users = users.Select(u => new
                {
                    u.firstName,
                    u.lastName,
                    u.age,
                    soldProducts = new
                    {
                        count = u.soldProducts.Count(),
                        products = u.soldProducts
                    }
                })
            };

            string json = JsonConvert.SerializeObject (output, new JsonSerializerSettings
            {
                //Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore,
                StringEscapeHandling = StringEscapeHandling.EscapeHtml
            });

            return json;
        }
    }
}