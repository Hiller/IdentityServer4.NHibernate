using AutoMapper;
using IdentityServer4.NHibernate.Entities;
using System.Collections.Generic;

namespace IdentityServer4.NHibernate.Mappings.Entities
{
    /// <summary>
    /// Defines entity/model mapping for API resources and Identity resources.
    /// </summary>
    internal class ResourceStoreMappingProfile : Profile
    {
        public ResourceStoreMappingProfile()
        {
            CreateMap<ApiResourceProperty, KeyValuePair<string, string>>()
                .ReverseMap();

            CreateMap<ApiResource, Models.ApiResource>(MemberList.Destination)
#if NET6_0_OR_GREATER
                 .ConstructUsing(src => new Models.ApiResource()) 
#else
                 .ConstructUsing(src => new Models.ApiResource())
#endif
                .ForMember(dst => dst.ApiSecrets, opt => opt.MapFrom(src => src.Secrets))
                .ForMember(
                    dst => dst.AllowedAccessTokenSigningAlgorithms, 
                    opt => opt.ConvertUsing(AllowedSigningAlgorithmsConverter.Instance, src => src.AllowedAccessTokenSigningAlgorithms))
                .ReverseMap()
                    .ForMember(
                        dst => dst.AllowedAccessTokenSigningAlgorithms, 
                        opt => opt.ConvertUsing(AllowedSigningAlgorithmsConverter.Instance, src => src.AllowedAccessTokenSigningAlgorithms));

            CreateMap<ApiResourceClaim, string>()
#if NET6_0_OR_GREATER
                 .ConstructUsing(x => x.Type) 
#else
                 .ConstructUsing(x => x.Type)
#endif
                .ReverseMap()
                    .ForMember(dst => dst.Type, opt => opt.MapFrom(src => src));

            CreateMap<ApiResourceSecret, Models.Secret>(MemberList.Destination)
                .ForMember(dst => dst.Type, opt => opt.Condition(src => src != null))
                .ReverseMap();

            CreateMap<ApiResourceScope, string>()
#if NET6_0_OR_GREATER
                 .ConstructUsing(src => src.Scope) 
#else
                 .ConstructUsing(src => src.Scope)
#endif
                .ReverseMap()
                    .ForMember(dst => dst.Scope, opt => opt.MapFrom(src => src));

            CreateMap<ApiScopeClaim, string>()
#if NET6_0_OR_GREATER
                 .ConstructUsing(src => src.Type) 
#else
                 .ConstructUsing(src => src.Type)
#endif
                .ReverseMap()
                    .ForMember(dst => dst.Type, opt => opt.MapFrom(src => src));

            CreateMap<ApiScope, Models.ApiScope>(MemberList.Destination)
#if NET6_0_OR_GREATER
                 .ConstructUsing(src => new Models.ApiScope()) 
#else
                 .ConstructUsing(src => new Models.ApiScope())
#endif
                .ForMember(dst => dst.Properties, opt => opt.MapFrom(src => src.Properties))
                .ForMember(dst => dst.UserClaims, opt => opt.MapFrom(src => src.UserClaims))
                .ReverseMap();

            CreateMap<IdentityResourceProperty, KeyValuePair<string, string>>()
                .ReverseMap();

            CreateMap<IdentityResource, Models.IdentityResource>(MemberList.Destination)
#if NET6_0_OR_GREATER
                 .ConstructUsing(src => new Models.IdentityResource()) 
#else
                 .ConstructUsing(src => new Models.IdentityResource())
#endif
                .ReverseMap();

            CreateMap<IdentityResourceClaim, string>()
#if NET6_0_OR_GREATER
                 .ConstructUsing(src => src.Type) 
#else
                 .ConstructUsing(src => src.Type)
#endif
                .ReverseMap()
                    .ForMember(dst => dst.Type, opt => opt.MapFrom(src => src));
        }
    }
}
