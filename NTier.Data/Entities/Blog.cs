using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTier.Data.Entities
{
    public class Blog
    {
        public Blog()
        {
            Posts = new HashSet<Post>();
        }

        [Key]
        public int Id { get; set; }

        [StringLength(1000)]
        public string Url { get; set; }


        #region Navigation Properties

        public ICollection<Post> Posts { get; set; }
        public string UserId { get; set; }
      

        #endregion
    }
}
