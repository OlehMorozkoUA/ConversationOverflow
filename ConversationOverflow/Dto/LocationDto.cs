using System.ComponentModel.DataAnnotations;

namespace ConversationOverflow.Dto
{
    public class LocationDto
    {
        [Display(Name = "Country")]
        [DataType(DataType.Text)]
        public string Country { get; set; }
        [DataType(DataType.Text)]
        [Display(Name = "Region")]
        public string Region { get; set; }
        [DataType(DataType.Text)]
        [Display(Name = "Address")]
        public string Address { get; set; }
        [DataType(DataType.Text)]
        [Display(Name = "Postcode")]
        public int Postcode { get; set; }
    }
}
