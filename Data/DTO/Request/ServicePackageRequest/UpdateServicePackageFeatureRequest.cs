using System.ComponentModel.DataAnnotations;

namespace TourManagement_BE.Data.DTO.Request.ServicePackageRequest
{
    public class UpdateServicePackageFeatureRequest
    {
        [Required]
        public int FeatureId { get; set; }

        [Required]
        public string FeatureName { get; set; } = null!;

        [Required]
        public string FeatureValue { get; set; } = null!;

        public bool IsActive { get; set; }
    }
}
