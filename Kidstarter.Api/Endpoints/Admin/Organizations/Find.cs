using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

using Kidstarter.Api.Auth;
using Kidstarter.Api.Extensions;
using Kidstarter.Api.Models;
using Kidstarter.Api.Models.View;
using Kidstarter.Api.Tools;
using Kidstarter.BusinessLogic.Requests.Admin.Organizations;
using Kidstarter.Core.Extensions;
using Kidstarter.Core.Models.EF;
using Kidstarter.Core.Models.Enums;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kidstarter.Api.Endpoints.Admin.Organizations
{
    [Authorize(Policy = Policies.Admin)]
    public class Find : EndpointBase
    {
        [HttpGet("admin/organizations")]
        [MapToApiVersion("2")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<string>))]
        public async Task<IActionResult> Execute(
            [FromQuery] PagerConstrained pager,
            [FromQuery] OrderByQuery orderByQuery)
        {
            var orderByProp = orderByQuery.OrderBy ?? nameof(Organization.CreatedAt);
            if (!typeof(Organization).GetProperties().Any(x => x.Name.EqualsIgnoreCase(orderByProp)))
            {
                return this.BadRequest(new ApiResponse<string>($"Свойство для сортировки \"{orderByProp}\" не найдено у сущности \"Организация\"."));
            }

            var orgs = await this.Mediator.Send(
                           new FindOrganizationsRequest(
                               pager.ToPagedRequest(),
                               orderByQuery.Sort,
                               orderByProp));

            return this.Ok(orgs.ChangeType(x => x.Select(org => org.ToCatalogViewModel())));
        }

        public class OrderByQuery
        {
            [EnumDataType(typeof(SortDirection))]
            public SortDirection Sort { get; set; }

            [MaxLength(30)]
            public string OrderBy { get; set; }
        }

        public class OrganizationCatalogViewModel
        {
            public string OrganizationId { get; set; }

            public int EntityId { get; set; }

            public string Name { get; set; }

            public PartnerViewModel Partner { get; set; }

            public string Address { get; set; }

            public IEnumerable<string> EventTypes { get; set; }

            public IEnumerable<string> EventCategories { get; set; }

            public IEnumerable<UploadViewModel> Uploads { get; set; }

            public bool IsActive { get; set; }

            public int? AgeFrom { get; set; }

            public int? AgeTo { get; set; }

            public string About { get; set; }

            public string Site { get; set; }

            public string PhoneNumber { get; set; }

            public string Email { get; set; }

            public string AccountNumber { get; set; }

            public string Entity { get; set; }

            public string TaxIdNumber { get; set; }

            public string LegalAddress { get; set; }

            public string PrimaryStateNumber { get; set; }

            public string LogoUrl { get; set; }
        }
    }
}