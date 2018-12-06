﻿using System.Collections.Generic;

namespace IdentityServer4.NHibernate.Storage.Entities
{
    public class ApiScope : EntityBase<int>
    {
        public virtual string Name { get; set; }
        public virtual string DisplayName { get; set; }
        public virtual string Description { get; set; }
        public virtual bool Required { get; set; }
        public virtual bool Emphasize { get; set; }
        public virtual bool ShowInDiscoveryDocument { get; set; } = true;
        public virtual ISet<ApiScopeClaim> UserClaims { get; set; } = new HashSet<ApiScopeClaim>();
    }
}