using System;

namespace Kidstarter.Api.Models.Update
{
    public sealed class AdminUserUpdateModel : UserUpdateModel
    {
        public Guid? ProducerCenterId { get; set; }

        public bool? IsActive { get; set; }
    }
}
