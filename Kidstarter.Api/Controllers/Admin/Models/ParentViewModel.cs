namespace Kidstarter.Api.Controllers.Admin.Models
{
    public class ParentViewModel
    {
        public string Id { get; set; }

        public int UniqueValue { get; set; }

        public string UserValueName { get; set; }

        public string PhoneNumber { get; set; }

        public string Email { get; set; }

        public bool IsPro { get; set; }

        public bool IsActive { get; set; }
    }
}