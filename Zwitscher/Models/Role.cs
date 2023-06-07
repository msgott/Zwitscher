namespace Zwitscher.Models
{
    public class Role
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = "";

        //Relation to User (One to many)
        public ICollection<User> Users { get;} = new List<User>();
    }
}
