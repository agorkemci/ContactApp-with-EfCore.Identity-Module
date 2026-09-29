using ContactApp.Contracts.DTOs;

namespace ContactApp.Controllers
{
    public class NewsService:INewsService
    {
        public HttpClient _httpClient { get; set; }

        public ILogger<NewsService> _logger { get; set; }
        public NewsService(HttpClient _httpClient, ILogger<NewsService> _logger)
        {
            this._httpClient = _httpClient;
            this._logger = _logger;
        }
        public async Task<IReadOnlyList<NewsArticleDto>> GetAllAsync(CancellationToken cancellationToken)
        {
            try
            {
                var list = await _httpClient.GetFromJsonAsync<List<NewsArticleDto>>("api/NewsArticles", cancellationToken);
                //şurdaki api route kullanarak new article dto nesnelerini tutan bir listeyi json formatın al
                return list ?? new List<NewsArticleDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Request from api/NewArticles is failed!");
                return Array.Empty<NewsArticleDto>();

            }
        }

        public async Task<NewsArticleDto?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            try {
                var response = await _httpClient.GetAsync($"api/NewsArticles/{id}", cancellationToken);
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)//404
                {
                    return null;
                }
                response.EnsureSuccessStatusCode();//200
                return await response.Content.ReadFromJsonAsync<NewsArticleDto>(cancellationToken);
            }
            catch(Exception ex) {
                _logger.LogError(ex, $"related ID from json could not read:{id}");
                return null;
            }
        }
    }
}
