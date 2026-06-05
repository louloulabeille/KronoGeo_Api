using KronoGeo_Api.Applications.Model.DTO;
using KronoGeo_Api.Interface.Service;
using KronoGeo_Api.Models.Infrastructure.Email;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace KronoGeo_Api.Applications.MediatR.Queries.Identity
{
    public class UpdateUserHandler( ILogger<AddUserHandler> logger
        , IOptions<KeyBearer> keyBearer, SignInManager<IdentityUser> signInManager
        , IServiceSendMessage serviceSendMail
        , IOptions<UrlOptions> urlOptions)
        : UserIdentityHandler (logger, keyBearer, signInManager)
    {


        #region private properties
        private readonly IServiceSendMessage _serviceSendMail = serviceSendMail;
        private readonly IOptions<UrlOptions> _urlOptions = urlOptions;
        #endregion

        #region public method



        #endregion

    }
}
