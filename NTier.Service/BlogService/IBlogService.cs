using NTier.Core.Utils.Results;
using NTier.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTier.Service.BlogService
{
   public interface IBlogService
    {
        Task<IDataResult<List<Blog>>> GetAllBlogs();
        IResults AddBlog(Blog blog);
    }
}
