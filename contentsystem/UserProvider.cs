using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Randomous.EntitySystem;

namespace Randomous.ContentSystem
{
    public class UserProvider : BaseEntityProvider<User, BasicUser, UserSearch> , IUserProvider
    {
        public UserProvider(ILogger<UserProvider> logger, IEntityProvider provider, IMapper mapper, Dictionary<Enum, string> keys) : 
            base(logger, provider, mapper, keys) 
        { 
            TransferData[Identifier.Email] = new EntityTransferData()
            {
                Assign = (u, s) => u.email = s,
                Retrieve = (u) => u.email,
                FieldLike = (s) => s.EmailLike
            };
            TransferData[Identifier.Password] = new EntityTransferData()
            {
                Assign = (u, s) => u.passwordHash = s,
                Retrieve = (u) => u.passwordHash,
                FieldLike = (s) => null
            };
        }

        protected override SystemType EntityType { get => SystemType.User; }
    }
}