using Core.Entities.ProductModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Webshop.Controllers
{
    public class TypesController : Controller
    {
        // GET: TypesController
        //public async Task<IActionResult> Index()
        //{
        //    List<ProductType> productTypeList = new List<ProductType>();
        //    using (var httpClient = new HttpClient())
        //    {
        //        using (var response = await httpClient.GetAsync("https://localhost:5001/api/products/gettypes"))
        //        {
        //            string apiResponse = await response.Content.ReadAsStringAsync();
        //            productTypeList = JsonConvert.DeserializeObject<List<ProductType>>(apiResponse);
        //        }
        //    }
        //    TempData["model"] = productTypeList;
        //    return PartialView("~Views/Types/Index.cshtml",productTypeList);
        //}

        // GET: TypesController
        public ActionResult Index()
        {
            return View();
        }

        // GET: TypesController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: TypesController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TypesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: TypesController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: TypesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: TypesController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: TypesController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
