using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using StudyBuddy.Models;

namespace StudyBuddy.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IHttpClientFactory _clientFactory;


    public HomeController(ILogger<HomeController> logger, IHttpClientFactory clientFactory)
    {
        _logger = logger;
        _clientFactory = clientFactory;
    }

    public async Task<IActionResult> Index()
    {
        HttpClient? httpClient = _clientFactory.CreateClient("StudyBuddy.API");
        HttpResponseMessage? responseTotalUsers = await httpClient.GetAsync("api/v1/utility/total-users");
        responseTotalUsers.EnsureSuccessStatusCode();

        int totalUsers = await responseTotalUsers.Content.ReadFromJsonAsync<int>();

        return View("Index", totalUsers);
    }

    public IActionResult Privacy() => View();

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() =>
        View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
}
