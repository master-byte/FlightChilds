using System.Collections.Generic;

using Kidstarter.Api.Endpoints.Admin.EventDirections;
using Kidstarter.Api.Endpoints.Admin.Organizations;
using Kidstarter.Api.Endpoints.Common.Organizations.Models;
using Kidstarter.Api.Endpoints.Parent.Children.Models;
using Kidstarter.Api.Models.View;
using Kidstarter.Core.Models.EF;

using Nelibur.ObjectMapper;

namespace Kidstarter.Api.Extensions
{
    internal sealed class TinyMapperConfig
    {
        public static void Register()
        {
            TinyMapper.Bind<OrganizationBusinessHours, BusinessHoursViewModel>();
            TinyMapper.Bind<List<OrganizationBusinessHours>, List<BusinessHoursViewModel>>();

            TinyMapper.Bind<EventDirection, DirectionViewModel>();

            TinyMapper.Bind<Partner, PartnerViewModel>(
                config =>
                    {
                        config.Bind(s => s.User.Email, t => t.Email);
                        config.Bind(s => s.User.PhoneNumber, t => t.PhoneNumber);
                    });

            TinyMapper.Bind<UploadThumbnail, UploadThumbnailViewModel>();
            TinyMapper.Bind<List<UploadThumbnail>, List<UploadThumbnailViewModel>>();
            TinyMapper.Bind<Upload, UploadViewModel>(config =>
                {
                    config.Bind(s => s.Thumbnails, t => t.Thumbnails);
                });

            TinyMapper.Bind<Organization, OrganizationViewModel>(
                config =>
                    {
                        config.Ignore(s => s.Directions);
                        config.Ignore(s => s.Uploads);
                    });

            TinyMapper.Bind<BusinessHoursCreateModel, OrganizationBusinessHours>();
            TinyMapper.Bind<OrganizationCreateModel, Organization>(
                config =>
                    {
                        config.Ignore(x => x.UploadIds);
                        config.Ignore(x => x.Directions);
                    });

            TinyMapper.Bind<Organization, Find.OrganizationCatalogViewModel>(
                config =>
                    {
                        config.Ignore(s => s.Uploads);
                    });

            TinyMapper.Bind<Tariff, TariffViewModel>(
                config =>
                    {
                        config.Bind(s => s.Price, d => d.OriginalPrice);
                    });

            TinyMapper.Bind<EventDirectionCreateModel, EventDirection>();

            BindChildren();
        }

        private static void BindChildren()
        {
            TinyMapper.Bind<Child, ChildViewModel>();
        }
    }
}