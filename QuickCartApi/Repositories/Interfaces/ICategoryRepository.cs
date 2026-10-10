using QuickCartApi.Entities;

namespace QuickCartApi.Repositories.Interfaces
{
    public interface ICategoryRepository : IBaseRepository<Category>
    {
        Task<List<Category>> GetAllAsync();
        Task<bool> HasProductAsync(int categoryId);
    }
}
