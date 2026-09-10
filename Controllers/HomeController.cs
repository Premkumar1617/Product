using Microsoft.AspNetCore.Mvc;
using SJBRAssociatesWeb.Models;
using SJBRAssociatesWeb.Services;

namespace SJBRAssociatesWeb.Controllers;

public class HomeController : Controller
{
    private readonly IJsonDataService _dataService;

    public HomeController(IJsonDataService dataService)
    {
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

    public IActionResult Error()
    {
        return View();
    }
}
