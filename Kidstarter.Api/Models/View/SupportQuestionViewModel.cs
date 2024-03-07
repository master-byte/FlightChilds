using System;

using Kidstarter.Core.Models.Enums;

namespace Kidstarter.Api.Models.View
{
    public sealed class SupportQuestionViewModel
    {
        public string Id { get; set; }

        public string Text { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public SupportQuestionStatus Status { get; set; }
    }
}