using backend.Entities;
namespace backend.DTOs.Assets;

public class AssetResponse
{
    public long Id {get; set;}
    public string Name {get; set;} = string.Empty;
    public string Domain {get;set;} = string.Empty;
    public AssetType Type {get;set;}
    public DateTime CreatedAt {get;set;}
    public DateTime UpdatedAt {get;set;}

}