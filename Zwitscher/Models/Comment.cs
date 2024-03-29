﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Zwitscher.Models
{
    public class Comment
    {
        public Guid Id { get; set; }
        
        [DisplayName("Erstellungsdatum")]
        [DataType(DataType.Date)]
        public DateTime CreatedDate { get; set; }
        public string CommentText { get; set; } = "";


        //Relation to User (required one to many)
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        //Relation to Post (optional one to many)
        public Guid? PostId { get; set; }
        public Post? Post { get; set; } = null!;

        //Relation to Comment (optional one to many)
        public Guid? commentsCommentId { get; set; }
        public Comment? commentsComment { get; set; } = null!;
        public List<Comment> commentedBy { get; set; } = new();
    }
}
