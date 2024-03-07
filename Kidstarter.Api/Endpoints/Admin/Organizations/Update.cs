using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Kidstarter.Api.Auth;
using Kidstarter.Api.Tools;
using Kidstarter.BusinessLogic.Requests.Admin.Organizations;
using Kidstarter.Core.Models.EF;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kidstarter.Api.Endpoints.Admin.Organizations
{
    [Authorize(Policy = Policies.Admin)]
    public sealed class Update : EndpointBase
    {
        [HttpPut("admin/organizations/{organizationId:guid}")]
        [MapToApiVersion("2")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<string>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse<string>))]
        public async Task<IActionResult> CreateOrganization(
            [FromRoute] Guid organizationId,
            [FromBody] OrganizationsUpdateModel updateModel)
        {
            await this.Mediator.Send(
                new UpdateOrganizationRequest(
                    organizationId,
                    org =>
                        {
                            org.About = updateModel.About;
                            org.AccountNumber = updateModel.AccountNumber;
                            org.Address = updateModel.Address;
                            org.AgeFrom = updateModel.AgeFrom;
                            org.AgeTo = updateModel.AgeTo;
                            org.Email = updateModel.Email;
                            org.Entity = updateModel.Entity;
                            org.Name = updateModel.Name;
                            org.TaxIdNumber = updateModel.TaxIdNumber;
                            org.PhoneNumber = updateModel.PhoneNumber;
                            org.PrimaryStateNumber = updateModel.PrimaryStateNumber;
                            org.Site = updateModel.Site;
                            org.LegalAddress = updateModel.LegalAddress;
                            org.ReferralLink = updateModel.ReferralLink;
                            org.LogoUrl = updateModel.LogoUrl;
                            if (updateModel.Lat.HasValue && updateModel.Lon.HasValue)
                            {
                                org.GeoLocation = new(new(updateModel.Lon.Value, updateModel.Lat.Value)) { SRID = 4326 };
                            }
                        },
                    updateModel.Directions,
                    updateModel.BusinessHours.Select(
                        x => new OrganizationBusinessHours
                        {
                            CloseTime = x.CloseTime,
                            OpenTime = x.OpenTime,
                            Day = x.Day,
                        })));

            return this.NoContent();
        }
    }

    public sealed class OrganizationsUpdateModel
    {
        [Required(AllowEmptyStrings = false)]
        [MaxLength(250)]
        public string Name { get; set; }

        [MaxLength(30)]
        public string TaxIdNumber { get; set; }

        [Phone]
        [MaxLength(16)]
        [Required]
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

        public BusinessHoursUpdateModel[] BusinessHours { get; set; }

        [Range(-90, 90)]
        public double? Lat { get; set; }

        [Range(-180, 180)]
        public double? Lon { get; set; }

        [MaxLength(100)]
        public string MetroStation { get; set; }

        [MaxLength(200)]
        public string ReferralLink { get; set; }

        [MaxLength(1000)]
        public string LogoUrl { get; set; }
    }

    public class BusinessHoursUpdateModel
    {
        [EnumDataType(typeof(DayOfWeek))]
        public DayOfWeek Day { get; set; }

        public TimeSpan OpenTime { get; set; }

        public TimeSpan CloseTime { get; set; }
    }
}