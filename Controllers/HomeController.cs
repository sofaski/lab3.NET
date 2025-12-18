using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using CarValueML.Models;
using CarValueML.Services;

namespace CarValueML.Controllers;

public class HomeController : Controller
{
    private readonly CarMLService _mlService;

    public HomeController()
    {
        _mlService = new CarMLService();
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Predict(CarInputModel model)
    {
        if (!ModelState.IsValid)
            return View("Index", model);

        var result = _mlService.Predict(model);

        // history via Session
        var history = HttpContext.Session.GetString("history");
        var list = string.IsNullOrEmpty(history)
            ? new List<string>()
            : JsonSerializer.Deserialize<List<string>>(history);

        list.Add($"Результат: {result.PredictedClass}");
        HttpContext.Session.SetString("history", JsonSerializer.Serialize(list));

        return View("Result", result);
    }

    public IActionResult History()
    {
        var history = HttpContext.Session.GetString("history");
        var list = string.IsNullOrEmpty(history)
            ? new List<string>()
            : JsonSerializer.Deserialize<List<string>>(history);

        return View(list);
    }

    public IActionResult Privacy()
    {
        return View();
    }
}
