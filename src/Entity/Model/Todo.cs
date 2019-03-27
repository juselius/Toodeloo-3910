using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Toodeloo.Entity
{
    [Table("todo", Schema = "dbo")]
    public partial class Todo
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("title")]
        [StringLength(200)]
        public string Title { get; set; }
        [Column("description")]
        [StringLength(2000)]
        public string Description { get; set; }
        [Column("priority")]
        public int? Priority { get; set; }
        [Column("due", TypeName = "date")]
        public DateTime? Due { get; set; }
    }
}
