using EFIntroduction.Models;
using Microsoft.EntityFrameworkCore;
using System;

using SoftUniContext context = new SoftUniContext();

//var employee = await context.Employees
//    .Include(e => e.Department)
//    .Include(e => e.Manager)
//    .Include(e => e.Projects)
//    .FirstOrDefaultAsync(e => e.EmployeeId == 147);

//Console.WriteLine($"Name: {employee.FirstName}");

//string query = context.Employees
//    .Include(e => e.Department)
//    .Include(e => e.Manager)
//    .Include(e => e.Projects)
//    .Where(e => e.EmployeeId == 147)
//    .ToQueryString();

//await Console.Out.WriteLineAsync(query);

//var employeeTest =  context.Employees
//    .Include(e => e.Department)
//    .Include(e => e.Manager)
//    .Include(e => e.Projects)
//    .Select(e => new
//    {
//        Id = e.EmployeeId,
//        Name = e.FirstName + " " + e.LastName,
//        Department = e.Department.Name,
//        Manager = e.Manager.FirstName,
//        Projects = e.Projects.Select(p => new
//        {
//            p.Name,
//            p.StartDate,
//            p.EndDate
//        })
//    })
//    .FirstOrDefault(e => e.Id == 147);

//await Console.Out.WriteLineAsync();


await context.Projects.AddAsync(new Project
{
    Name = "Judge System",
    StartDate = DateTime.Now,
    Description = "Example system",
});

await context.SaveChangesAsync();
