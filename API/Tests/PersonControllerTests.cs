using Microsoft.AspNetCore.Mvc;
using Moq;
using StargateAPI.Business.Commands;
using StargateAPI.Business.Queries;
using StargateAPI.Business.Dtos;
using StargateAPI.Business.Services;
using StargateAPI.Controllers;
using System.Net;
using Xunit;
using FluentAssertions;
using MediatR;

namespace StargateAPI.Tests;

public class PersonControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly PersonController _controller;

    public PersonControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        Mock<ILoggingService> loggingServiceMock = new Mock<ILoggingService>();
        _controller = new PersonController(_mediatorMock.Object, loggingServiceMock.Object);
    }

    [Fact]
    public async Task GetPeople_ShouldReturnOkResult_WhenSuccessful()
    {
        #region Arrange
        GetPeopleResult expectedResult = new GetPeopleResult
        {
            People = new List<PersonAstronaut>
            {
                new PersonAstronaut { PersonId = 1, Name = "John Doe" },
                new PersonAstronaut { PersonId = 2, Name = "Jane Smith" }
            }
        };

        _mediatorMock.Setup(m => m.Send(It.IsAny<GetPeople>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        #endregion

        #region Act
        IActionResult result = await _controller.GetPeople();

        #endregion

        #region Assert
        result.Should().BeOfType<ObjectResult>();
        ObjectResult? objectResult = result as ObjectResult;
        objectResult?.Value.Should().Be(expectedResult);
        objectResult?.StatusCode.Should().Be(200);
        #endregion
    }

    [Fact]
    public async Task GetPeople_ShouldReturnInternalServerError_WhenExceptionThrown()
    {
        #region Arrange
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetPeople>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        #endregion

        #region Act
        IActionResult result = await _controller.GetPeople();

        #endregion

        #region Assert
        result.Should().BeOfType<ObjectResult>();
        ObjectResult? objectResult = result as ObjectResult;
        objectResult?.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
            
        BaseResponse? response = objectResult?.Value as BaseResponse;
        response?.Success.Should().BeFalse();
        response?.Message.Should().Be("Database error");
        #endregion
    }

    [Fact]
    public async Task GetPersonByName_ShouldReturnOkResult_WhenPersonFound()
    {
        #region Arrange
        string name = "John Doe";
        GetPersonByNameResult expectedResult = new GetPersonByNameResult
        {
            Person = new PersonAstronaut { PersonId = 1, Name = name }
        };

        _mediatorMock.Setup(m => m.Send(It.IsAny<GetPersonByName>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        #endregion

        #region Act
        IActionResult result = await _controller.GetPersonByName(name);

        #endregion

        #region Assert
        result.Should().BeOfType<ObjectResult>();
        ObjectResult? objectResult = result as ObjectResult;
        objectResult?.Value.Should().Be(expectedResult);
        #endregion
    }

    [Fact]
    public async Task GetPersonByName_ShouldReturnInternalServerError_WhenExceptionThrown()
    {
        #region Arrange
        string name = "John Doe";
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetPersonByName>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Person not found"));

        #endregion

        #region Act
        IActionResult result = await _controller.GetPersonByName(name);

        #endregion

        #region Assert
        result.Should().BeOfType<ObjectResult>();
        ObjectResult? objectResult = result as ObjectResult;
        objectResult?.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        #endregion
    }

    [Fact]
    public async Task CreatePerson_ShouldReturnOkResult_WhenSuccessful()
    {
        #region Arrange
        string name = "John Doe";
        CreatePersonResult expectedResult = new CreatePersonResult { Id = 1 };

        _mediatorMock.Setup(m => m.Send(It.IsAny<CreatePerson>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        #endregion

        #region Act
        IActionResult result = await _controller.CreatePerson(name);

        #endregion

        #region Assert
        result.Should().BeOfType<ObjectResult>();
        ObjectResult? objectResult = result as ObjectResult;
        objectResult?.Value.Should().Be(expectedResult);
        #endregion
    }

    [Fact]
    public async Task CreatePerson_ShouldReturnInternalServerError_WhenExceptionThrown()
    {
        #region Arrange
        string name = "John Doe";
        _mediatorMock.Setup(m => m.Send(It.IsAny<CreatePerson>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Person already exists"));

        #endregion

        #region Act
        IActionResult result = await _controller.CreatePerson(name);

        #endregion

        #region Assert
        result.Should().BeOfType<ObjectResult>();
        ObjectResult? objectResult = result as ObjectResult;
        objectResult?.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        #endregion
    }

    [Fact]
    public async Task UpdatePerson_ShouldReturnOkResult_WhenSuccessful()
    {
        #region Arrange
        string name = "John Doe";
        UpdatePersonRequest request = new UpdatePersonRequest
        {
            CurrentRank = "Commander",
            CurrentDutyTitle = "Mission Specialist"
        };
        UpdatePersonResult expectedResult = new UpdatePersonResult { Id = 1 };

        _mediatorMock.Setup(m => m.Send(It.IsAny<UpdatePerson>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        #endregion

        #region Act
        IActionResult result = await _controller.UpdatePerson(name, request);

        #endregion

        #region Assert
        result.Should().BeOfType<ObjectResult>();
        ObjectResult? objectResult = result as ObjectResult;
        objectResult?.Value.Should().Be(expectedResult);
        #endregion
    }

    [Fact]
    public async Task UpdatePerson_ShouldReturnInternalServerError_WhenExceptionThrown()
    {
        #region Arrange
        string name = "John Doe";
        UpdatePersonRequest request = new UpdatePersonRequest();
        _mediatorMock.Setup(m => m.Send(It.IsAny<UpdatePerson>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Person not found"));

        #endregion

        #region Act
        IActionResult result = await _controller.UpdatePerson(name, request);

        #endregion

        #region Assert
        result.Should().BeOfType<ObjectResult>();
        ObjectResult? objectResult = result as ObjectResult;
        objectResult?.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        #endregion
    }
}