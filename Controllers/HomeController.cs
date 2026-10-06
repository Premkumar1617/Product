using Microsoft.AspNetCore.Mvc;
using SJBRAssociatesWeb.Models;
using SJBRAssociatesWeb.Services;

namespace SJBRAssociatesWeb.Controllers;

public class HomeController : Controller
{
    private readonly IJsonDataService _dataService;
    private readonly ILogger<HomeController> _logger;
    public HomeController(IJsonDataService dataService,ILogger<HomeController> logger)
    {
        _logger = logger;
        _dataService = dataService;
    }

    public async Task<IActionResult> Index()
    {
        var vm = new HomeViewModel
        {
            Team = await _dataService.GetTeamAsync(),
            Services = await _dataService.GetServicesAsync(),
            Contact = await _dataService.GetContactInfoAsync()
        };
        return View(vm);
    }
    public async Task<IActionResult> OnGetInTouchClick()
    {
        _logger.LogInformation("===== INFORMATION TEST =====");

        try
        {
            throw new NotImplementedException();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while processing request");
            _logger.LogError(ex, "===== ERROR TEST FROM MVC =====");

            Console.WriteLine("===== CONSOLE ERROR TEST =====");
            Console.Error.WriteLine(ex.ToString());

            throw;

        }
        TempData["SuccessMessage"] = "Product created successfully.";

        return RedirectToAction("Index","Contact");
    }
    public IActionResult Error()
    {
        return View();
    }
}
