using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Oqtane.Modules;
using OqtanePoc.Models;

namespace OqtanePoc.Modules.ProductCatalog.Repository
{
    public interface IProductRepository
    {
        IEnumerable<ProductItem> GetProducts(int moduleId);
        ProductItem GetProduct(int productId);
        ProductItem AddProduct(ProductItem product);
        ProductItem UpdateProduct(ProductItem product);
        void DeleteProduct(int productId);
    }

    public class ProductRepository : IProductRepository, ITransientService
    {
        private readonly IDbContextFactory<ProductContext> _factory;

        public ProductRepository(IDbContextFactory<ProductContext> factory)
        {
            _factory = factory;
        }

        public IEnumerable<ProductItem> GetProducts(int moduleId)
        {
            using var db = _factory.CreateDbContext();
            return db.Products.Where(item => item.ModuleId == moduleId).AsNoTracking().ToList();
        }

        public ProductItem GetProduct(int productId)
        {
            using var db = _factory.CreateDbContext();
            return db.Products.Find(productId);
        }

        public ProductItem AddProduct(ProductItem product)
        {
            using var db = _factory.CreateDbContext();
            db.Products.Add(product);
            db.SaveChanges();
            return product;
        }

        public ProductItem UpdateProduct(ProductItem product)
        {
            using var db = _factory.CreateDbContext();
            db.Entry(product).State = EntityState.Modified;
            db.SaveChanges();
            return product;
        }

        public void DeleteProduct(int productId)
        {
            using var db = _factory.CreateDbContext();
            var product = db.Products.Find(productId);
            if (product != null)
            {
                db.Products.Remove(product);
                db.SaveChanges();
            }
        }
    }
}
