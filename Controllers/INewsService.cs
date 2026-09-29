using ContactApp.Contracts.DTOs;

namespace ContactApp.Controllers
{
    public interface INewsService
    {
        Task<IReadOnlyList<NewsArticleDto>> GetAllAsync(CancellationToken cancellationToken);
        Task<NewsArticleDto?> GetByIdAsync(int id, CancellationToken cancellationToken);





    }
}
