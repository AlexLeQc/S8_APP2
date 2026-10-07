using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sanssoussi.Models
{
    [Table("Comments")]
    public class Comment
    {
        [Key]
        public string CommentId { get; set; }
        
        public string UserId { get; set; }
        
        [Column("Comment")]
        public string Text { get; set; }
    }
}
