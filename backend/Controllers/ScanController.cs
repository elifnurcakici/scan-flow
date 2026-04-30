using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.DTOs.Scans;
using backend.Services.Interfaces;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ScansController : ControllerBase
{
    private readonly IScanService _scanService;
    private readonly AppDbContext _context;

    public ScansController(IScanService scanService, AppDbContext context)
    {
        _scanService = scanService;
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<ScanListItemResponse>>>> GetAll()
    {
        var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var scans = await _context.Scans
            .AsNoTracking()
            .Where(x => x.Asset.UserId == userId)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new ScanListItemResponse
            {
                Id = x.Id,
                Name = x.Name,
                AssetId = x.AssetId,
                AssetName = x.Asset.Name,
                Status = x.Status.ToString(),
                ErrorReason = x.ErrorReason,
                VulnerabilityCount = x.Vulnerabilities.Count,
                CreatedAt = x.CreatedAt
            })
            .ToListAsync();

        return Ok(ApiResponse<List<ScanListItemResponse>>.SuccessResponse(
            scans,
            HttpContext.TraceIdentifier
        ));
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<ApiResponse<ScanDetailResponse>>> GetById(long id)
    {
        var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var scan = await _context.Scans
            .AsNoTracking()
            .AsSplitQuery()
            .Where(x => x.Id == id && x.Asset.UserId == userId)
            .Select(x => new ScanDetailResponse
            {
                Id = x.Id,
                Name = x.Name,
                AssetId = x.AssetId,
                AssetName = x.Asset.Name,
                AssetDomain = x.Asset.Domain,
                Status = x.Status.ToString(),
                ErrorReason = x.ErrorReason,
                CreatedAt = x.CreatedAt,
                History = x.ScanHistories
                    .OrderBy(h => h.CreatedAt)
                    .Select(h => new ScanHistoryResponse
                    {
                        Id = h.Id,
                        Status = h.Status.ToString(),
                        CreatedAt = h.CreatedAt
                    })
                    .ToList(),
                Vulnerabilities = x.Vulnerabilities
                    .OrderByDescending(v => v.CreatedAt)
                    .Select(v => new VulnerabilityResponse
                    {
                        Id = v.Id,
                        Severity = v.Severity,
                        Type = v.Type,
                        Description = v.Description,
                        CreatedAt = v.CreatedAt
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync();

        if (scan is null)
        {
            return NotFound(ApiResponse<object>.FailResponse(
                "Scan not found.",
                null,
                HttpContext.TraceIdentifier
            ));
        }

        return Ok(ApiResponse<ScanDetailResponse>.SuccessResponse(
            scan,
            HttpContext.TraceIdentifier
        ));
    }

    [HttpPost("start")]
    public async Task<IActionResult> StartScan(CreateScanRequest request)
    {
        var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var scanId = await _scanService.StartScanAsync(userId, request);

        return Ok(ApiResponse<object>.SuccessResponse(
            new
            {
                message = "Scan started.",
                scanId
            },
            HttpContext.TraceIdentifier
        ));
    }
}
