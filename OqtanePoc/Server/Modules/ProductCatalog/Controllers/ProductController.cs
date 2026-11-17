using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Controllers;
using Oqtane.Enums;
using Oqtane.Infrastructure;
using Oqtane.Shared;
using OqtanePoc.Models;
using OqtanePoc.Services;

namespace OqtanePoc.Modules.ProductCatalog.Controllers
{
    [Route(ControllerRoutes.ApiRoute)]
    public class ProductController : ModuleControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService, ILogManager logger, IHttpContextAccessor accessor) : base(logger, accessor)
        {
            _productService = productService;
        }

        [HttpGet]
        [Authorize(Policy = PolicyNames.ViewModule)]
        public async Task<IEnumerable<ProductItem>> Get(string moduleid)
        {
            if (int.TryParse(moduleid, out int moduleId) && IsAuthorizedEntityId(EntityNames.Module, moduleId))
            {
                return await _productService.GetProductsAsync(moduleId);
            }
            HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            return null;
        }

        [HttpGet("{id}/{moduleid}")]
        [Authorize(Policy = PolicyNames.ViewModule)]
        public async Task<ProductItem> Get(int id, int moduleid)
        {
            if (IsAuthorizedEntityId(EntityNames.Module, moduleid))
            {
                return await _productService.GetProductAsync(id, moduleid);
            }
            HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            return null;
        }

        [HttpPost]
        [Authorize(Policy = PolicyNames.EditModule)]
        public async Task<ProductItem> Post([FromBody] ProductItem product)
        {
            if (ModelState.IsValid && IsAuthorizedEntityId(EntityNames.Module, product.ModuleId))
            {
                return await _productService.AddProductAsync(product);
            }
            HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            return null;
        }

        [HttpPut("{id}")]
        [Authorize(Policy = PolicyNames.EditModule)]
        public async Task<ProductItem> Put(int id, [FromBody] ProductItem product)
        {
            if (ModelState.IsValid && product.ProductId == id && IsAuthorizedEntityId(EntityNames.Module, product.ModuleId))
            {
                return await _productService.UpdateProductAsync(product);
            }
            HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            return null;
        }

        [HttpDelete("{id}/{moduleid}")]
        [Authorize(Policy = PolicyNames.EditModule)]
        public async Task Delete(int id, int moduleid)
        {
            if (IsAuthorizedEntityId(EntityNames.Module, moduleid))
            {
                await _productService.DeleteProductAsync(id, moduleid);
            }
            else
            {
                HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            }
        }
    }
}
