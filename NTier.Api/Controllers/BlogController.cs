using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NTier.Data.Entities;
using NTier.Service.BlogService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NTier.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogController : ControllerBase
    {
        private readonly IBlogService _blogService;

        public BlogController(IBlogService blogService)
        {
            _blogService = blogService;
        }

        [HttpGet("blogs")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllBlogs()
        {
            var result = await _blogService.GetAllBlogs();

            if (result.Success)
            {
                return Ok(result.Data);
            }

            return BadRequest(result.Message);

        }

        [HttpPost("add")]
        public IActionResult AddBlog(Blog blog)
        {
            var result =  _blogService.AddBlog(blog);
            if (result.Success)
            {
                return Ok(result.Message);
            }
            return BadRequest(result.Message);

        }
    }
}
