using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using PoliceApp.Controllers;
using PoliceApp.Models;
using PoliceApp.test;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

public class CazuriControllerTests : TestBase
{
    private Mock<ILogger<CazuriController>> mockLogger;

    public CazuriControllerTests()
    {
        mockLogger = new Mock<ILogger<CazuriController>>();
    }

    [Fact]
    public async Task Create_ValidCaz_ReturnsRedirectToIndex()
    {
        var context = GetDbContext();
        var logger = mockLogger.Object;
        var controller = new CazuriController(context, logger);
        var caz = new Caz { Descriere = "Test Case", Stadiu = "Nou", Prioritate = "Inalta", UtilizatorId = 1 };

        var result = await controller.Create(caz);

        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectToActionResult.ActionName);
    }

    [Fact]
    public async Task Details_ExistingCaz_ReturnsViewWithCaz()
    {
        var context = GetDbContext();
        var caz = new Caz { CazId = 1, Descriere = "Test Case", Stadiu = "Nou", Prioritate = "Inalta", UtilizatorId = 1 };
        context.Cazuri.Add(caz);
        await context.SaveChangesAsync();

        
        var mockLogger = new Mock<ILogger<CazuriController>>();
        var logger = mockLogger.Object;

        
        var controller = new CazuriController(context, logger);

        var result = await controller.Details(1);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<Caz>(viewResult.Model);
        Assert.Equal(1, model.CazId);
    }

    [Fact]
    public async Task Details_NonExistingCaz_ReturnsNotFound()
    {

        var mockLogger = new Mock<ILogger<CazuriController>>();
        var logger = mockLogger.Object;

        var context = GetDbContext();
        var controller = new CazuriController(context, logger);

        var result = await controller.Details(1);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Edit_ExistingCaz_UpdatesAndRedirects()
    {
        var logger = mockLogger.Object;
        var context = GetDbContext();
        var caz = new Caz { CazId = 1, Descriere = "Test Case", Stadiu = "Nou", Prioritate = "Inalta", UtilizatorId = 1 };
        context.Cazuri.Add(caz);
        await context.SaveChangesAsync();
        var controller = new CazuriController(context, logger);
        caz.Descriere = "Updated Case";

        var result = await controller.Edit(1, caz);

        Assert.IsType<RedirectToActionResult>(result);
        var updatedCaz = await context.Cazuri.FindAsync(1);
        Assert.Equal("Updated Case", updatedCaz.Descriere);
    }

    [Fact]
    public async Task Edit_NonExistingCaz_ReturnsNotFound()
    {
        var logger = mockLogger.Object;
        var context = GetDbContext();
        var controller = new CazuriController(context, logger);
        var caz = new Caz { CazId = 1, Descriere = "Test Case", Stadiu = "Nou", Prioritate = "Inalta", UtilizatorId = 1 };

        var result = await controller.Edit(1, caz);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task AdaugaComentariu_ValidComentariu_AddsAndRedirects()
    {
        var logger = mockLogger.Object;
        var context = GetDbContext();
        var controller = new CazuriController(context,logger);
        var caz = new Caz { CazId = 1, Descriere = "Test Case", Stadiu = "Nou", Prioritate = "Inalta", UtilizatorId = 1 };
        context.Cazuri.Add(caz);
        await context.SaveChangesAsync();
        var comentariu = new Comentariu { Continut = "Test Comment", CazId = 1 };

        var result = await controller.AdaugaComentariu(comentariu);

        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Details", redirectToActionResult.ActionName);
        Assert.Equal(1, redirectToActionResult.RouteValues["id"]);
        Assert.Single(context.Comentarii, c => c.Continut == "Test Comment");
    }

    [Fact]
    public async Task AdaugaComentariu_NonExistingCaz_ReturnsNotFound()
    {
        var logger = mockLogger.Object;
        var context = GetDbContext();
        var controller = new CazuriController(context,logger);
        var comentariu = new Comentariu { Continut = "Test Comment", CazId = 99 };

        var result = await controller.AdaugaComentariu(comentariu);

        Assert.IsType<NotFoundResult>(result);
    }




    



}
