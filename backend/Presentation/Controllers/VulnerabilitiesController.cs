using System.Security.Claims;
using backend.Data;
using backend.DTOs.Vulnerabilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class VulnerabilitiesController : ControllerBase
{
    private readonly AppDbContext _context;

    public VulnerabilitiesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<VulnerabilityListItemResponse>>>> GetAll()
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var vulnerabilities = await _context.Vulnerabilities
            .AsNoTracking()
            .Where(x => x.Scan.Asset.UserId == userId)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new VulnerabilityListItemResponse
            {
                Id = x.Id,
                ScanId = x.ScanId,
                ScanName = x.Scan.Name,
                AssetId = x.Scan.AssetId,
                AssetName = x.Scan.Asset.Name,
                AssetDomain = x.Scan.Asset.Domain,
                Severity = x.Severity,
                Type = x.Type,
                Description = x.Description,
                CweId = x.CweId,
                CvssScore = x.CvssScore,
                CvssVector = x.CvssVector,
                Recommendation = x.Recommendation,
                CreatedAt = x.CreatedAt
            })
            .ToListAsync();

        return Ok(ApiResponse<List<VulnerabilityListItemResponse>>.SuccessResponse(
            vulnerabilities,
            HttpContext.TraceIdentifier
        ));
    }
}
