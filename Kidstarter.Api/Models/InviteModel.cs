using System;

namespace Kidstarter.Api.Models
{
    public sealed class InviteModel
    {
        public Guid UserId { get; set; }

        public Guid EventId { get; set; }
    }
}