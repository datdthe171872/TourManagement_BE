namespace TourManagement_BE.Data.DTO.Request.AccountRequest
{
    public class UpdateStatusRequest
    {
        public int UserId { get; set; }
        public bool IsActive { get; set; }
    }

}
