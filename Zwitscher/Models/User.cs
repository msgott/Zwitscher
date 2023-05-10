using System.ComponentModel.DataAnnotations;

namespace Zwitscher.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Username { get; set; }
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
