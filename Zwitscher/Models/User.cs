using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Zwitscher.Models
{
    
    public class User
    {
        public Guid Id { get; set; }

        [Required]
        [DisplayName("Nachname")]
        [StringLength(50, MinimumLength =3 , ErrorMessage = "The name must be between 3 and 50 characters long.")]
        public string LastName { get; set; }

        [Required]
        [DisplayName("Vorname")]
        [StringLength(50, MinimumLength =3 , ErrorMessage = "The name must be between 3 and 50 characters long.")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50, MinimumLength =3 , ErrorMessage = "The username must be between 3 and 50 characters long.")]
        public string Username { get; set; }

        [Required]
        [DisplayName("Passwort")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,32}$", ErrorMessage = "Password must be 8 to 32 characters long and include at least one lowercase letter, one uppercase letter, and one digit.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DisplayName("Geburtstag")]
        [DataType(DataType.Date)]
        public DateTime Birthday { get; set; }

        [DisplayName("Biographie")]
        public string? Biography { get; set; }

        [DisplayName("Gesperrt")]
        public bool isLocked { get; set; } = false;

        [DisplayName("Erstellungsdatum")]
        [DataType(DataType.Date)]
        public DateTime CreatedDate { get; set; } = DateTime.Now;


        //Relation to Role (Required one to many)
        public Guid? RoleID { get; set; }
        [DisplayName("Rolle")]
        public Role? Role { get; set; } = null!;

        //Relation to Media (Optional one to one)
        public Guid? MediaId { get; set; }
        [DisplayName("Profilbild")]
        public Media? ProfilePicture { get; set; }

        //Relation to Follower (optional many to many)
        [DisplayName("Folgt")]
        public List<User> Following { get; } = new();
        [DisplayName("Follower")]
        public List<User> FollowedBy { get; } = new();

        //Relation to Posts (required one to many)
        public ICollection<Post> Posts { get; } = new List<Post>();

        //Relation to Comments (required one to many)
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();

        //Relation to Vote (required one to many)
        public ICollection<Vote> Votes { get; set; } = new List<Vote>();

        public override string ToString()
        {
            return FirstName + " " + LastName;
        }
    }
}
