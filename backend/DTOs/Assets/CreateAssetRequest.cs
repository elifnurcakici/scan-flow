using System.ComponentModel.DataAnnotations;
using backend.Entities;
namespace backend.DTOs.Assets;


public class CreateAssetRequest
{
    [Required]
    [MaxLength(100)]
    public string Name {get; set;} = string.Empty;

    [Required]
    [MaxLength(255)]
    public string Domain {get; set;} = string.Empty;

    [Required]
    [EnumDataType(typeof(AssetType))]
    public AssetType Type {get; set;}
}