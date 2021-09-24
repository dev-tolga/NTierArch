using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTier.Core.Utils.Results
{
   public interface IResults
    {
        bool Success { get; }
        string Message { get; }
    }
}
