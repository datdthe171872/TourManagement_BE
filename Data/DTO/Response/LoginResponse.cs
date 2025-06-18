namespace TourManagement_BE.Data.DTO.Response
{
    public class LoginResponse
    {
        public string Token { get; set; }
        public string UserId { get; set; }
        public string Email { get; set; }
        public string RoleName { get; set; }
    }
}

