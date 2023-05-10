using System.ComponentModel.DataAnnotations;

namespace Zwitscher.Models
{
    public class Comment
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid PostId { get; set; }
        [DataType(DataType.Date)]
        public DateTime CreatedDate { get; set; }
        public string CommentText { get; set; } = "";


        public virtual User? User { get; set; }

    }
}
