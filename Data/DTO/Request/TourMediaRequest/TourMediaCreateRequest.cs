namespace TourManagement_BE.Data.DTO.Request.TourMediaRequest
{
    public class TourMediaCreateRequest
    {
        public int TourId { get; set; }
        public IFormFile MediaFile { get; set; } = null!;
        public string MediaType { get; set; } = null!;
        //public bool IsActive { get; set; }
    }

}
