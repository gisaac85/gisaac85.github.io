using Core.Entities.ProductModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IProductRepository
    {
        Task<Product> GetProductByIdAsync(int id);
        Task<List<Product>> GetProductByNameAsync(string name);
        Task<List<Product>> GetProductsAsync();
        Task<IReadOnlyList<ProductBrand>> GetProductBrandsAsync();
        Task<IReadOnlyList<Product>> GetProductsByBrandAsync(int id);
        Task<IReadOnlyList<Product>> GetProductsByTypeAsync(int id);
        Task<IReadOnlyList<ProductType>> GetProductTypesAsync();
        Task<Product> CreateProduct(Product model);
        Task<IReadOnlyList<Product>> SortProductByFilter(int? filter);
        Task<Product> EditProduct(Product model);
        Task<Product> DeleteProduct(int id);

    }
}

