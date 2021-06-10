using Core.Entities.ProductModels;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Data.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly WebshopDataContext _context;
        public ProductRepository(WebshopDataContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<ProductBrand>> GetProductBrandsAsync()
        {
            return await _context.ProductBrands.ToListAsync();
        }

        public async Task<IReadOnlyList<Product>> GetProductsByBrandAsync(int id)
        {
            return await _context.Products.Where(x=>x.ProductBrandId==id)
                .Include(x => x.ProductBrand)
                .Include(x => x.ProductType)
                .Select(t => new Product(
                  t.Id,t.Name,t.Description,t.Price,t.PictureUrl,t.ProductTypeId,t.ProductBrandId, t.ProductBrand, t.ProductType))
                .ToListAsync();
        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            return await _context.Products.Where(x => x.Id == id)
                .Include(x => x.ProductBrand)
                .Include(x => x.ProductType)
                .Select(x => new Product(
                 x.Id, x.Name, x.Description, x.Price, x.PictureUrl, x.ProductTypeId, x.ProductBrandId, x.ProductBrand, x.ProductType))
                .FirstOrDefaultAsync();
        }

        public async Task<List<Product>> GetProductByNameAsync(string name)
        {
            return await _context.Products.Where(x => x.Name.ToLower() == name || x.Name.ToUpper() == name || x.Name == name).Include(x => x.ProductBrand).Include(x => x.ProductType).
                Select(t => new Product
                     (
                       t.Id, t.Name, t.Description, t.Price, t.PictureUrl, t.ProductTypeId, t.ProductBrandId,t.ProductBrand,t.ProductType
                     )
                )
                .ToListAsync();
        }

        public async Task<IReadOnlyList<Product>> GetProductsByTypeAsync(int id)
        {
            return await _context.Products.Where(x => x.ProductTypeId == id)
                .Include(x => x.ProductBrand)
                .Include(x => x.ProductType)
                .Select(t => new Product(
                      t.Id, t.Name, t.Description, t.Price, t.PictureUrl, t.ProductTypeId, t.ProductBrandId, t.ProductBrand, t.ProductType))
                .ToListAsync();
        }

        public async Task<List<Product>> GetProductsAsync()
        {
            return await _context.Products.Include(p=>p.ProductBrand).Include(p=>p.ProductType).ToListAsync();
        }

        public async Task<IReadOnlyList<ProductType>> GetProductTypesAsync()
        {
            return await _context.ProductTypes.ToListAsync();
        }

        public async Task<Product> CreateProduct(Product model)
        {
            _context.Products.Add(model);
            await _context.SaveChangesAsync();
            return model;
        }

        public async Task<Product> EditProduct(Product model)
        {
            _context.Products.Update(model);
            await _context.SaveChangesAsync();
            return model;
        }

        public async Task<IReadOnlyList<Product>> SortProductByFilter(int? filter)
        {
            List<Product> products = new List<Product>();
            if(filter!=null)
            {
                switch(filter)
                {
                    case 0:
                        products= await _context.Products.OrderBy(p => p.Price).Include(p => p.ProductBrand).Include(p => p.ProductType).ToListAsync();
                        break;
                    case 1:
                        products = await _context.Products.OrderByDescending(p => p.Price).Include(p => p.ProductBrand).Include(p => p.ProductType).ToListAsync();
                        break;                  
                    default:
                        products = await _context.Products.OrderBy(p => p.Name).Include(p => p.ProductBrand).Include(p => p.ProductType).ToListAsync();
                        break;
                }
            }
            return products;           
        }

        public async Task<Product> DeleteProduct(int id)
        {
            var product = _context.Products.FindAsync(id).Result;
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return product;
        }
    }
}
