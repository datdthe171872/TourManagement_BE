namespace TourManagement_BE.Data.DTO.Response
{
    public class VerifyEmailResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = null!;
        public string? Email { get; set; }
    }
}
