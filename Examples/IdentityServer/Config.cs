// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;

namespace IdentityServer
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> Ids =>
            new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
                new IdentityResource
                {
                    Name = IdentityServerConstants.StandardScopes.Address,
                    DisplayName = "Address",
                    UserClaims = new [] { "address", "phone_number" }
                },
                new IdentityResource
                {
                    Name = "api_role",
                    DisplayName = "Api Role",
                    UserClaims = new [] { "api_role" }
                }
            };

        public static IEnumerable<ApiResource> Apis =>
            new List<ApiResource>
            {
                new ApiResource("api", "Weather Forecast API", new[] { "api_role" })
            };

        public static IEnumerable<Client> Clients =>
            new List<Client>
            {
                new Client
                {
                    ClientId = "Client.Code",
                    ClientName = "Client.Code",

                    AllowedGrantTypes = GrantTypes.Code,
                    RequireClientSecret = false,

                    RedirectUris = {
                        "http://localhost:5005/signin-callback-oidc",
                    },
                    PostLogoutRedirectUris = { "http://localhost:5005/" },

                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "api"
                    }
                },

                new Client
                {
                    ClientId = "Client.Implicit.UsePopup",
                    ClientName = "Client.Implicit.UsePopup",

                    AllowedGrantTypes = GrantTypes.Implicit,
                    RequireClientSecret = false,
                    AllowAccessTokensViaBrowser = true,

                    RedirectUris = {
                        "http://localhost:5003/signin-popup-oidc",
                        "http://localhost:5003/signout-popup-oidc",
                    },
                    PostLogoutRedirectUris = { 
                        "http://localhost:5003/", 
                        "http://localhost:5003/signout-popup-oidc" 
                    },

                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "api"
                    }
                },

                new Client
                {
                    ClientId = "Client.Code.CustomizeUri",
                    ClientName = "Client.Code.CustomizeUri",

                    AllowedGrantTypes = GrantTypes.Code,
                    RequireClientSecret = false,

                    RedirectUris = {
                        "http://localhost:5006/fantastic-url-for-redirect",
                        "http://localhost:5006/wonderful-link-for-popup-login",
                        "http://localhost:5006/sign-out-popup-here",
                    },
                    PostLogoutRedirectUris = { 
                        "http://localhost:5006/", 
                        "http://localhost:5006/sign-out-popup-here" 
                    },

                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "api"
                    }
                },

                new Client
                {
                    ClientId = "Client.Code.Complex",
                    ClientName = "Client.Code.Complex",

                    AllowedGrantTypes = GrantTypes.Code,
                    RequireClientSecret = false,
                    RequireConsent = false,
                    RequirePkce = true,

                    RedirectUris = {
                        "http://localhost:5002/signin-popup-oidc",
                        "http://localhost:5002/signin-callback-oidc",
                        "http://localhost:5002/silent-callback-oidc",
                    },
                    PostLogoutRedirectUris = { "http://localhost:5002/" },

                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        IdentityServerConstants.StandardScopes.Address,
                        "api_role",
                        "api",
                    },
                    AllowOfflineAccess = true,
                    AccessTokenLifetime = 50,
                },

                new Client
                {
                    ClientId = "Client.Implicit.RequiredLogin",
                    ClientName = "Client.Implicit.RequiredLogin",

                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,
                    RequireClientSecret = false,

                    RedirectUris = {
                        "http://localhost:5004/signin-callback-oidc",
                    },
                    PostLogoutRedirectUris = { "http://localhost:5004/" },

                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "api"
                    }
                },
        };
    }
}