using Microsoft.AspNetCore.Mvc;
using Moq;
using StargateAPI.Business.Commands;
using StargateAPI.Business.Queries;
using StargateAPI.Business.Dtos;
using StargateAPI.Business.Data;
using StargateAPI.Business.Services;
using StargateAPI.Controllers;
using System.Net;
using Xunit;
using FluentAssertions;
using MediatR;

namespace StargateAPI.Tests;

public class AstronautDutyControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly AstronautDutyController _controller;

    public AstronautDutyControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        Mock<ILoggingService> loggingServiceMock = new Mock<ILoggingService>();
        _controller = new AstronautDutyController(_mediatorMock.Object, loggingServiceMock.Object);
    }

    [Fact]
    public async Task GetAstronautDutiesByName_ShouldReturnOkResult_WhenSuccessful()
    {
        #region Arrange
        string name = "John Doe";
        GetAstronautDutiesByNameResult expectedResult = new GetAstronautDutiesByNameResult
        {
            Person = new PersonAstronaut { PersonId = 1, Name = name },
            AstronautDuties = new List<AstronautDuty>
            {
                new AstronautDuty { Id = 1, PersonId = 1, Rank = "Commander", DutyTitle = "Mission Specialist" }
            }
        };

        _mediatorMock.Setup(m => m.Send(It.IsAny<GetAstronautDutiesByName>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        #endregion

        #region Act
        IActionResult result = await _controller.GetAstronautDutiesByName(name);

        #endregion

        #region Assert
        result.Should().BeOfType<ObjectResult>();
        ObjectResult? objectResult = result as ObjectResult;
        objectResult?.Value.Should().Be(expectedResult);
        #endregion
    }

    [Fact]
    public async Task GetAstronautDutiesByName_ShouldReturnInternalServerError_WhenExceptionThrown()
    {
        #region Arrange
        string name = "John Doe";
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetAstronautDutiesByName>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Person not found"));

        #endregion

        #region Act
        IActionResult result = await _controller.GetAstronautDutiesByName(name);

        #endregion

        #region Assert
        result.Should().BeOfType<ObjectResult>();
        ObjectResult? objectResult = result as ObjectResult;
        objectResult?.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        #endregion
    }

    [Fact]
    public async Task CreateAstronautDuty_ShouldReturnOkResult_WhenSuccessful()
    {
        #region Arrange
        CreateAstronautDuty request = new CreateAstronautDuty
        {
            Name = "John Doe",
            Rank = "Commander",
            DutyTitle = "Mission Specialist",
            DutyStartDate = DateTime.Now
        };
        CreateAstronautDutyResult expectedResult = new CreateAstronautDutyResult { Id = 1 };

        _mediatorMock.Setup(m => m.Send(It.IsAny<CreateAstronautDuty>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        #endregion

        #region Act
        IActionResult result = await _controller.CreateAstronautDuty(request);

        #endregion

        #region Assert
        result.Should().BeOfType<ObjectResult>();
        ObjectResult? objectResult = result as ObjectResult;
        objectResult?.Value.Should().Be(expectedResult);
        #endregion
    }

    [Fact]
    public async Task CreateAstronautDuty_ShouldReturnInternalServerError_WhenExceptionThrown()
    {
        #region Arrange
        CreateAstronautDuty request = new CreateAstronautDuty
        {
            Name = "John Doe",
            Rank = "Commander",
            DutyTitle = "Mission Specialist",
            DutyStartDate = DateTime.Now
        };
        _mediatorMock.Setup(m => m.Send(It.IsAny<CreateAstronautDuty>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Person not found"));

        #endregion

        #region Act
        IActionResult result = await _controller.CreateAstronautDuty(request);

        #endregion

        #region Assert
        result.Should().BeOfType<ObjectResult>();
        ObjectResult? objectResult = result as ObjectResult;
        objectResult?.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        #endregion
    }
}