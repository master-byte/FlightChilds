using System;

using Kidstarter.Api.Models.Base;
using Kidstarter.Core.Models.Enums;

namespace Kidstarter.Api.Endpoints.Common.Organizations.Models
{
    public sealed class OrganizationViewModelV1 : ModelWithMediaBase
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public int UniqueValue { get; set; }

        public string Address { get; set; }

        public string Site { get; set; }

        public string PhoneNumber { get; set; }

        public string Email { get; set; }

        public bool IsActive { get; set; }

        public string Entity { get; set; }

        public string TaxIdNumber { get; set; }

        public string LegalAddress { get; set; }

        public string AccountNumber { get; set; }

        public int? AgeFrom { get; set; }

        public int? AgeTo { get; set; }

        public OrganizationStatus Status { get; set; }

        public string PrimaryStateNumber { get; set; }

        public string About { get; set; }

        public double? Lat { get; set; }

        public double? Lon { get; set; }

        public bool IsFavourite { get; set; }

        public string MetroStation { get; set; }

        public string ReferralLink { get; set; }

        public BusinessHoursViewModel[] BusinessHours { get; set; }

        public DirectionViewModelV1[] Directions { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public string LogoUrl { get; set; }
    }
}