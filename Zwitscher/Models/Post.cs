﻿using System.ComponentModel.DataAnnotations;

namespace Zwitscher.Models
{
    public class Post
    {
        public Guid Id { get; set; }
        public Guid UserID { get; set; }
        [DataType(DataType.Date)]
        public DateTime CreatedDate { get; set; }
        public string TextContent { get; set; }


        public virtual User User { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }
    }
}
