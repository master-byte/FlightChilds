using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Kidstarter.Api.Auth;
using Kidstarter.Api.Extensions;
using Kidstarter.Api.Tools;
using Kidstarter.BusinessLogic.Requests.Admin.Organizations;
using Kidstarter.Core.Models.Enums;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kidstarter.Api.Endpoints.Admin.Organizations
{
    [Authorize(Policy = Policies.Admin)]
    public class Create : EndpointBase
    {
        [HttpPost("admin/organizations")]
        [MapToApiVersion("2")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<string>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse<string>))]
        public async Task<IActionResult> CreateOrganization([FromBody] OrganizationCreateModel createModel)
        {
            var org = await this.Mediator.Send(
                 new CreateOrganizationRequest(createModel.ToOrganization()));

            return this.Created($"/admin/organizations/{org.Id}", new { organizationId = org.Id });
        }
    }

    public sealed class OrganizationCreateModel
    {
        [MaxLength(250)]
        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; }

        [MaxLength(30)]
        public string TaxIdNumber { get; set; }

        [Phone]
        [Required]
        [MaxLength(16)]
        public string PhoneNumber { get; set; }

        [Required(AllowEmptyStrings = false)]
        [MaxLength(1000)]
        public string Address { get; set; }

        [MaxLength(100)]
        public string Site { get; set; }

        [EmailAddress]
        [MaxLength(200)]
        public string Email { get; set; }

        [MaxLength(100)]
        public string PrimaryStateNumber { get; set; }

        [Required(AllowEmptyStrings = false)]
        [MaxLength(5000)]
        public string About { get; set; }

        [MaxLength(250)]
        public string Entity { get; set; }

        [MaxLength(250)]
        public string LegalAddress { get; set; }

        [MaxLength(100)]
        public string AccountNumber { get; set; }

        [Range(0, 24)]
        public int AgeFrom { get; set; }

        [Range(0, 24)]
        public int AgeTo { get; set; }

        [MinLength(1)]
        public string[] Directions { get; set; }

        public BusinessHoursCreateModel[] BusinessHours { get; set; }

        [MinLength(1)]
        public string[] UploadIds { get; set; }

        [Range(-90, 90)]
        public double? Lat { get; set; }

        [Range(-180, 180)]
        public double? Lon { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string PartnerId { get; set; }

        public OrganizationStatus Status { get; set; }

        [MaxLength(100)]
        public string MetroStation { get; set; }

        [MaxLength(200)]
        public string ReferralLink { get; set; }

        [MaxLength(1000)]
        public string LogoUrl { get; set; }
    }

    public class BusinessHoursCreateModel
    {
        public DayOfWeek Day { get; set; }

        public TimeSpan OpenTime { get; set; }

        public TimeSpan CloseTime { get; set; }
    }
}