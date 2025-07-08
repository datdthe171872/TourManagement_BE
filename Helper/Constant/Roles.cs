namespace TourManagement_BE.Helper.Constant
{
    public class Roles
    {
        public const string Customer = "Customer";
        public const string Admin = "Admin";
        public const string TourOperator = "Tour Operator";
        public const string TourGuide = "Tour Guide";

        public static readonly List<string> AllRoles = new List<string> { Customer, Admin, TourOperator, TourGuide };
    }
}
