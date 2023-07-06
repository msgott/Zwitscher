using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Zwitscher.Models
{
    public class Post
    {
        public Guid Id { get; set; }
        [DataType(DataType.Date)]
        [DisplayName("Erstellt")]
        public DateTime CreatedDate { get; set; }
        [StringLength(281)]
        [DisplayName("Text")]
        public string TextContent { get; set; } = "";
        [DisplayName("Öffentlich")]
        public bool IsPublic { get; set; } = true;

        //Relation to User (required one to many)
        public Guid UserId { get; set; }
        
        public User User { get; set; } = null!;

        //Relation to Media (optional one to many)
        [DisplayName("Medien")]
        public ICollection<Media> Media { get; set; } = new List<Media>();

        //Relation to Comments (required one to many)
        [DisplayName("Kommentare")]
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();

        //Relation to Vote (required one to many)
        public ICollection<Vote> Votes { get; set; } = new List<Vote>();


        //Relation to Post (optional one to one)
        [DisplayName("RezwitscherID")]
        public Guid? retweetsID { get; set; }
        [DisplayName("Rezwitschert")]
        public Post? retweets { get; set; }
    }
}
