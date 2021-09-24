using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTier.Data.Entities
{
    public class Post
    {
        [Key]
        public int Id { get; set; }

        [StringLength(500)]
        public string Title { get; set; }
        public string Content { get; set; }

        #region Navigation Properties

        public int BlogId { get; set; }
        public virtual Blog Blog { get; set; }

        #endregion
    }
}
