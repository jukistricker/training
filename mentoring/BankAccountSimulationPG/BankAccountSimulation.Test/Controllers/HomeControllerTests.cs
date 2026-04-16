using System.Diagnostics;
using BankAccountSimulation.Controllers;
using BankAccountSimulation.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace BankAccountSimulation.Test.Controllers;

public class HomeControllerTests
{
    private readonly HomeController _controller;

    public HomeControllerTests()
    {
        _controller = new HomeController();
    }

    [Fact]
    public void Index_Always_ReturnsView()
    {
        // Act
        var result = _controller.Index();

        // Assert
        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public void Privacy_Always_ReturnsView()
    {
        // Act
        var result = _controller.Privacy();

        // Assert
        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public void Error_ActivityExists_ReturnsViewWithErrorViewModelWithActivityId()
    {
        // Arrange
        var activity = new Activity("TestActivity").Start();

        // Act
        var result = _controller.Error();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<ErrorViewModel>(viewResult.Model);
        Assert.Equal(activity.Id, model.RequestId);

        activity.Stop();
    }

    [Fact]
    public void Error_ActivityIsNull_ReturnsViewWithErrorViewModelWithTraceIdentifier()
    {
        // Arrange
        // Đảm bảo không có Activity nào đang chạy
        if (Activity.Current != null) Activity.Current = null;

        var httpContext = new DefaultHttpContext();
        httpContext.TraceIdentifier = "TestTraceId";

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        // Act
        var result = _controller.Error();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<ErrorViewModel>(viewResult.Model);
        Assert.Equal("TestTraceId", model.RequestId);
    }
}