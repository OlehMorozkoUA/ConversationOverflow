using ConversationOverflow.Attributes;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace ConversationOverflow.Dto
{
    public class AttachmentDto
    {
        [Required(ErrorMessage = "Please select a file.")]
        [DataType(DataType.Upload)]
        [MaxFileSize(5 * 1024 * 1024)]
        public IFormFile File { get; set; }
        [Required]
        public string FilePath { get; set; }
    }
}
