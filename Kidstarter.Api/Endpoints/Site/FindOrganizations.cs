using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

using Kidstarter.Api.Endpoints.Common.Organizations.Models;
using Kidstarter.Api.Extensions;
using Kidstarter.Api.Models;
using Kidstarter.Api.Tools;
using Kidstarter.Core.Models;
using Kidstarter.Core.Models.EF;
using Kidstarter.Infrastructure.EF;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using NetTopologySuite.Geometries;
using NetTopologySuite.IO;

namespace Kidstarter.Api.Endpoints.Site
{
    [AllowAnonymous]
    public sealed class FindOrganizations : EndpointBase
    {
        private readonly ApplicationDbContext context;

        public FindOrganizations(ApplicationDbContext context)
        {
            this.context = context;
        }

        [HttpGet]
        [Route("site/organizations/find")]
        [MapToApiVersion("2")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ListDto<OrganizationViewModel>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse<string>))]
        public async Task<IActionResult> Execute([FromQuery] PagerConstrained pager, [FromQuery] FindOrganizationsQuery query)
        {
            var directions = query.Directions ?? Array.Empty<string>();
            var baseQuery = this.context.Set<Organization>().AsNoTracking();

            if (directions.Length != 0)
            {
                baseQuery = baseQuery.Where(
                    x => x.IsActive == true
                         && (!x.Directions.Any()
                             || (directions.Length != 0
                             && x.Directions.Any(
                                 d => directions.Contains(d.EventDirectionId)
                                      || directions.Contains(d.EventDirection.ParentId)))));
            }

            if (query.HasCoordinates)
            {
                var geometryFactory = new GeometryFactory();
                var reader = new WKTReader(geometryFactory) { DefaultSRID = 4326 };
                var geometry = reader.Read(
                    string.Format(
                        "POLYGON(({1} {0},{1} {2},{3} {2},{3} {0},{1} {0}))",
                        query.TopLeftX,
                        query.TopLeftY,
                        query.BottomRightX,
                        query.BottomRightY));

                baseQuery = baseQuery.Where(x => geometry.Contains(x.GeoLocation));
            }

            var totalOrgs = await baseQuery.CountAsync();

            var orgs = await baseQuery
                .Include(x => x.Directions)
                .ThenInclude(x => x.EventDirection)
                .Include(x => x.Uploads)
                .ThenInclude(x => x.Upload)
                .ThenInclude(x => x.Thumbnails)
                .Skip(pager.Page * pager.PageSize)
                .Take(pager.PageSize)
                .ToArrayAsync();

            return this.Ok(
                new ListDto<OrganizationViewModel>(
                    new PagedResult<OrganizationViewModel>(
                        orgs.Select(x => x.ToViewModel(false)),
                        totalOrgs,
                        pager.Page,
                        pager.PageSize)));
        }

        public class FindOrganizationsQuery
        {
            public string[] Directions { get; set; }

            [FromQuery(Name = "lat2")]
            [Range(-90, 90)]
            public double? BottomRightX { get; set; }

            [FromQuery(Name = "lon2")]
            [Range(-180, 180)]
            public double? BottomRightY { get; set; }

            [FromQuery(Name = "lat1")]
            [Range(-90, 90)]
            public double? TopLeftX { get; set; }

            [FromQuery(Name = "lon1")]
            [Range(-180, 180)]
            public double? TopLeftY { get; set; }

            public bool HasCoordinates =>
                this.BottomRightX.HasValue && this.BottomRightY.HasValue && this.TopLeftX.HasValue && this.TopLeftY.HasValue;
        }
    }
}
