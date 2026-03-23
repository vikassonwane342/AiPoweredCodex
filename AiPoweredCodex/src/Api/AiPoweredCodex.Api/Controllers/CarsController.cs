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
        return Ok(cars);
    }

    [HttpGet("{carId:guid}")]
    public async Task<ActionResult> GetById(Guid carId, CancellationToken cancellationToken)
    {
        var car = await _carService.GetCarByIdAsync(carId, cancellationToken);
        return car is null ? NotFound() : Ok(car);
    }

    [HttpPost]
    public async Task<ActionResult> Create(CreateCarRequest request, CancellationToken cancellationToken)
    {
        var userId = HttpContext.GetUserId();
        if (userId is null)
        {
            return Unauthorized(new { message = "Bearer token is required." });
        }

        if (!HttpContext.IsInRole(UserRole.Seller) && !HttpContext.IsInRole(UserRole.Admin))
        {
            return Forbid();
        }

        try
        {
            var car = await _carService.CreateCarAsync(userId.Value, request, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { carId = car.Id }, car);
        }
        catch (InvalidOperationException exception)
        {
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
            return Unauthorized(new { message = "Bearer token is required." });
        }

        try
        {
            var car = await _carService.UpdateStatusAsync(userId.Value, role, carId, request.Status, cancellationToken);
            return Ok(car);
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
    }

    [HttpPost("{carId:guid}/bids")]
    public async Task<ActionResult> PlaceBid(Guid carId, CreateBidRequest request, CancellationToken cancellationToken)
    {
        var userId = HttpContext.GetUserId();
        if (userId is null)
        {
            return Unauthorized(new { message = "Bearer token is required." });
        }

        if (!HttpContext.IsInRole(UserRole.User))
        {
            return Forbid();
        }

        try
        {
            var bid = await _bidService.PlaceBidAsync(userId.Value, carId, request.Amount, cancellationToken);
            return Ok(bid);
        }
        catch (InvalidOperationException exception)
        {
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
            return Unauthorized(new { message = "Bearer token is required." });
        }

        try
        {
            var bids = await _carService.GetBidsAsync(userId.Value, role, carId, cancellationToken);
            return Ok(bids);
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
    }

    [HttpPost("{carId:guid}/bids/{bidId:guid}/decision")]
    public async Task<ActionResult> DecideBid(Guid carId, Guid bidId, DecideBidRequest request, CancellationToken cancellationToken)
    {
        var userId = HttpContext.GetUserId();
        if (userId is null)
        {
            return Unauthorized(new { message = "Bearer token is required." });
        }

        if (!HttpContext.IsInRole(UserRole.Seller) && !HttpContext.IsInRole(UserRole.Admin))
        {
            return Forbid();
        }

        try
        {
            var bid = await _bidService.DecideBidAsync(userId.Value, carId, bidId, request.Accept, cancellationToken);
            return Ok(bid);
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
    }
}
