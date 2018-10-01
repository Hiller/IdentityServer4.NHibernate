﻿using System.Collections.Generic;

namespace IdentityServer4.NHibernate.Entities
{
    public class IdentityResource : EntityBase<int>
    {
        public virtual bool Enabled { get; set; } = true;
        public virtual string Name { get; set; }
        public virtual string DisplayName { get; set; }
        public virtual string Description { get; set; }
        public virtual bool Required { get; set; }
        public virtual bool Emphasize { get; set; }
        public virtual bool ShowInDiscoveryDocument { get; set; } = true;
        public virtual ISet<IdentityClaim> UserClaims { get; set; } = new HashSet<IdentityClaim>();
    }
}