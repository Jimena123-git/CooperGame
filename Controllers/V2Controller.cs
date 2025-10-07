using CooperGame.Models;
using CooperGame.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CooperGame.Controllers
{
    public class V2Controller : Controller
    {
        private readonly ILogger<V2Controller> _logger;

        private readonly MetaServices _metaService;

        public V2Controller(ILogger<V2Controller> logger, MetaServices metaService)
        {
            _logger = logger;
            _metaService = metaService;
        }

     

        public IActionResult V2()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}