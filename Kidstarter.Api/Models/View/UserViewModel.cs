using System;

using Kidstarter.Api.Models.Base;
using Kidstarter.Core.Models.Enums;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Kidstarter.Api.Models.View
{
    public class UserViewModel : ModelWithMediaBase
    {
        public Guid Id { get; set; }

        public int UniqueValue { get; set; }

        public string UserName { get; set; }

        public string UserValueName { get; set; }

        public string PhoneNumber { get; set; }

        public string Email { get; set; }

        public UserRoleTypeEnum? RoleType { get; set; }

        public DateTime? BirthDate { get; set; }

        public int? Age { get; set; }

        public string Address { get; set; }

        public string City { get; set; }

        public string Country { get; set; }

        public string AdditionalPhoneNumber { get; set; }

        [JsonProperty("photo")]
        public UploadViewModel[] Media { get; set; }

        public Guid? DefaultChildId { get; set; }

        public string FirstName { get; set; }

        public string SecondName { get; set; }

        public string Experience { get; set; }

        public string About { get; set; }

        public string SocialNetworks { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public Gender? Gender { get; set; }

        public HairColor? HairColor { get; set; }

        public HairLength? HairLength { get; set; }

        public EyeColor? EyeColors { get; set; }

        [Obsolete]
        public FaceShape? FaceShape { get; set; }

        public AppearanceType? AppearanceType { get; set; }

        public int? Height { get; set; }

        [Obsolete]
        public int? ShoeSize { get; set; }

        public bool IsPro { get; set; }

        public bool IsActive { get; set; }

        public string ProducerCenterId { get; set; }

        public string OrganizationId { get; set; }

        public bool AcceptedUserAgreement { get; set; }
    }
}