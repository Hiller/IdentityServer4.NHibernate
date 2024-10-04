using System.Collections.Generic;
using System.Security.Claims;
using AutoMapper;
using IdentityServer4.Models;

namespace IdentityServer4.NHibernate.Mappings.Entities
{
    /// <summary>
    /// Entity to model mapping (and vice-versa) for clients.
    /// </summary>
    internal class ClientStoreMappingProfile : Profile
    {
        public ClientStoreMappingProfile()
        {
            CreateMap<NHibernate.Entities.ClientProperty, KeyValuePair<string, string>>()
                .ReverseMap();

            CreateMap<NHibernate.Entities.Client, Models.Client>()
                .ForMember(dst => dst.ProtocolType, opt => opt.Condition(src => src != null))
                .ForMember(
                    dst => dst.AllowedIdentityTokenSigningAlgorithms, 
                    opt => opt.ConvertUsing(AllowedSigningAlgorithmsConverter.Instance, src => src.AllowedIdentityTokenSigningAlgorithms))
                .ReverseMap()
                    .ForMember(
                        dst => dst.AllowedIdentityTokenSigningAlgorithms, 
                        opt => opt.ConvertUsing(AllowedSigningAlgorithmsConverter.Instance, src => src.AllowedIdentityTokenSigningAlgorithms));

            CreateMap<NHibernate.Entities.ClientGrantType, string>()
#if NET6_0_OR_GREATER
                 .ConstructUsing(src => src.GrantType) 
#else
                 .ConstructUsing(src => src.GrantType)
#endif
                .ReverseMap()
                    .ForMember(dst => dst.GrantType, opt => opt.MapFrom(src => src));

            CreateMap<NHibernate.Entities.ClientRedirectUri, string>()
#if NET6_0_OR_GREATER
                 .ConstructUsing(src => src.RedirectUri) 
#else
                 .ConstructUsing(src => src.RedirectUri)
#endif
                .ReverseMap()
                    .ForMember(dst => dst.RedirectUri, opt => opt.MapFrom(src => src));

            CreateMap<NHibernate.Entities.ClientPostLogoutRedirectUri, string>()
#if NET6_0_OR_GREATER
                 .ConstructUsing(src => src.PostLogoutRedirectUri) 
#else
                 .ConstructUsing(src => src.PostLogoutRedirectUri)
#endif
                .ReverseMap()
                    .ForMember(dst => dst.PostLogoutRedirectUri, opt => opt.MapFrom(src => src));

            CreateMap<NHibernate.Entities.ClientScope, string>()
#if NET6_0_OR_GREATER
                 .ConstructUsing(src => src.Scope) 
#else
                 .ConstructUsing(src => src.Scope)
#endif
                .ReverseMap()
                    .ForMember(dst => dst.Scope, opt => opt.MapFrom(src => src));

            CreateMap<NHibernate.Entities.ClientSecret, Secret>(MemberList.Destination)
                .ForMember(dst => dst.Type, opt => opt.Condition(src => src != null))
                .ReverseMap();

            CreateMap<NHibernate.Entities.ClientIdPRestriction, string>()
#if NET6_0_OR_GREATER
                 .ConstructUsing(src => src.Provider) 
#else
                 .ConstructUsing(src => src.Provider)
#endif
                .ReverseMap()
                    .ForMember(dst => dst.Provider, opt => opt.MapFrom(src => src));

            CreateMap<NHibernate.Entities.ClientClaim, ClientClaim>(MemberList.None)
#if NET6_0_OR_GREATER
                 .ConstructUsing(src => new ClientClaim(src.Type, src.Value, ClaimValueTypes.String)) 
#else
                 .ConstructUsing(src => new ClientClaim(src.Type, src.Value, ClaimValueTypes.String))
#endif
                .ReverseMap();

            CreateMap<NHibernate.Entities.ClientCorsOrigin, string>()
#if NET6_0_OR_GREATER
                 .ConstructUsing(src => src.Origin) 
#else
                 .ConstructUsing(src => src.Origin)
#endif
                .ReverseMap()
                    .ForMember(dst => dst.Origin, opt => opt.MapFrom(src => src));
        }
    }
}
