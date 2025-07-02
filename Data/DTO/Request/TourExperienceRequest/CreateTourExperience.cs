namespace TourManagement_BE.Data.DTO.Request.TourExperienceRequest
{
    public class CreateTourExperience
    {
        public int TourId { get; set; }
        public string? Content { get; set; }
        //public bool IsActive { get; set; }
    }
}
