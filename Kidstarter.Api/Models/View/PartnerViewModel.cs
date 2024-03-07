using Kidstarter.Core.Models.Enums;

namespace Kidstarter.Api.Models.View
{
    public class PartnerViewModel
    {
        public string PartnerId { get; set; }

        public string FirstName { get; set; }

        public string SecondName { get; set; }

        public string PhoneNumber { get; set; }

        public string Email { get; set; }

        public PartnerStatus Status { get; set; }

        /// <summary>
        /// Юридическое лицо
        /// </summary>
        public string Entity { get; set; }

        /// <summary>
        /// ИНН
        /// </summary>
        public string TaxIdNumber { get; set; }

        /// <summary>
        /// ОГРН
        /// </summary>
        public string PrimaryStateNumber { get; set; }

        /// <summary>
        /// Юридический адрес
        /// </summary>
        public string LegalAddress { get; set; }

        /// <summary>
        /// Расчетный счет
        /// </summary>
        public string AccountNumber { get; set; }
    }
}
