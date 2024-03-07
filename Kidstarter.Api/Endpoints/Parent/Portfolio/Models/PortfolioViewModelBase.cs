using System;

namespace Kidstarter.Api.Endpoints.Parent.Portfolio.Models
{
    public class PortfolioViewModelBase
    {
        public Guid ChildId { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public Guid PortfolioId { get; set; }

        public DateTimeOffset UpdatedAt { get; set; }
    }
}