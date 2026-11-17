using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Controllers;
using Oqtane.Infrastructure;
using Oqtane.Models;
using Oqtane.Shared;
using OqtanePoc.Filters;
using OqtanePoc.Models;
using OqtanePoc.Modules.ProductCatalog.Repository;

namespace OqtanePoc.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [TenantAuthorize("Master", "Tenant1")] // Restricted to Primary Tenant
    public class TenantRestrictedApiController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ITenantManager _tenantManager;

        public TenantRestrictedApiController(IProductRepository productRepository, ITenantManager tenantManager)
        {
            _productRepository = productRepository;
            _tenantManager = tenantManager;
        }

        [HttpGet("status")]
        public IActionResult GetTenantStatus()
        {
            Tenant tenant = _tenantManager.GetTenant();
            Alias alias = _tenantManager.GetAlias();

            return Ok(new
            {
                TenantId = tenant.TenantId,
                TenantName = tenant.Name,
                AliasName = alias.Name,
                Message = "Access Granted: You are successfully interacting with the isolated Primary Tenant data service.",
                ServerTimestampUtc = DateTime.UtcNow
            });
        }

        [HttpGet("catalog")]
        public ActionResult<IEnumerable<ProductItem>> GetIsolatedProducts()
        {
            // Note: Since _productRepository connects through Oqtane's tenant-aware DBContextFactory,
            // this query physically queries ONLY the primary tenant's isolated database.
            var products = _productRepository.GetProducts(-1);
            return Ok(products);
        }
    }
}
