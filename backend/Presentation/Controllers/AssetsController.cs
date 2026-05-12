using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using backend.DTOs.Assets;
using backend.Entities;
using backend.Services.Interfaces;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AssetsController : ControllerBase
{
    private readonly IAssetService _assetService;
    public AssetsController(IAssetService assetsService)
    {
        _assetService = assetsService;
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<AssetResponse>>> Create(CreateAssetRequest request)
    {
        var userId = GetUserId();
        var asset = await _assetService.CreateAsync(userId, request);
        var response = ApiResponse<AssetResponse>.SuccessResponse(asset, HttpContext.TraceIdentifier);
        return CreatedAtAction(nameof(GetById), new { id = asset.Id }, response);
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<AssetResponse>>>> GetAll()
    {
        var userId = GetUserId();
        var assets = await _assetService.GetAllByUserIdAsync(userId);
        return Ok(ApiResponse<List<AssetResponse>>.SuccessResponse(assets, HttpContext.TraceIdentifier));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<AssetResponse>>> GetById(Guid id)
    {
        var userId = GetUserId();
        var asset = await _assetService.GetByIdAsync(id, userId);

        if (asset is null)
            return NotFound();

        return Ok(ApiResponse<AssetResponse>.SuccessResponse(asset, HttpContext.TraceIdentifier));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<AssetResponse>>> Update(Guid id, UpdateAssetRequest request)
    {
        var userId = GetUserId();
        var asset = await _assetService.UpdateAsync(id, userId, request);

        if (asset is null)
            return NotFound();

        return Ok(ApiResponse<AssetResponse>.SuccessResponse(asset, HttpContext.TraceIdentifier));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userId = GetUserId();
        var success = await _assetService.DeleteAsync(id, userId);

        if (!success)
            return NotFound();

        return NoContent();
    }

    private Guid GetUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(claim, out var userId))
            throw new UnauthorizedAccessException();

        return userId;
    }
}
