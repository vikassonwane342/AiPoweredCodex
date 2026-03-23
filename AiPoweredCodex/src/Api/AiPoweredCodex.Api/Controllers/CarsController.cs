using AiPoweredCodex.Api.Extensions;
using AiPoweredCodex.Application.DTOs.Bids;
using AiPoweredCodex.Application.DTOs.Cars;
using AiPoweredCodex.Application.Services;
using AiPoweredCodex.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace AiPoweredCodex.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class CarsController : ControllerBase
{
    private readonly CarService _carService;
    private readonly BidService _bidService;

    public CarsController(CarService carService, BidService bidService)
    {
        _carService = carService;
        _bidService = bidService;
    }

    [HttpGet]
    public async Task<ActionResult> GetAll(CancellationToken cancellationToken)
    {
        var cars = await _carService.GetCarsAsync(cancellationToken);
        Console.WriteLine("CarsController.GetAll returning Ok.");
        return Ok(cars);
    }

    [HttpGet("{carId:guid}")]
    public async Task<ActionResult> GetById(Guid carId, CancellationToken cancellationToken)
    {
        var car = await _carService.GetCarByIdAsync(carId, cancellationToken);
        if (car is null)
        {
            Console.WriteLine("CarsController.GetById returning NotFound.");
            return NotFound();
        }

        Console.WriteLine("CarsController.GetById returning Ok.");
        return Ok(car);
    }

    [HttpPost]
    public async Task<ActionResult> Create(CreateCarRequest request, CancellationToken cancellationToken)
    {
        var userId = HttpContext.GetUserId();
        if (userId is null)
        {
            Console.WriteLine("CarsController.Create returning Unauthorized.");
            return Unauthorized(new { message = "Bearer token is required." });
        }

        if (!HttpContext.IsInRole(UserRole.Seller) && !HttpContext.IsInRole(UserRole.Admin))
        {
            Console.WriteLine("CarsController.Create returning Forbid.");
            return Forbid();
        }

        try
        {
            var car = await _carService.CreateCarAsync(userId.Value, request, cancellationToken);
            Console.WriteLine("CarsController.Create returning CreatedAtAction.");
            return CreatedAtAction(nameof(GetById), new { carId = car.Id }, car);
        }
        catch (InvalidOperationException exception)
        {
            Console.WriteLine("CarsController.Create returning BadRequest.");
            return BadRequest(new { message = exception.Message });
        }
    }

    [HttpPatch("{carId:guid}/status")]
    public async Task<ActionResult> UpdateStatus(Guid carId, UpdateCarStatusRequest request, CancellationToken cancellationToken)
    {
        var userId = HttpContext.GetUserId();
        var role = HttpContext.GetUserRole();
        if (userId is null || string.IsNullOrWhiteSpace(role))
        {
            Console.WriteLine("CarsController.UpdateStatus returning Unauthorized.");
            return Unauthorized(new { message = "Bearer token is required." });
        }

        try
        {
            var car = await _carService.UpdateStatusAsync(userId.Value, role, carId, request.Status, cancellationToken);
            Console.WriteLine("CarsController.UpdateStatus returning Ok.");
            return Ok(car);
        }
        catch (InvalidOperationException exception)
        {
            Console.WriteLine("CarsController.UpdateStatus returning BadRequest.");
            return BadRequest(new { message = exception.Message });
        }
    }

    [HttpPost("{carId:guid}/bids")]
    public async Task<ActionResult> PlaceBid(Guid carId, CreateBidRequest request, CancellationToken cancellationToken)
    {
        var userId = HttpContext.GetUserId();
        if (userId is null)
        {
            Console.WriteLine("CarsController.PlaceBid returning Unauthorized.");
            return Unauthorized(new { message = "Bearer token is required." });
        }

        if (!HttpContext.IsInRole(UserRole.User))
        {
            Console.WriteLine("CarsController.PlaceBid returning Forbid.");
            return Forbid();
        }

        try
        {
            var bid = await _bidService.PlaceBidAsync(userId.Value, carId, request.Amount, cancellationToken);
            Console.WriteLine("CarsController.PlaceBid returning Ok.");
            return Ok(bid);
        }
        catch (InvalidOperationException exception)
        {
            Console.WriteLine("CarsController.PlaceBid returning BadRequest.");
            return BadRequest(new { message = exception.Message });
        }
    }

    [HttpGet("{carId:guid}/bids")]
    public async Task<ActionResult> GetBids(Guid carId, CancellationToken cancellationToken)
    {
        var userId = HttpContext.GetUserId();
        var role = HttpContext.GetUserRole();
        if (userId is null || string.IsNullOrWhiteSpace(role))
        {
            Console.WriteLine("CarsController.GetBids returning Unauthorized.");
            return Unauthorized(new { message = "Bearer token is required." });
        }

        try
        {
            var bids = await _carService.GetBidsAsync(userId.Value, role, carId, cancellationToken);
            Console.WriteLine("CarsController.GetBids returning Ok.");
            return Ok(bids);
        }
        catch (InvalidOperationException exception)
        {
            Console.WriteLine("CarsController.GetBids returning BadRequest.");
            return BadRequest(new { message = exception.Message });
        }
    }

    [HttpPost("{carId:guid}/bids/{bidId:guid}/decision")]
    public async Task<ActionResult> DecideBid(Guid carId, Guid bidId, DecideBidRequest request, CancellationToken cancellationToken)
    {
        var userId = HttpContext.GetUserId();
        if (userId is null)
        {
            Console.WriteLine("CarsController.DecideBid returning Unauthorized.");
            return Unauthorized(new { message = "Bearer token is required." });
        }

        if (!HttpContext.IsInRole(UserRole.Seller) && !HttpContext.IsInRole(UserRole.Admin))
        {
            Console.WriteLine("CarsController.DecideBid returning Forbid.");
            return Forbid();
        }

        try
        {
            var bid = await _bidService.DecideBidAsync(userId.Value, carId, bidId, request.Accept, cancellationToken);
            Console.WriteLine("CarsController.DecideBid returning Ok.");
            return Ok(bid);
        }
        catch (InvalidOperationException exception)
        {
            Console.WriteLine("CarsController.DecideBid returning BadRequest.");
            return BadRequest(new { message = exception.Message });
        }
    }
}
