using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTier.Core.Utils.Results
{
    public interface IDataResult<out T> : IResults
    {
        T Data { get; }
    }
}
