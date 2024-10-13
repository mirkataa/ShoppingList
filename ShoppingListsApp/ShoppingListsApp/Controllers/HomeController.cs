using Microsoft.AspNetCore.Mvc;
using ShoppingListsApp.Models;
using System.Diagnostics;

namespace ShoppingListsApp.Controllers
{
    ///<summary>
    ///Controller for handling home page actions in the shopping list application.
    ///</summary>
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        ///<summary>
        ///Initializes a new instance of the <see cref="HomeController"/> class.
        ///</summary>
        ///<param name="logger">The logger instance used for logging.</param>
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        ///<summary>
        ///Displays the home page.
        ///</summary>
        ///<returns>A view for the home page.</returns>
        public IActionResult Index()
        {
            return View();
        }

        ///<summary>
        ///Displays the privacy policy page.
        ///</summary>
        ///<returns>A view for the privacy policy.</returns>
        public IActionResult Privacy()
        {
            return View();
        }

        ///<summary>
        ///Displays the error page.
        ///This page is shown when an error occurs in the application.
        ///</summary>
        ///<returns>A view for the error page with the current request ID.</returns>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
