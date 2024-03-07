using System;
using System.ComponentModel.DataAnnotations;

using Kidstarter.Core.Models.Enums;

namespace Kidstarter.Api.Models.Update
{
    public class UserUpdateModel
    {
        [MaxLength(250)]
        public string UserValueName { get; set; }

        [EmailAddress]
        [MaxLength(200)]
        public string Email { get; set; }

        [Range(typeof(DateTime), "1/1/1900", "1/1/2100")]
        public DateTime? BirthDate { get; set; }

        [Range(0, 24)]
        public uint? Age { get; set; }

        [MaxLength(250)]
        public string Address { get; set; }

        [MaxLength(125)]
        public string City { get; set; }

        [MaxLength(125)]
        public string Country { get; set; }

        public string AdditionalPhoneNumber { get; set; }

        [MaxLength(125)]
        public string FirstName { get; set; }

        [MaxLength(125)]
        public string SecondName { get; set; }

        [MaxLength(5000)]
        public string Experience { get; set; }

        [MaxLength(5000)]
        public string About { get; set; }

        [MaxLength(1000)]
        public string SocialNetworks { get; set; }

        [EnumDataType(typeof(Gender))]
        public Gender? Gender { get; set; }

        [EnumDataType(typeof(HairColor))]
        public HairColor? HairColor { get; set; }

        [EnumDataType(typeof(HairLength))]
        public HairLength? HairLength { get; set; }

        [EnumDataType(typeof(EyeColor))]
        public EyeColor? EyeColors { get; set; }

        [EnumDataType(typeof(AppearanceType))]
        public AppearanceType? AppearanceType { get; set; }

        [Range(40, 230)]
        public uint? Height { get; set; }
    }
}