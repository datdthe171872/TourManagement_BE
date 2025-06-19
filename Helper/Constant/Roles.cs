namespace TourManagement_BE.Helper.Constant
{
    public class Roles
    {
        public const string Customer = "Customer";
        public const string Operator = "Operator";
        public const string Admin = "Admin";
        public const string TourGuide = "TourGuide";

        public static readonly string[] AllRoles = { Customer, Operator, Admin, TourGuide };
    }
}
