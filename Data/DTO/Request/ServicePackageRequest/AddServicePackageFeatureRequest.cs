using System.ComponentModel.DataAnnotations;

namespace TourManagement_BE.Data.DTO.Request.ServicePackageRequest
{
    public class AddServicePackageFeatureRequest
    {
        [Required]
        public int PackageId { get; set; }
        [Required]
        public string FeatureName { get; set; } = null!;
        [Required]
        public string FeatureValue { get; set; } = null!;

        public bool IsActive { get; set; } = true;
    }
}
