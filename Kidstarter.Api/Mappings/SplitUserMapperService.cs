using System;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;

using Kidstarter.Api.Extensions;
using Kidstarter.Api.Models.View;
using Kidstarter.BusinessLogic.Requests.Common.Users;
using Kidstarter.BusinessLogic.Requests.Parent.Portfolios;
using Kidstarter.Core.Models.EF;
using Kidstarter.Core.Models.EF.Portfolios;
using Kidstarter.Core.Models.Enums;
using Kidstarter.Core.Workflow;

using MediatR;

namespace Kidstarter.Api.Mappings
{
    public class SplitUserMapperService
    {
        private readonly IMapper mapper;

        private readonly IMediator mediator;

        private readonly IUserProvider userProvider;

        public SplitUserMapperService(IMapper mapper, IMediator mediator, IUserProvider userProvider)
        {
            this.mapper = mapper;
            this.mediator = mediator;
            this.userProvider = userProvider;
        }

        public async Task<(TUserViewModel ViewModel, User User)> Map<TUserViewModel>(GetUserProfileRequest request)
            where TUserViewModel : UserViewModel
        {
            var user = await this.mediator.Send(request);
            var viewModel = this.mapper.Map<TUserViewModel>(user);
            if (viewModel == null)
            {
                return (null, null);
            }

            var defaultChild = user.Children.FirstOrDefault(x => x.IsDefault);
            FilmingPortfolio filmingPortfolio = null;
            if (defaultChild != null)
            {
                using var scope = this.userProvider.CreateScope(new(Guid.Parse(user.Id), UserRoleTypeEnum.Parent));
                var portfolios = await this.mediator.Send(new GetPortfoliosRequest(defaultChild.ChildId));
                filmingPortfolio = portfolios.FirstOrDefault(x => x.PortfolioType == "Casting") as FilmingPortfolio;
            }

            this.mapper.MapSplitEntitiesIntoUser(viewModel, defaultChild, filmingPortfolio);

            return (viewModel, user);
        }
    }
}