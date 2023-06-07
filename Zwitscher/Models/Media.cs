using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Zwitscher.Models
{
    public class Media
    {
        public Guid Id { get; set; }
        public string FileName { get; set; } = "";
        public string FilePath { get; set; } = "";

        [NotMapped] // This attribute tells Entity Framework to ignore this property during database operations
        public IFormFile File { get; set; } = null!;

        //Relation to User (optional one to one)
        public User? User { get; set; }

        //Relation to Post (optional one to many)
        public Guid? PostId { get; set; }
        public Post? Post { get; set; }
    }
}
