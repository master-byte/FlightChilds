using System;
using System.Linq;

using AutoMapper;

using Kidstarter.Api.Models.Update;
using Kidstarter.Api.Models.View;
using Kidstarter.Core.Models.EF;
using Kidstarter.Core.Models.EF.Portfolios;

namespace Kidstarter.Api.Extensions
{
    internal static class UserExtensions
    {
        public static UserViewModel MapSplitEntitiesIntoUser(
            this IMapper mapper,
            UserViewModel viewModel,
            Child defaultChild,
            FilmingPortfolio filmingPortfolio)
        {
            if (defaultChild == null)
            {
                return viewModel;
            }

            viewModel.FirstName = defaultChild.FirstName;
            viewModel.SecondName = defaultChild.SecondName;
            viewModel.Gender = defaultChild.Gender;
            viewModel.BirthDate = defaultChild.BirthDate;
            viewModel.Media = mapper.Map<UploadViewModel[]>(defaultChild.Uploads.Select(x => x.Upload));
            viewModel.Photos = viewModel.Media.Where(x => x.ContentType.StartsWith("image", StringComparison.OrdinalIgnoreCase)).ToArray();
            viewModel.Videos = viewModel.Media.Where(x => x.ContentType.StartsWith("video", StringComparison.OrdinalIgnoreCase)).ToArray();

            if (filmingPortfolio == null)
            {
                return viewModel;
            }

            viewModel.HairColor = filmingPortfolio.HairColor;
            viewModel.HairLength = filmingPortfolio.HairLength;
            viewModel.Height = filmingPortfolio.Height;
            viewModel.EyeColors = filmingPortfolio.EyeColors;
            viewModel.AppearanceType = filmingPortfolio.AppearanceType;
            viewModel.Experience = filmingPortfolio.Experience;
            viewModel.About = filmingPortfolio.About;
            viewModel.SocialNetworks = filmingPortfolio.SocialNetworks;

            return viewModel;
        }

        public static UserViewModel MapSplitEntitiesIntoUser(
            this IMapper mapper,
            FilmingPortfolio filmingPortfolio)
        {
            var defaultChild = filmingPortfolio.Child;
            var viewModel = mapper.Map<UserViewModel>(filmingPortfolio.Child.Parent);

            viewModel.FirstName = defaultChild.FirstName;
            viewModel.SecondName = defaultChild.SecondName;
            viewModel.Gender = defaultChild.Gender;
            viewModel.BirthDate = defaultChild.BirthDate;
            viewModel.Media = mapper.Map<UploadViewModel[]>(defaultChild.Uploads.Select(x => x.Upload));
            viewModel.Photos = viewModel.Media.Where(x => x.ContentType.StartsWith("image", StringComparison.OrdinalIgnoreCase)).ToArray();
            viewModel.Videos = viewModel.Media.Where(x => x.ContentType.StartsWith("video", StringComparison.OrdinalIgnoreCase)).ToArray();

            viewModel.HairColor = filmingPortfolio.HairColor;
            viewModel.HairLength = filmingPortfolio.HairLength;
            viewModel.Height = filmingPortfolio.Height;
            viewModel.EyeColors = filmingPortfolio.EyeColors;
            viewModel.AppearanceType = filmingPortfolio.AppearanceType;
            viewModel.Experience = filmingPortfolio.Experience;
            viewModel.About = filmingPortfolio.About;
            viewModel.SocialNetworks = filmingPortfolio.SocialNetworks;

            return viewModel;
        }

        public static Action<User> GetUpdateAction(this UserUpdateModel updateModel)
        {
            void UpdateAction(User user)
            {
                if (updateModel.UserValueName != null)
                {
                    user.UserValueName = updateModel.UserValueName;
                }

                if (updateModel.Email != null)
                {
                    user.Email = updateModel.Email;
                }

                if (updateModel.Age != null)
                {
                    user.Age = (int?)updateModel.Age;
                }

                if (updateModel.Address != null)
                {
                    user.Address = updateModel.Address;
                }

                if (updateModel.City != null)
                {
                    user.City = updateModel.City;
                }

                if (updateModel.Country != null)
                {
                    user.Country = updateModel.Country;
                }

                if (updateModel.AdditionalPhoneNumber != null)
                {
                    user.AdditionalPhoneNumber = updateModel.AdditionalPhoneNumber;
                }
            }

            return UpdateAction;
        }

        public static Action<User> GetUpdateAction(this AdminUserUpdateModel updateModel)
        {
            void UpdateAction(User user)
            {
                if (updateModel.UserValueName != null)
                {
                    user.UserValueName = updateModel.UserValueName;
                }

                if (updateModel.Email != null)
                {
                    user.Email = updateModel.Email;
                }

                if (updateModel.Age != null)
                {
                    user.Age = (int?)updateModel.Age;
                }

                if (updateModel.Address != null)
                {
                    user.Address = updateModel.Address;
                }

                if (updateModel.City != null)
                {
                    user.City = updateModel.City;
                }

                if (updateModel.Country != null)
                {
                    user.Country = updateModel.Country;
                }

                if (updateModel.AdditionalPhoneNumber != null)
                {
                    user.AdditionalPhoneNumber = updateModel.AdditionalPhoneNumber;
                }

                if (updateModel.ProducerCenterId != null)
                {
                    user.OrganizationId = updateModel.ProducerCenterId?.ToString();
                }

                if (updateModel.IsActive != null)
                {
                    user.IsActive = updateModel.IsActive;
                }
            }

            return UpdateAction;
        }
    }
}