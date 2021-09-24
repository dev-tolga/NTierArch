﻿using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using NTier.Core.UnitOfWork;
using NTier.Core.Utils.Results;
using NTier.Data;
using NTier.Data.Entities;
using NTier.Utils.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTier.Service.BlogService
{
    public class BlogService : IBlogService
    {
        private readonly IUnitOfWork<NTierDBContext> _appDbContext;
        private readonly IMemoryCache _memoryCache;
        private readonly IMapper _mapper;

        public BlogService(IUnitOfWork<NTierDBContext> appDbContext,IMapper mapper,IMemoryCache memoryCache)
        {
            _mapper = mapper;
            _memoryCache = memoryCache;
            _appDbContext = appDbContext;
        }
        public async Task<IDataResult<List<Blog>>> GetAllBlogs()
        {
           
            var dataResult = new  SuccessDataResult<List<Blog>>();

            try
            {
                if (_memoryCache.TryGetValue(Constants.BlogList, out List<Blog> cachedResult))
                {
                  
                    return new SuccessDataResult<List<Blog>>(cachedResult,"from Cache");
                }

               
                //EF repo
                dataResult = new SuccessDataResult<List<Blog>>( await _appDbContext.GetRepository<Blog>().TableNoTracking.ToListAsync());

                //var bloglistResult = _mapper.Map<List<BlogModel>>(blogList);
                _memoryCache.Set(Constants.BlogList, dataResult.Data, DateTimeOffset.UtcNow.AddMinutes(30));

                //result.Data = bloglistResult;
                //result.Message = "from app service";
            }
            catch (Exception ex)
            {
                //result.IsSuccess = false;
                //result.Message = ex.Message;
            }


            return dataResult;
        }

       
    }
}