﻿namespace IdentityServer4.NHibernate.Mappings.Storage.Configuration
{
    using IdentityServer4.NHibernate.Entities;
    using global::NHibernate.Mapping.ByCode;
    using global::NHibernate.Mapping.ByCode.Conformist;

    internal class IdentityResourceMap : ClassMapping<IdentityResource>
    {
        public IdentityResourceMap()
        {
            Id(p => p.ID, map => 
            {
                map.Generator(Generators.Native);
                map.Column("Id");
            });

            Property(p => p.Name, map => 
            {
                map.Length(200);
                map.NotNullable(true);
                map.UniqueKey("UK_IdentityResource_Name");
            });

            Property(p => p.DisplayName, map => map.Length(200));
            Property(p => p.Description, map => map.Length(1000));
            Property(p => p.Required);
            Property(p => p.ShowInDiscoveryDocument, map => 
            {
                map.NotNullable(true);
            });
            Property(p => p.Emphasize);
            Property(p => p.Enabled);

            Set<IdentityClaim>("_userClaims", map => 
            {
                map.Key(fk =>
                {
                    fk.Column("IdentityResourceId");
                    fk.NotNullable(true);
                    fk.ForeignKey("FK_IdentityClaims_IdentityResource");
                });
                map.Fetch(CollectionFetchMode.Join);
                map.Access(Accessor.Field);
                map.Cascade(Cascade.All.Include(Cascade.DeleteOrphans));
            },
                r => r.OneToMany(m => m.Class(typeof(IdentityClaim)))
            );
        }
    }
}
