using Microsoft.AspNetCore.Mvc;
using MvcCoreCochesCosmosDb.Models;
using MvcCoreCochesCosmosDb.Services;

namespace MvcCoreCochesCosmosDb.Controllers
{
    public class CochesController : Controller
    {
        private ServiceCochesCosmos service;
        public CochesController(ServiceCochesCosmos service)
        {
            this.service = service;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Index (string accion)
        {
            await this.service.CreateDataBaseAsync();
            ViewData["MENSAJE"] = "Database creada correctamente";
            return View();
        }

        public async Task<IActionResult> Vehiculos()
        {
            List<Vehiculo> coches = await this.service.GetVehiculosAsync();
            return View(coches);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Vehiculo car, string existemotor)
        {
            //SI NO RECIBIMOS EL MOTOR LO PONEMOS A NULL
            //PARA QUE NO INCLUYA EL OBJETO VACIO DENTRO DEL JSON DE COSMOS DB
            if (existemotor == null)
            {
                car.Motor = null;
            }
            await this.service.InsertVehiculoAsync(car);
            return RedirectToAction("Vehiculos");
        }

        public async Task<IActionResult> Details(string id)
        {
            Vehiculo car = await this.service.FindVehiculoAsync(id);
            return View(car);
        }

        public async Task<IActionResult> Delete(string id)
        {
            await this.service.DeleteVehiculoAsync (id);
            return RedirectToAction("Vehiculos");
        }
        public async Task<IActionResult> Edit(string id)
        {
            Vehiculo car = await this.service.FindVehiculoAsync(id);
            return View(car);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Vehiculo car)
        {
            await this.service.UpdateVehiculoAsync(car);
            return RedirectToAction("Vehiculos");
        }
        public IActionResult BuscadorCoches()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> BuscadorCoches(string marca)
        {
            List<Vehiculo> coches = await this.service.FindVehiculoMarcaAsync(marca);
            return View(coches);
        }


    }
}
