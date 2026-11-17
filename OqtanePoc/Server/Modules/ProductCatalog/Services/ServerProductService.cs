using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Oqtane.Enums;
using Oqtane.Infrastructure;
using Oqtane.Models;
using Oqtane.Modules;
using Oqtane.Security;
using Oqtane.Shared;
using OqtanePoc.Models;
using OqtanePoc.Modules.ProductCatalog.Repository;
using OqtanePoc.Services;

namespace OqtanePoc.Modules.ProductCatalog.Services
{
    public class ServerProductService : IProductService, ITransientService
    {
        private readonly IProductRepository _productRepository;
        private readonly IUserPermissions _userPermissions;
        private readonly ILogManager _logger;
        private readonly IHttpContextAccessor _accessor;
        private readonly Alias _alias;

        public ServerProductService(IProductRepository productRepository, IUserPermissions userPermissions, ITenantManager tenantManager, ILogManager logger, IHttpContextAccessor accessor)
        {
            _productRepository = productRepository;
            _userPermissions = userPermissions;
            _logger = logger;
            _accessor = accessor;
            _alias = tenantManager.GetAlias();
        }

        public Task<List<ProductItem>> GetProductsAsync(int moduleId)
        {
            if (_userPermissions.IsAuthorized(_accessor.HttpContext.User, _alias.SiteId, EntityNames.Module, moduleId, PermissionNames.View))
            {
                return Task.FromResult(_productRepository.GetProducts(moduleId).ToList());
            }
            _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized Product Get Attempt {ModuleId}", moduleId);
            return Task.FromResult(new List<ProductItem>());
        }

        public Task<ProductItem> GetProductAsync(int productId, int moduleId)
        {
            if (_userPermissions.IsAuthorized(_accessor.HttpContext.User, _alias.SiteId, EntityNames.Module, moduleId, PermissionNames.View))
            {
                return Task.FromResult(_productRepository.GetProduct(productId));
            }
            return Task.FromResult<ProductItem>(null);
        }

        public Task<ProductItem> AddProductAsync(ProductItem product)
        {
            if (_userPermissions.IsAuthorized(_accessor.HttpContext.User, _alias.SiteId, EntityNames.Module, product.ModuleId, PermissionNames.Edit))
            {
                return Task.FromResult(_productRepository.AddProduct(product));
            }
            return Task.FromResult<ProductItem>(null);
        }

        public Task<ProductItem> UpdateProductAsync(ProductItem product)
        {
            if (_userPermissions.IsAuthorized(_accessor.HttpContext.User, _alias.SiteId, EntityNames.Module, product.ModuleId, PermissionNames.Edit))
            {
                return Task.FromResult(_productRepository.UpdateProduct(product));
            }
            return Task.FromResult<ProductItem>(null);
        }

        public Task DeleteProductAsync(int productId, int moduleId)
        {
            if (_userPermissions.IsAuthorized(_accessor.HttpContext.User, _alias.SiteId, EntityNames.Module, moduleId, PermissionNames.Edit))
            {
                _productRepository.DeleteProduct(productId);
            }
            return Task.CompletedTask;
        }
    }
}
