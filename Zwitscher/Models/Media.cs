using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Zwitscher.Models
{
    public class Media
    {
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        [NotMapped] // This attribute tells Entity Framework to ignore this property during database operations
        public IFormFile File { get; set; }
    }
}
