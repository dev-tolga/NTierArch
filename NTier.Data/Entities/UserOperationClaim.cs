using System;
using NTier.Core;

namespace NTier.Data.Entities
{
    public class UserOperationClaim : BaseEntity
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int OperationClaimId { get; set; }
    }
}
