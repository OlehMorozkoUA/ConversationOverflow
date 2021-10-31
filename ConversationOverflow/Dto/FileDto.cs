using ConversationOverflow.Attributes;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace ConversationOverflow.Dto
{
    public class FileDto
    {
        [Required(ErrorMessage = "Please select a file.")]
        [DataType(DataType.Upload)]
        [MaxFileSize(5 * 1024 * 1024)]
        [AllowedExtensions(new string[] { ".jpg", ".png" })]
        public IFormFile Image { get; set; }
        [Required]
        public string FilePath { get; set; }
    }
}
