
namespace EventMe.Infrastructure.Data.Contracts
{
    /// <summary>
    /// Entity което може да бъде изтрито
    /// </summary>
    public interface IDeletable
    {
        /// <summary>
        /// Записът е активен
        /// </summary>
        bool IsActive { get; set; }

        /// <summary>
        /// Изтрит на
        /// </summary>
        DateTime? DeletedOn { get; set; }
    }
}
