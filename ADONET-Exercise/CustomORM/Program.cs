

using CustomORM;
using Microsoft.EntityFrameworkCore;

ApplicationDBContext db = new ApplicationDBContext();

var towns = db.Towns.Include(c => c.Country);


    Console.WriteLine(towns.ToQueryString());


