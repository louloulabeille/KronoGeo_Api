using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace KronoGeo_Api.Applications.CustomTokenProviders
{
    public class EmailConfirmationTokenProvider<TUser>(IDataProtectionProvider dataProtectionProvider
            , IOptions<DataProtectionTokenProviderOptions> options
            , ILogger<DataProtectorTokenProvider<TUser>> logger) 
        : DataProtectorTokenProvider<TUser>(dataProtectionProvider, options, logger) where TUser : class
    {
      
    }

    public class EmailConfirmationTokenProviderOptions : DataProtectionTokenProviderOptions
    {
    }
}
