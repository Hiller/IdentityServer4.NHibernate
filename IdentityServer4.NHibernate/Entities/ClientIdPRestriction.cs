﻿namespace IdentityServer4.NHibernate.Entities
{
    public class ClientIdPRestriction : EntityBase<int>
    {
        public virtual string Provider { get; set; }
        public virtual Client Client { get; set; }
    }
}