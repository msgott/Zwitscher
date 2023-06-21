using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Zwitscher.Models
{
    public class Vote
    {
        public Guid Id { get; set; }
        [Required]
        [DisplayName("ist Upvote")]
        public bool isUpVote { get; set; } = true;

        //Relation to User (required one to many)
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        //Relation to Post (required one to many)
        public Guid PostId { get; set; }
        public Post Post { get; set; } = null!;
    }
}
