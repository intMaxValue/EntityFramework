namespace BookShop;

using BookShop.Models.Enums;
using Data;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text;

public class StartUp
{
    public static void Main()
    {
        using var db = new BookShopContext();
        //DbInitializer.ResetDatabase(db);

        //var input = int.Parse(Console.ReadLine());

        //Console.WriteLine(GetMostRecentBooks(db));
    }

    // 02. AgeRestriction
    public static string GetBooksByAgeRestriction(BookShopContext context, string command)
    {
        if (!Enum.TryParse<AgeRestriction>(command, true, out var ageRestriction))
        {
            return $"{command} is not a valid age restriction";
        }

        var books = context.Books
            .Where(b => b.AgeRestriction == ageRestriction)
            .Select(b => new
            {
                b.Title
            })
            .OrderBy(b => b.Title)
            .ToList();


        return string.Join(Environment.NewLine, books.Select(b => b.Title));
    }

    // 03. GoldenBooks
    public static string GetGoldenBooks(BookShopContext context) 
    {
        var books = context.Books
            .Where(b => b.EditionType == EditionType.Gold)
            .Where(b => b.Copies < 5000)
            .OrderBy(b => b.BookId)
            .Select(b => new
            {
                b.Title
            })
            .ToList();

        return String.Join(Environment.NewLine, books.Select(b => b.Title));
    }

    // 04. BooksByPrice
    public static string GetBooksByPrice(BookShopContext context)
    {
        var books = context.Books
            .Where(b => b.Price > 40)
            .Select(b => new
            {
                b.Title,
                b.Price
            })
            .OrderByDescending(b => b.Price)
            .ToList();

        var sb = new StringBuilder();

        foreach (var item in books)
        {
            sb.AppendLine($"{item.Title} - ${item.Price:f2}");
        }

        return sb.ToString().TrimEnd();
    }

    // 05. NotReleasedIn
    public static string GetBooksNotReleasedIn(BookShopContext context, int year)
    {
        var books = context.Books
            .Where(b => b.ReleaseDate.HasValue && b.ReleaseDate.Value.Year != year)
            .ToList();

        return string.Join(Environment.NewLine, books.Select(b => b.Title));
    }

    // 06. BookTitlesByCategory
    public static string GetBooksByCategory(BookShopContext context, string input)
    {
        string[] categories = input.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                                        .Select(c => c.ToLower())
                                        .ToArray();

        var books = context.Books
            .Select(b => new { b.Title, b.BookCategories })
            .Where(b => b.BookCategories.Any(bc =>
                categories.Contains(bc.Category.Name.ToLower())))
            .OrderBy(b => b.Title)
            .ToList();

        return string.Join(Environment.NewLine, books.Select(b => b.Title));
    }

    // 07. ReleaseBeforeDate
    public static string GetBooksReleasedBefore(BookShopContext context, string date)
    {
        DateTime targetDate = DateTime.ParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture);

        var books = context.Books
            .Where(b => b.ReleaseDate < targetDate)
            .OrderByDescending(b => b.ReleaseDate)
            .Select(b => $"{b.Title} - {b.EditionType} - ${b.Price:F2}")
            .ToList();

        return string.Join(Environment.NewLine, books);
    }

    // 08. AuthorSearch
    public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
    {
        var authors = context.Authors
            .Where(a => a.FirstName.EndsWith(input))
            .Select(a => new { FullName = a.FirstName + " " + a.LastName })
            .OrderBy(a => a.FullName)
            .ToList();

        return string.Join(Environment.NewLine, authors.Select(a => a.FullName));
    }

    // 09. BookSearch
    public static string GetBookTitlesContaining(BookShopContext context, string input)
    {
        var books = context.Books
            .Where(b => b.Title.ToLower().Contains(input.ToLower()))
            .OrderBy(b => b.Title)
            .ToList();

        return string.Join(Environment.NewLine, books.Select(b => b.Title));
    }

    // 10. BookSearchByAuthor
    public static string GetBooksByAuthor(BookShopContext context, string input)
    {
        var books = context.Books.
            Where(b => b.Author.LastName.ToLower().StartsWith(input.ToLower()))
            .Select(b => new
            {
                b.BookId,
                b.Title,
                Author = b.Author.FirstName + " " + b.Author.LastName,
            })
            .OrderBy(b => b.BookId)
            .ToList();

        var sb = new StringBuilder();

        foreach (var b in books)
        {
            sb.AppendLine($"{b.Title} ({b.Author})");
        }

        return sb.ToString().Trim();
    }

    // 11. CountBooks
    public static int CountBooks(BookShopContext context, int lengthCheck)
    {
        var books = context.Books
            .Where(b => b.Title.Length > lengthCheck)
            .Count();

        return books;
    }

    // 12. TotalBookCopies
    public static string CountCopiesByAuthor(BookShopContext context)
    {
        var authors = context.Authors
            .Select(a => new
            {
                Author = a.FirstName + " " + a.LastName,
                TotalBooks = a.Books.Sum(b => b.Copies)
            })
            .OrderByDescending(a => a.TotalBooks)
            .ToList();

        return string.Join(Environment.NewLine, authors.Select(a => $"{a.Author} - {a.TotalBooks}"));
    }

    // 13. ProfitByCategory
    public static string GetTotalProfitByCategory(BookShopContext context) 
    {
        var profit = context.Categories
            .Select(c => new
            {
                CategoryName = c.Name,
                TotalProfit = c.CategoryBooks.Sum(c => c.Book.Price * c.Book.Copies)
            })
            .OrderByDescending(a => a.TotalProfit)
            .ThenBy(c => c.CategoryName)
            .ToList();

        return string.Join(Environment.NewLine, profit.Select(p => $"{p.CategoryName} ${p.TotalProfit:f2}"));
    }

    // 14. MostRecentBooks
    public static string GetMostRecentBooks(BookShopContext context)
    {
        var books = context.Categories
            .Select(c => new
            {
                CategoryName = c.Name,
                Books = c.CategoryBooks.OrderByDescending(c => c.Book.ReleaseDate).Take(3)
                        .Select(cb => new
                        {
                            Title = cb.Book.Title,
                            cb.Book.ReleaseDate.Value.Year,
                        })
            })
            .OrderBy(c => c.CategoryName);

        var sb = new StringBuilder();

        foreach (var b in books)
        {
            sb.AppendLine($"--{b.CategoryName}");

            foreach (var item in b.Books)
            {
                sb.AppendLine($"{item.Title} ({item.Year})");
            }
        }

        return sb.ToString().Trim();
    }

    // 15. IncreasePrice
    public static void IncreasePrices(BookShopContext context)
    {
        var books = context.Books
            .Where(b => b.ReleaseDate.Value.Year < 2010)
            .ToList();

        foreach (var b in books)
        {
            b.Price += 5;
        }

        context.SaveChanges();
    }

    // 16. RemoveBooks
    public static int RemoveBooks(BookShopContext context)
    {
        var books = context.Books
            .Where(b => b.Copies < 4200);

        var count = books.Count();

        context.RemoveRange(books);
        context.SaveChanges();

        return count;
    }
}


