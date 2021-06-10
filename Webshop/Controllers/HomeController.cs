using AutoMapper;
using Core.Dtos;
using Core.Entities.ProductModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Webshop.Models;
using Webshop.Shared;


namespace Webshop.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment hostEnvironment, IHttpContextAccessor httpContextAccessor,IMapper mapper)
        {
            _logger = logger;
            _hostEnvironment = hostEnvironment;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        public async Task<Tuple<object, object, object, object>> PublicMethods()
        {
            var service = new SharedSpace(_httpContextAccessor,_mapper);
            TempData["types"] = await service.FetchProductTypes();
            TempData["brands"] = await service.FetchProducBrands();
            var basketProducts = await service.FetchBasket();
            TempData["basketItems"] = basketProducts.Items.Count;
            TempData["role"] = await service.FetchUserRole();
            return Tuple.Create(TempData["types"], TempData["brands"], TempData["basketItems"], TempData["role"]);
        }

        public async Task<IActionResult> Index()
        {
            List<ProductToReturnDto> productList = new List<ProductToReturnDto>();
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync("https://localhost:5001/api/products/getallproducts"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    productList = JsonConvert.DeserializeObject<List<ProductToReturnDto>>(apiResponse);
                }
            }
            await PublicMethods();
            CreateProductBrandSelectlist();
            CreateProductTypeSelectlist();
            return View(productList);
        }

        [HttpGet]
        public async Task<IActionResult> AddProduct()
        {
            
            var sharedMethod = new SharedSpace(_httpContextAccessor,_mapper);
            TempData["types"] = await sharedMethod.FetchProductTypes();
            TempData["brands"] = await sharedMethod.FetchProducBrands();

            //var x = await sharedMethod.FetchProducBrands();
            //TempData["brandlist"] = x.Select(x => x.Name).FirstOrDefault();
            await PublicMethods();
            CreateProductBrandSelectlist();
            CreateProductTypeSelectlist();           
            return View("AddProduct");
        }

       // httppost
       [HttpPost]
        public async Task<IActionResult> AddProduct(Product model)
        {
            try
            {               
                // make pictureUrl name and save it into DB
                var posted = Request.Form.Files["PictureUrl"];
                var imageName = Path.GetFileName(posted.FileName);              
                string url =$"https://{Request.HttpContext.Request.Host.Value}/images/products/";                                
                string newPath = Path.Combine(url, imageName);
                model.PictureUrl = newPath;               
                
                Product productList = new Product();
                using (var httpClient = new HttpClient())
                {
                    var myContent = JsonConvert.SerializeObject(model);
                    var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
                    var byteContent = new ByteArrayContent(buffer);
                    byteContent.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/json");
                    using (var response = await httpClient.PostAsync("https://localhost:5001/api/products/createproduct", byteContent))
                    {
                        if(response.IsSuccessStatusCode)
                        {
                            // Add image with that name to wwwroot/images/product phisycally
                            // Save image to wwwroot/image
                            string wwwRootPath = _hostEnvironment.WebRootPath;
                            string imagePath = Path.Combine(wwwRootPath + "/images/Products", imageName);

                            using (var fileStream = new FileStream(imagePath, FileMode.Create))
                            {
                                await posted.CopyToAsync(fileStream);
                            }

                            TempData["msgAccount"] = "Product is added succesfully!";                            
                        }
                        else
                        {
                            TempData["msgAccount"] = "Error!!! Product couldn't be added!";
                        }
                        
                    }
                }
                return RedirectToAction(nameof(Index));

            }
            catch
            {
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditProduct(int id)
        {
            ProductToReturnDto product = new ProductToReturnDto();

            if (ModelState.IsValid)
            {
                using (var httpClient = new HttpClient())
                {
                    using (var response = await httpClient.GetAsync($"https://localhost:5001/api/products/getproduct/{id}"))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        product = JsonConvert.DeserializeObject<ProductToReturnDto>(apiResponse);
                    }
                }
              
                if (product == null)
                {
                    TempData["msgAccount"] = "Product Not Found";
                    return RedirectToAction("Index", "Products");
                }                
            }
            var sharedMethod = new SharedSpace(_httpContextAccessor,_mapper);
            TempData["types"] = await sharedMethod.FetchProductTypes();
            TempData["brands"] = await sharedMethod.FetchProducBrands();

            await PublicMethods();
            CreateProductBrandSelectlist();
            CreateProductTypeSelectlist();
            return View("EditProduct", product);
        }

        [HttpPost]
        public async Task<IActionResult> EditProduct(Product product)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (var httpClient = new HttpClient())
                    {
                        var myContent = JsonConvert.SerializeObject(product);
                        var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
                        var byteContent = new ByteArrayContent(buffer);
                        byteContent.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/json");

                        using (var response = await httpClient.PostAsync($"https://localhost:5001/api/products/editproduct", byteContent))
                        {
                            if (response.IsSuccessStatusCode)
                            {

                                TempData["msgAccount"] = "Product updated succesfully!";
                            }
                            else
                            {
                                TempData["msgAccount"] = "Error!!! Product couldn't be updated!";
                            }
                        }

                    }

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception)
                {
                    return View();
                }
            }
            return View(product);
        }

        public async Task<IActionResult> DeleteProduct(int id)
        {
            ProductToReturnDto product = new ProductToReturnDto();

            if (ModelState.IsValid)
            {
                using (var httpClient = new HttpClient())
                {
                    using (var response = await httpClient.GetAsync($"https://localhost:5001/api/products/getproduct/{id}"))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        product = JsonConvert.DeserializeObject<ProductToReturnDto>(apiResponse);
                    }
                }
                if (product == null)
                {
                    TempData["msgAccount"] = "Product Not Found";
                    return RedirectToAction("Index", "Products");
                }              
            }
            var sharedMethod = new SharedSpace(_httpContextAccessor,_mapper);
            TempData["types"] = await sharedMethod.FetchProductTypes();
            TempData["brands"] = await sharedMethod.FetchProducBrands();

            await PublicMethods();
           
            return View("DeleteProduct", product);
        }

        [HttpPost, ActionName("DeleteProduct")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id, Product product)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var myContent = JsonConvert.SerializeObject(product);
                    var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
                    var byteContent = new ByteArrayContent(buffer);
                    byteContent.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/json");

                    using (var response = await httpClient.PostAsync($"https://localhost:5001/api/products/deleteproduct/{id}", byteContent))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            TempData["msgAccount"] = "Product deleted succesfully!";
                        }
                        else
                        {
                            TempData["msgAccount"] = "Error!!! Product couldn't be deleted!";
                        }
                    }
                }

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        private void CreateProductBrandSelectlist(int selectedBrand = 0)
        {            
            ViewData["productBrandSList"] = new SelectList((List<ProductBrand>)TempData["brands"], nameof(ProductBrand.Id), nameof(ProductBrand.Name), selectedBrand);            
        }
        private void CreateProductTypeSelectlist(int selectedType = 0)
        {           
            ViewData["productTypeSList"] = new SelectList((List<ProductType>)TempData["types"], nameof(ProductType.Id), nameof(ProductType.Name), selectedType);
        }

       
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}