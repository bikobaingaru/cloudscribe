﻿//// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
//// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

//// This is a copy of the Microsoft SecurityStampValidator from ASP.NET Core.
//// It's the exact same code, except that it simply does not re-issue the authentication
//// cookie if the security stamp is still valid. By re-issuing the cookie from the
//// user's claims in the database, we lose crucial claims that were only available
//// at login time (idp, amr, sid, etc).
//// Last Modified 2017-07-25 JA

//using Microsoft.AspNetCore.Identity;
//using System;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Authentication;
//using Microsoft.AspNetCore.Authentication.Cookies;
//using Microsoft.Extensions.Options;
//using cloudscribe.Core.Models;
//using cloudscribe.Core.Identity;

//namespace cloudscribe.Core.IdentityServerIntegration
//{
//    public class SecurityStampValidator<TUser> : ISecurityStampValidator where TUser : SiteUser
//    {
//        private readonly SecurityStampValidatorOptions _options;
//        private readonly SignInManager<TUser> _signInManager;
//        private ISystemClock _clock;

//        public SecurityStampValidator(
//            IOptions<SecurityStampValidatorOptions> options, 
//            SignInManager<TUser> signInManager, 
//            ISystemClock clock)
//        {
//            _options = options.Value;
//            _signInManager = signInManager;
//            _clock = clock;
//        }

//        public async Task ValidateAsync(CookieValidatePrincipalContext context)
//        {
//            var currentUtc = DateTimeOffset.UtcNow;
//            if (context.Options != null && _clock != null)
//            {
//                currentUtc = _clock.UtcNow;
//            }
//            var issuedUtc = context.Properties.IssuedUtc;

//            // Only validate if enough time has elapsed
//            var validate = (issuedUtc == null);
//            if (issuedUtc != null)
//            {
//                var timeElapsed = currentUtc.Subtract(issuedUtc.Value);
//                validate = timeElapsed > _options.ValidationInterval;
//            }
//            if (validate)
//            {
//                var user = await _signInManager.ValidateSecurityStampAsync(context.Principal);
//                if (user != null)
//                {
//                    // REVIEW: note we lost login authenticaiton method

//                    // Brock Allen: this line have been deliberatly commented out (see comment above)
//                    //context.ReplacePrincipal(await _signInManager.CreateUserPrincipalAsync(user));

//                    // Brock Allen: we leave this line so a new cookie is issued with updated
//                    // issused and expires values
//                    context.ShouldRenew = true;
//                }
//                else
//                {
//                    context.RejectPrincipal();
//                    await _signInManager.SignOutAsync();
//                }
//            }
//        }
//    }
//}
