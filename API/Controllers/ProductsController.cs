using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Interfaces;
using Core.Entities.ProductModels;
using Core.Dtos;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using API.Errors;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : BaseApiController
    {
        private readonly IProductRepository _productsRepo;
        private readonly IMapper _mapper;

        public ProductsController(IProductRepository productsRepo,IMapper mapper)
        {        
            _productsRepo = productsRepo;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("getallproducts")]
        public async Task<ActionResult<List<ProductToReturnDto>>> GetProducts()
        {
            var products = await _productsRepo.GetProductsAsync();
            return Ok(_mapper.Map<List<Product>, List<ProductToReturnDto>>(products));
        }

        [HttpGet]
        [Route("getproduct/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductToReturnDto>> GetProduct(int id)
        {
            var product = await _productsRepo.GetProductByIdAsync(id);
            return Ok(_mapper.Map<Product, ProductToReturnDto>(product));
        }

        [HttpGet]
        [Route("getproductbyname/{name}")]
        public async Task<ActionResult<Product>> GetProductByName(string name)
        {
            var products = await _productsRepo.GetProductByNameAsync(name);
            return Ok(_mapper.Map<List<Product>, List<ProductToReturnDto>>(products));

        }

        [HttpGet]
        [Route("getbrands")]
        public async Task<ActionResult<List<ProductBrand>>> GetProductBrandsAsync()
        {
            return Ok(await _productsRepo.GetProductBrandsAsync());
        }

        [HttpGet]
        [Route("gettypes")]
        public async Task<ActionResult<List<ProductType>>> GetProductTypesAsync()
        {
            return Ok(await _productsRepo.GetProductTypesAsync());
        }

        [HttpGet]
        [Route("getproductsbybrand/{brandId}")]
        public async Task<ActionResult<Product>> GetProductsByBrandAsync(int brandId)
        {
            var products =  await _productsRepo.GetProductsByBrandAsync(brandId);
            return Ok(_mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductToReturnDto>>(products));
        }

        [HttpGet]
        [Route("getproductsbytype/{typeId}")]
        public async Task<ActionResult<Product>> GetProductsByTypeAsync(int typeId)
        {
            var products = await _productsRepo.GetProductsByTypeAsync(typeId);
            return Ok(_mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductToReturnDto>>(products));
        }

        [HttpPost]
        [Route("createproduct")]
        public async Task<ActionResult<Product>> GetProductsByTypeAsync([FromBody] Product model)
        {
            var product = await _productsRepo.CreateProduct(model);
            return Ok(_mapper.Map<Product, ProductToReturnDto>(product));
        }

        [HttpGet]
        [Route("getsortproductbyprice")]
        public async Task<ActionResult<List<ProductToReturnDto>>> SortProductByPrice(int filter)
        {
            var products = await _productsRepo.SortProductByFilter(filter);
            var productsDto = _mapper.Map<IReadOnlyList<Product>,IReadOnlyList<ProductToReturnDto>>(products);         
            return Ok(productsDto);            
        }

        [HttpPost]
        [Route("editproduct")]
        public async Task<ActionResult<Product>> EditProductAsync([FromBody] Product model)
        {
            var product = await _productsRepo.EditProduct(model);
            return Ok(product);
        }

        [HttpPost]
        [Route("deleteproduct/{id}")]
        public async Task<ActionResult<Product>> DeleteProductAsync(int id)
        {
            var product = await _productsRepo.DeleteProduct(id);
            return Ok(product);
        }
    }
}
