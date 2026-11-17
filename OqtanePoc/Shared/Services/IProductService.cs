using System.Collections.Generic;
using System.Threading.Tasks;
using OqtanePoc.Models;

namespace OqtanePoc.Services
{
    public interface IProductService
    {
        Task<List<ProductItem>> GetProductsAsync(int moduleId);
        Task<ProductItem> GetProductAsync(int productId, int moduleId);
        Task<ProductItem> AddProductAsync(ProductItem product);
        Task<ProductItem> UpdateProductAsync(ProductItem product);
        Task DeleteProductAsync(int productId, int moduleId);
    }
}
