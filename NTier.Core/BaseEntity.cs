using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTier.Core
{
  public  class BaseEntity
    {
        private DateTime dateTime;
        [NotMapped]
        public DateTime UsedTime { get { this.dateTime = DateTime.Now; return dateTime; } set { } }
        public bool IsDeleted { get; set; }
    }
}
