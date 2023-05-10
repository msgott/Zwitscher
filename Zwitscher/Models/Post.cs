namespace Zwitscher.Models
{
    public class Post
    {
        public Guid Id { get; set; }
        public Guid UserID { get; set; }
        public DateTime CreatedDate { get; set; }
        public string TextContent { get; set; }


        public virtual User User { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }
    }
}
