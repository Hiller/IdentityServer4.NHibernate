﻿namespace IdentityServer4.NHibernate.Entities
{
    public class ClientScope : EntityBase<int>
    {
        public string Scope { get; set; }
        public virtual Client Client { get; set; }
    }
}