using KronoGeo_Api.Infrastructure.Service.Email.MessageFactoryMethod;
using KronoGeo_Api.Interface.Service;
using KronoGeo_Api.Models.Infrastructure.Email;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace KronoGeo_Api.Applications.Email
{

    public class ApiMailIdentity(IServiceSendMessage serviceMail
        , ILogger logger)
    {
        #region private properties
        private readonly IServiceSendMessage _serviceSendMail = serviceMail;
        private readonly ILogger _logger = logger;
        #endregion


        /// <summary>
        /// Method d'envoi du mail qui se fait automatique dans l'API
        /// </summary>
        /// <param name="user"></param>
        /// <param name="messagObjectMessage">Object d'instanciation du body du mail</param>
        /// <returns></returns>
        public MessageResult SendEmail(IdentityUser user, MessageCourrielFactory objectMessage)
        {
          
            var message = MessageCourriel.Message(objectMessage);
            if (user.Email is null)
            {
                _logger.LogError("User {UserName} has no email address. Cannot send confirmation email.", user.UserName);
                return new MessageResult()
                {
                    To = string.Empty,
                    Message = message,
                    Status = EmailResultStatus.Failure,
                    Exception = new ArgumentNullException($"{user.UserName} has no email address. Cannot send confirmation email.")
                };
            }

            var result = _serviceSendMail.Send(user.Email, "Confirmation de votre adresse email", message);
            if (result.Status == EmailResultStatus.Failure)
            {
                _logger.LogError("Failed to send confirmation email to {Email}: {Message}", user.Email, result.Message);
            }
            return result;
        }
    }
}
