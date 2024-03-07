using System;
using System.ComponentModel.DataAnnotations;

using Kidstarter.Core.Models.Enums;

namespace Kidstarter.Api.Models.Create
{
    public sealed class UserCreateModel
    {
        [MaxLength(250)]
        public string UserValueName { get; set; }

        [RegularExpression(@"^(?:\+7)\d{10}$")]
        public string PhoneNumber { get; set; }

        [Range(typeof(DateTime), "1/1/1990", "1/1/2100")]
        public DateTime? BirthDate { get; set; }

        [EmailAddress]
        [MaxLength(200)]
        public string Email { get; set; }

        [MaxLength(100)]
        public string Role { get; set; }

        [MaxLength(125)]
        public string FirstName { get; set; }

        [MaxLength(125)]
        public string SecondName { get; set; }

        [EnumDataType(typeof(Gender))]
        public Gender? Gender { get; set; }

        [EnumDataType(typeof(HairColor))]
        public HairColor? HairColor { get; set; }

        [EnumDataType(typeof(EyeColor))]
        public EyeColor? EyeColors { get; set; }

        [Range(40, 230)]
        public uint? Height { get; set; }

        [EnumDataType(typeof(HairLength))]
        public HairLength? HairLength { get; set; }

        [EnumDataType(typeof(AppearanceType))]
        public AppearanceType? AppearanceType { get; set; }

        [Range(20, 50)]
        [Obsolete]
        public uint? ShoeSize { get; set; }

        [Obsolete]
        [EnumDataType(typeof(FaceShape))]
        public FaceShape? FaceShape { get; set; }

        [MaxLength(5000)]
        public string About { get; set; }

        [MaxLength(5000)]
        public string Experience { get; set; }

        [RegularExpression(@"^(?:\+7)\d{10}$")]
        public string AdditionalPhoneNumber { get; set; }

        [MaxLength(1000)]
        public string SocialNetworks { get; set; }

        [MaxLength(125)]
        public string City { get; set; }

        [MaxLength(125)]
        public string Country { get; set; }

        [MaxLength(250)]
        public string Address { get; set; }

        public bool? IsActive { get; set; }

        [MaxLength(250)]
        public string DeviceToken { get; set; }

        [MaxLength(100)]
        public string TimeZone { get; set; }
    }
}