using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiSqliteDemo
{
    [Table("Comment")]
    public class Comment
    {
        [PrimaryKey]
        [AutoIncrement]
        [Column("id")]

        public int Id { get; set; }
        [Column("postid")]
        public int PostId { get; set; }
        [Column("name")]
        public string Name { get; set; }
        [Column("email")]
        public string Email { get; set; }
        [Column("body")]
        public string Body { get; set; }
    }
}
