using System.ComponentModel.DataAnnotations;

namespace Zwitscher.Models
{
    public class Post
    {
        public Guid Id { get; set; }
        public Guid UserID { get; set; }
        [DataType(DataType.Date)]
        public DateTime CreatedDate { get; set; }
        [StringLength(281)]
        public string TextContent { get; set; } = "";


        public virtual User User { get; set; }
        
        public virtual ICollection<Media>? Media { get; set; }

        public virtual ICollection<Comment>? Comments { get; set; }
    }
}
