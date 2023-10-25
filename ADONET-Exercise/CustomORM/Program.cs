

using CustomORM;
using CustomORM.Minions;
using Microsoft.EntityFrameworkCore;

ApplicationDBContext db = new ApplicationDBContext();

var towns = db.Towns.Include(c => c.Country);

Countries country = new Countries
{
    Name = "USA"

};

db.Countries.Add(country);



Towns newTown = new Towns
{
    Name = "New New York"
};

var townToRemove = db.Towns.FirstOrDefault(t => t.Name == "New New York");
if (townToRemove != null)
{
    db.Towns.Remove(townToRemove);
    db.SaveChanges();
}

Console.WriteLine(towns.ToString());


