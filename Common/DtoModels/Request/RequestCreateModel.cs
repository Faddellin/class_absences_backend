
using System.ComponentModel.DataAnnotations;

namespace Common.DtoModels.Request
{
    public class RequestCreateModel
    {
        [Required]
        public DateTime AbsenceDateFrom { get; set; }

        [Required]
        public DateTime AbsenceDateTo { get; set; }
        [Required]
        [MinLength(1)]
        public string Description { get; set; }
    }
}
