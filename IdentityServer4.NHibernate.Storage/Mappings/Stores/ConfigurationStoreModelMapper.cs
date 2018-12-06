﻿using System;
using IdentityServer4.NHibernate.Storage.Entities;
using IdentityServer4.NHibernate.Storage.Options;
using NHibernate.Mapping.ByCode;

namespace IdentityServer4.NHibernate.Storage.Mappings.Stores
{
    internal class ConfigurationStoreModelMapper : ModelMapperBase
    {
        private readonly ConfigurationStoreOptions _options;

        public ConfigurationStoreModelMapper(ConfigurationStoreOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            BeforeMapClass += BeforeMapConfigurationStoreClass;
        }

        /// <summary>
        /// Sets table name and table's schema based on the rule that a the table's name is the same as the type's name.
        /// </summary>
        /// <remarks>
        /// The only exception to the rule is the "ApiResourceClaim" class, that has to be mapped to the "ApiClaims" table.
        /// </remarks>
        private void BeforeMapConfigurationStoreClass(IModelInspector modelInspector, Type type, IClassAttributesMapper classCustomizer)
        {
            TableDefinition tableDef = null;
            if (type == typeof(ApiResourceClaim))
            {
                tableDef = GetTableDefinition(nameof(_options.ApiClaim), _options);
            }
            else
            {
                tableDef = GetTableDefinition(type.Name, _options);
            }

            if (tableDef != null)
            {
                classCustomizer.Table(tableDef.Name);
                if (string.IsNullOrEmpty(tableDef.Schema))
                {
                    classCustomizer.Schema(_options.DefaultSchema);
                }
                else
                {
                    classCustomizer.Schema(tableDef.Schema);
                }
            }

            // Common mapping rule for IDs
            classCustomizer.Id(map =>
            {
                map.Column("Id");
                map.Generator(Generators.Native);
            });
        }
    }
}
