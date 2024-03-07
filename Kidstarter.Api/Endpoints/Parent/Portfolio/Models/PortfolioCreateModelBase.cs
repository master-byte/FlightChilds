using System;

using JsonSubTypes;

using Kidstarter.Api.Binders;

using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;

namespace Kidstarter.Api.Endpoints.Parent.Portfolio.Models
{
    [ModelBinder(BinderType = typeof(PortfolioCreateModelBinder))]
    [JsonConverter(typeof(JsonSubtypes), nameof(PortfolioType))]
    [JsonSubtypes.KnownSubType(typeof(FilmingPortfolioCreateModel), "Casting")]
    public abstract class PortfolioCreateModelBase
    {
        public Guid ChildId { get; set; }

        public string PortfolioType { get; set; }
    }
}