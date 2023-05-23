using System.ComponentModel.DataAnnotations;

namespace Zwitscher.Models
{
    public class User
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(50, MinimumLength =3 , ErrorMessage = "The name must be between 3 and 50 characters long.")]
        public string LastName { get; set; }

        [Required]
        [StringLength(50, MinimumLength =3 , ErrorMessage = "The name must be between 3 and 50 characters long.")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50, MinimumLength =3 , ErrorMessage = "The username must be between 3 and 50 characters long.")]
        public string Username { get; set; }

        [Required]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,32}$", ErrorMessage = "Password must be 8 to 32 characters long and include at least one lowercase letter, one uppercase letter, and one digit.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Date)]
        public DateTime Birthday { get; set; }

        public string? Biography { get; set; }

        public bool isLocked { get; set; } = false;

        public Guid RoleID { get; set; }

        public virtual Role Role { get; set; }

        public virtual ICollection<User> Following { get; set; }



    }
}
