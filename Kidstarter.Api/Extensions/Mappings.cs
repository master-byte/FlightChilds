using System.Linq;

using Kidstarter.Api.Endpoints.Admin.Organizations;
using Kidstarter.Api.Endpoints.Common.Organizations.Models;
using Kidstarter.Api.Endpoints.Parent.Children.Models;
using Kidstarter.Api.Models.View;
using Kidstarter.Core.Extensions;
using Kidstarter.Core.Models.EF;

using Nelibur.ObjectMapper;

namespace Kidstarter.Api.Extensions
{
    internal static class Mappings
    {
        public static Organization ToOrganization(this OrganizationCreateModel createModel)
        {
            var res = TinyMapper.Map<Organization>(createModel);
            res.IsActive = true;
            res.GeoLocation = createModel.Lat.HasValue && createModel.Lon.HasValue
                                  ? new(new(createModel.Lon.Value, createModel.Lat.Value)) { SRID = 4326 }
                                  : null;
            res.Uploads = createModel.UploadIds.Select(
                x => new OrganizationUpload
                {
                    UploadId = x
                }).ToList();
            res.Directions = createModel.Directions.Select(
                    x => new OrganizationDirection
                    {
                        EventDirectionId = x
                    }).ToList();

            return res;
        }

        public static Find.OrganizationCatalogViewModel ToCatalogViewModel(this Organization org)
        {
            var res = TinyMapper.Map<Find.OrganizationCatalogViewModel>(org);
            res.OrganizationId = org.Id;
            res.EventCategories = org.Directions.Select(d => d.EventDirection.Name).Distinct();
            res.EventTypes = org.Directions.Select(d => d.EventDirection?.Parent?.Name).Where(name => !string.IsNullOrEmpty(name)).Distinct();
            res.Uploads = org.Uploads.Select(u => u.Upload).Select(TinyMapper.Map<UploadViewModel>);

            return res;
        }

        public static OrganizationViewModel ToViewModel(this Organization org, bool isFavourite)
        {
            var res = TinyMapper.Map<OrganizationViewModel>(org);
            res.Lat = org.GeoLocation?.Y;
            res.Lon = org.GeoLocation?.X;
            res.IsFavourite = isFavourite;
            res.Directions = org.Directions.Select(x => x.EventDirection).Select(TinyMapper.Map<DirectionViewModel>);
            res.Photos = org.Uploads.Where(x => x.Upload.IsImage()).Select(x => TinyMapper.Map<UploadViewModel>(x.Upload));
            res.Videos = org.Uploads.Where(x => x.Upload.IsVideo()).Select(x => TinyMapper.Map<UploadViewModel>(x.Upload));

            return res;
        }

        public static ChildViewModel ToViewModel(this Child child)
        {
            var res = TinyMapper.Map<ChildViewModel>(child);
            res.Photos = child.Uploads.Where(x => x.Upload.IsImage()).Select(x => TinyMapper.Map<UploadViewModel>(x.Upload));
            res.Videos = child.Uploads.Where(x => x.Upload.IsVideo()).Select(x => TinyMapper.Map<UploadViewModel>(x.Upload));

            return res;
        }
    }
}
