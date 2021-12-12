using System.ComponentModel.DataAnnotations;

namespace Services.Classes
{
    public class ExistPrivateGroupDto
    {
        [Required]
        public int GroupId { get; set; }
        [Required]
        public bool Result { get; set; }
    }
}
