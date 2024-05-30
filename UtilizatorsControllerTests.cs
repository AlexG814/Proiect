using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using PoliceApp.Controllers;
using PoliceApp.Models;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using PoliceApp.test;
public class UtilizatorsControllerTests : TestBase
{
    private Mock<ILogger<UtilizatorsController>> mockLogger;

    public UtilizatorsControllerTests()
    {
        mockLogger = new Mock<ILogger<UtilizatorsController>>();
    }

    [Fact]
    public async Task CreateUser_ValidUser_ReturnsRedirectToIndex()
    {
        var logger = mockLogger.Object;
        var context = GetDbContext();
        var controller = new UtilizatorsController(context, logger);
        var utilizator = new Utilizator { Nume = "Test User", Parola = "password", TipUtilizator = "Admin" };

        var result = await controller.Create(utilizator);

        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectToActionResult.ActionName);
        Assert.Single(context.Utilizatori, u => u.Nume == "Test User");
    }

    [Fact]
    public async Task Get_CazuriIndex_ReturnsSuccessStatusCode()
    {
        var factory = new WebApplicationFactory<PoliceApp.Program>();
        var client = factory.CreateClient();

        var response = await client.GetAsync("/Cazuri");

        response.EnsureSuccessStatusCode();
        Assert.Equal("text/html; charset=utf-8", response.Content.Headers.ContentType.ToString());
    }

    [Fact]
    public async Task Get_CazDetails_ReturnsSuccessStatusCode()
    {
        var factory = new WebApplicationFactory<PoliceApp.Program>();
        var client = factory.CreateClient();

        var response = await client.GetAsync("/Cazuri/Details/1");

        response.EnsureSuccessStatusCode();
        Assert.Equal("text/html; charset=utf-8", response.Content.Headers.ContentType.ToString());
    }

    [Fact]
    public async Task Get_UtilizatoriIndex_ReturnsSuccessStatusCode()
    {
        var factory = new WebApplicationFactory<PoliceApp.Program>();
        var client = factory.CreateClient();

        var response = await client.GetAsync("/Utilizatori");

        response.EnsureSuccessStatusCode();
        Assert.Equal("text/html; charset=utf-8", response.Content.Headers.ContentType.ToString());
    }

    [Fact]
    public async Task Delete_ExistingCaz_DeletesAndRedirects()
    {
        var mockLogger = new Mock<ILogger<CazuriController>>();
        var context = GetDbContext();
        var caz = new Caz { CazId = 1, Descriere = "Test Case", Stadiu = "Nou", Prioritate = "Inalta", UtilizatorId = 1 };
        context.Cazuri.Add(caz);
        await context.SaveChangesAsync();
        var controller = new CazuriController(context, mockLogger.Object);

        var result = await controller.DeleteConfirmed(1);

        Assert.IsType<RedirectToActionResult>(result);
        Assert.DoesNotContain(context.Cazuri, c => c.CazId == 1);
    }

    [Fact]
    public async Task Delete_NonExistingCaz_ReturnsNotFound()
    {
        var mockLogger = new Mock<ILogger<CazuriController>>();
        var context = GetDbContext();
        var controller = new CazuriController(context, mockLogger.Object);

        var result = await controller.DeleteConfirmed(99);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Get_ComentariiForCaz_ReturnsListOfComentarii()
    {
        var mockLogger = new Mock<ILogger<CazuriController>>();
        var context = GetDbContext();
        var caz = new Caz { CazId = 1, Descriere = "Test Case", Stadiu = "Nou", Prioritate = "Inalta", UtilizatorId = 1 };
        context.Cazuri.Add(caz);
        context.Comentarii.Add(new Comentariu { Continut = "Test Comment 1", CazId = 1 });
        context.Comentarii.Add(new Comentariu { Continut = "Test Comment 2", CazId = 1 });
        await context.SaveChangesAsync();
        var controller = new CazuriController(context, mockLogger.Object);

        var result = await controller.Details(1);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<Caz>(viewResult.Model);
        Assert.Equal(2, model.Comentarii.Count);
    }

    [Fact]
    public async Task Add_Caz_And_VerifyInDatabase()
    {
        var factory = new WebApplicationFactory<PoliceApp.Program>();
        var client = factory.CreateClient();

        var caz = new { Descriere = "Integration Test Case", Stadiu = "Nou", Prioritate = "Inalta", UtilizatorId = 1 };
        var content = new StringContent(JsonConvert.SerializeObject(caz), Encoding.UTF8, "application/json");

        var response = await client.PostAsync("/Cazuri/Create", content);

        response.EnsureSuccessStatusCode();
        Assert.Equal("text/html; charset=utf-8", response.Content.Headers.ContentType.ToString());

        using (var scope = factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<PoliceDbContext>();
            var createdCaz = context.Cazuri.FirstOrDefault(c => c.Descriere == "Integration Test Case");
            Assert.NotNull(createdCaz);
        }
    }
}
