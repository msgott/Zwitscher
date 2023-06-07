using System.ComponentModel.DataAnnotations;

namespace Zwitscher.Models
{
    public class Post
    {
        public Guid Id { get; set; }
        [DataType(DataType.Date)]
        public DateTime CreatedDate { get; set; }
        [StringLength(281)]
        public string TextContent { get; set; } = "";

        //Relation to User (required one to many)
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        //Relation to Media (optional one to many)
        public ICollection<Media> Media { get; set; } = new List<Media>();

        //Relation to Comments (required one to many)
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}
