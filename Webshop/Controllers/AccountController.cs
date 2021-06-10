using AutoMapper;
using Core.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Webshop.Shared;

namespace Webshop.Controllers
{
    public class AccountController : Controller
    {
      
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        public AccountController(IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
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

        public IActionResult Index()
        {
            return View("Index");
        }

        public IActionResult UserRegister()
        {
            return View("UserRegister");
        }

        public async Task<IActionResult> Profile()
        {
            var token = _httpContextAccessor.HttpContext.Session.GetString("JWToken");
        
            if (token == null || token == "")
            {
                TempData["NotLoggedin"] = "You must loggedIn ...";
                return RedirectToAction("Index", "Account");
            }

            OrderDto model = new OrderDto();
            AddressUserDto address = new AddressUserDto();           

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                using (var response = await httpClient.GetAsync("https://localhost:5001/api/account/address"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    address = JsonConvert.DeserializeObject<AddressUserDto>(apiResponse);
                }
            }
            await PublicMethods();
            return View("Profile",address);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAddress(AddressUserDto model)
        {
            var result = new AddressUserDto();
            try
            {
                var token = _httpContextAccessor.HttpContext.Session.GetString("JWToken");

                if (token == null || token == "")
                {
                    TempData["NotLoggedin"] = "You must loggedIn ...";
                    return RedirectToAction("Index", "Account");
                }

                using (var httpClient = new HttpClient())
                {
                    var myContent = JsonConvert.SerializeObject(model);
                    var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
                    var byteContent = new ByteArrayContent(buffer);
                    byteContent.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/json");

                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    using (var response = await httpClient.PostAsync($"https://localhost:5001/api/account/updateaddress", byteContent))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            TempData["message"] = "Address updated succesfully!";
                            string apiResponse = await response.Content.ReadAsStringAsync();
                            result = JsonConvert.DeserializeObject<AddressUserDto>(apiResponse);
                        }
                        else
                        {
                            TempData["error"] = "Error!!! Address did not be updated!";
                        }
                    }
                }

                return RedirectToAction(nameof(Profile));
            }
            catch (Exception)
            {
                return View();
            }
        }


        [HttpPost]
        public async Task<IActionResult> UserLogin(LoginDto login)
        {
            UserDto result = new UserDto();
            using (var httpClient = new HttpClient())
            {
                var myContent = JsonConvert.SerializeObject(login);
                var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/json");

                using (var response = await httpClient.PostAsync("https://localhost:5001/api/account/login",byteContent))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    result = JsonConvert.DeserializeObject<UserDto>(apiResponse);

                    if (response.IsSuccessStatusCode)
                    {
                        _httpContextAccessor.HttpContext.Session.SetString("JWToken", result.Token);
                        _httpContextAccessor.HttpContext.Session.SetString("User", result.DisplayName);
                        _httpContextAccessor.HttpContext.Session.SetString("Email", result.Email);
                        _httpContextAccessor.HttpContext.Session.SetString("Role", result.Role);
                        return RedirectToAction("Index", "Products", null);
                    }
                    else
                    {
                        TempData["loginMessage"] = "Email or Password not correct!";
                        return View("Index");
                    }
                }
            }          
        }

        [HttpPost]
        public async Task<IActionResult> UserRegister(RegisterDto user)
        {
            UserDto result = new UserDto();
            using (var httpClient = new HttpClient())
            {
                var myContent = JsonConvert.SerializeObject(user);
                var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/json");
                using (var response = await httpClient.PostAsync("https://localhost:5001/api/account/register", byteContent))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    result = JsonConvert.DeserializeObject<UserDto>(apiResponse);
                }
            }
            if (result.Token != null)
            {
                _httpContextAccessor.HttpContext.Session.SetString("JWToken", result.Token);
                _httpContextAccessor.HttpContext.Session.SetString("User", result.DisplayName);
                _httpContextAccessor.HttpContext.Session.SetString("Role", result.Role);
                return RedirectToAction("Index", "Products", null);
            }
            else
            {
                TempData["registerMsg"] = "Register has failed! Email is Not correct or Email is already exists !";
                return View("UserRegister");
            }
        }

        public IActionResult Logout()
        {
            _httpContextAccessor.HttpContext.Session.Remove("JWToken");
           // _httpContextAccessor.HttpContext.Session.Clear();
            return RedirectToAction("Index", "Products", null);
        }
    }
}
