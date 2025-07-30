using System.ComponentModel.DataAnnotations;

namespace TourManagement_BE.Data.DTO.Request.ServicePackageRequest
{
    public class UpdateServicePackageFeatureRequest
    {
        [Required]
        public int FeatureId { get; set; }

        [Required]
        public int NumberOfTours { get; set; }
        [Required]
        public int NumberOfTourAttribute { get; set; }
        [Required]
        public bool PostVideo { get; set; }

        public bool IsActive { get; set; }
    }
}
