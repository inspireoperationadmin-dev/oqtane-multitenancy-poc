using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Oqtane.Modules;
using Oqtane.Services;
using Oqtane.Shared;
using OqtanePoc.Models;

namespace OqtanePoc.Services
{
    public class ClientProductService : ServiceBase, IProductService, ITransientService
    {
        public ClientProductService(HttpClient http, SiteState siteState) : base(http, siteState) { }

        private string Apiurl => CreateApiUrl("Product");

        public async Task<List<ProductItem>> GetProductsAsync(int moduleId)
        {
            List<ProductItem> products = await GetJsonAsync<List<ProductItem>>(CreateAuthorizationPolicyUrl($"{Apiurl}?moduleid={moduleId}", EntityNames.Module, moduleId), Enumerable.Empty<ProductItem>().ToList());
            return products;
        }

        public async Task<ProductItem> GetProductAsync(int productId, int moduleId)
        {
            return await GetJsonAsync<ProductItem>(CreateAuthorizationPolicyUrl($"{Apiurl}/{productId}/{moduleId}", EntityNames.Module, moduleId));
        }

        public async Task<ProductItem> AddProductAsync(ProductItem product)
        {
            return await PostJsonAsync<ProductItem>(CreateAuthorizationPolicyUrl($"{Apiurl}", EntityNames.Module, product.ModuleId), product);
        }

        public async Task<ProductItem> UpdateProductAsync(ProductItem product)
        {
            return await PutJsonAsync<ProductItem>(CreateAuthorizationPolicyUrl($"{Apiurl}/{product.ProductId}", EntityNames.Module, product.ModuleId), product);
        }

        public async Task DeleteProductAsync(int productId, int moduleId)
        {
            await DeleteAsync(CreateAuthorizationPolicyUrl($"{Apiurl}/{productId}/{moduleId}", EntityNames.Module, moduleId));
        }
    }
}
