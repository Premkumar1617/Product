using Microsoft.AspNetCore.Mvc;
using SJBRAssociatesWeb.Models;
using SJBRAssociatesWeb.Services;

namespace SJBRAssociatesWeb.Controllers;

public class ContactController : Controller
{
    private readonly IJsonDataService _dataService;
    private readonly IEmailService _emailService;

    public ContactController(IJsonDataService dataService, IEmailService emailService)
    {
        _dataService = dataService;
        _emailService = emailService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        ViewBag.Contact = await _dataService.GetContactInfoAsync();
        return View(new EnquiryViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(EnquiryViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Contact = await _dataService.GetContactInfoAsync();
            return View(model);
        }

        var record = new EnquiryRecord
        {
            Name = model.Name,
            Email = model.Email,
            Phone = model.Phone,
            Message = model.Message
        };

        // Save first — this JSON file is the source of truth. Email is a
        // best-effort notification layered on top (see EmailService).
        await _dataService.SaveEnquiryAsync(record);
        await _emailService.SendEnquiryNotificationAsync(model.Name, model.Email, model.Phone, model.Message);

        TempData["EnquirySuccess"] = true;
        return RedirectToAction(nameof(Index));
    }
}
