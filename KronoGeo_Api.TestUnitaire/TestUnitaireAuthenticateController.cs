using Castle.Core.Logging;
using KronoGeo_Api.Applications.MediatR.Commands.Identity;
using KronoGeo_Api.Applications.MediatR.Queries.Identity;
using KronoGeo_Api.Applications.Model.DTO;
using KronoGeo_Api.TestUnitaire.Data;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace KronoGeo_Api.TestUnitaire
{
    public class TestUnitaireAuthenticateController
    {
        #region private properties
        private readonly KronoGeoContextMemory _context = new();
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly Mock<IOptions<KeyBearer>> _keyBearer = new();
        #endregion

        #region constructor
        public TestUnitaireAuthenticateController()
        {
            // - configuration de l'environnement de test pour les tests unitaires
            // avec une base de données en mémoire et un SignInManager configuré pour les tests
            var userStore = new UserStore<IdentityUser>(_context);
            var userManager = new UserManager<IdentityUser>(userStore, null, new PasswordHasher<IdentityUser>(), null, null, null, null, null, null);
            _signInManager = new SignInManager<IdentityUser>(userManager, Mock.Of<IHttpContextAccessor>(), Mock.Of<IUserClaimsPrincipalFactory<IdentityUser>>(), null, null, null, null);

            // - intialisation de la base de données
            Init();

            // - configuration de la clé de sécurité pour les tests unitaires
            _keyBearer.Setup(k => k.Value)
                .Returns(new KeyBearer()
                {
                    Key = "d24209c8e0790252de7eb7a7482c14e84e495f54de79ebea8c57986bee481457ea9392d63deecb1f2e74db2553352fc269bdf6892dde6ba949cc050eb4715862",
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    ValidateActor = false,
                    ValidateLifetime = true
                });

        }
        #endregion

        #region private initialization || init test de la base de données en mémoire et des rôles pour les tests unitaires
        private void Init()
        {
            // - suppression de tous les utilisateurs et rôles existants dans la base de données en mémoire avant chaque test
            _context.Users.RemoveRange(_context.Users);
            _context.Roles.RemoveRange(_context.Roles);
            _context.SaveChanges();

            // - création des roles dans la base de données en mémoire
            var roleManager = 
                new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(_context), null, null, null, null);
            string[] roles = ["Admin", "Manager", "User"];

            foreach (var role in roles)
            {
                if (!roleManager.RoleExistsAsync(role).Result)
                {
                    roleManager.CreateAsync(new IdentityRole(role)).Wait();
                }
            }

        }

        /// <summary>
        /// initialisation de la base avec un utlisateur
        /// </summary>
        /// <param name="register"></param>
        /// <param name="roles"></param>
        private void InitUser(RegisterDTO register, string[] roles)
        {
            var user = new IdentityUser {
                    UserName = register.Login, 
                    Email = register.Email, 
                    PhoneNumber = register.PhoneNumber,
            };
            var result = _signInManager.UserManager.CreateAsync(user, register.Password).Result;
            if (result.Succeeded)
            {
                // - assignation des rôles aux utilisateurs créés pour les tests unitaires
                foreach (var role in roles)
                {
                    _signInManager.UserManager.AddToRoleAsync(user, role).Wait();
                }   
            }
        }
        #endregion

        #region XUnit TestUnit pour AddUserCommand de MediatR - ajout d'utilisateur
        [Fact]
        public async Task ShouldAddFirstUserSuccessfully()
        {
            // - Arrange
            Mock<ILogger<AddUserHandler>> _logger = new();

            var registerDto = new RegisterDTO
            {
                Login = "testuser",
                Password = "123456789&AAAA",
                Email = "testuser@example.com",
                PhoneNumber = "",
                Token = ""
            };

            // - Act
            var handler = new AddUserHandler(_logger.Object, _keyBearer.Object, _signInManager);
            var result = await handler.Handle(new AddUserCommand { Register = registerDto }, CancellationToken.None);
            
            var user = await _signInManager.UserManager.FindByNameAsync(registerDto.Login);
            var roles = await _signInManager.UserManager.GetRolesAsync(user!);

            // - Assert
            Assert.True(result is not null);
            Assert.True(result.Result is not null);
            Assert.True(result.Result.Succeeded);
            Assert.False(string.IsNullOrEmpty(result.Register.Token!));
            Assert.True(roles.Contains("Admin") && roles.Contains("User"), "The first user should have both Admin and User roles assigned.");

        }

        [Fact]
        public async Task ShouldAddOtherUserSuccessfully()
        {
            // - Arrange
            Mock<ILogger<AddUserHandler>> _logger = new();

            var registerDtoFirst = new RegisterDTO
            {
                Login = "testuser",
                Password = "123456789&AAAA",
                Email = "testuser@example.com",
                PhoneNumber = "",
                Token = ""
            };

            var registerDto = new RegisterDTO
            {
                Login = "testuser2",
                Password = "123456789&AAAA",
                Email = "testuser2@example.com",
                PhoneNumber = "",
                Token = ""
            };


            // - Act
            var handlerFirst = new AddUserHandler(_logger.Object, _keyBearer.Object, _signInManager);
            var resultFirst = await handlerFirst.Handle(new AddUserCommand { Register = registerDtoFirst }, CancellationToken.None);

            var handler = new AddUserHandler(_logger.Object, _keyBearer.Object, _signInManager);
            var result = await handler.Handle(new AddUserCommand { Register = registerDto }, CancellationToken.None);

            var user = await _signInManager.UserManager.FindByNameAsync(registerDto.Login);
            var roles = await _signInManager.UserManager.GetRolesAsync(user!);


            // - Assert
            Assert.True(result is not null);
            Assert.True(result.Result is not null);
            Assert.True(result.Result.Succeeded);
            Assert.False(string.IsNullOrEmpty(result.Register.Token!));
            Assert.True(roles.Contains("User"), "Subsequent users should only have the User role assigned.");
        }
        #endregion

        #region Xunit TestUnit pour LoginUserCommand de MediatR - connexion d'utilisateur
        [Fact]
        public async Task ShouldLoginUserSuccessfully()
        {
            // - Arrange
            Mock<ILogger<LoginUserHandler>> _logger = new();

            var registerDto = new RegisterDTO
            {
                Login = "testuser",
                Password = "123456789&AAAA",
                Email = "testuser@example.com",
                PhoneNumber = "",
                Token = ""
            };

            // - ajout de l'utilisateur dans la base de données en mémoire pour le test de connexion
            InitUser(registerDto, ["User"]);

            // - Act
            var handler = new LoginUserHandler(_logger.Object, _keyBearer.Object, _signInManager);
            var result = await handler.Handle(new LoginUserCommand { Register = registerDto }, CancellationToken.None);

            // - Assert
            Assert.True(result is not null);
            Assert.True(result.SignInResult is not null);
            Assert.True(result.SignInResult.Succeeded);
            Assert.False(string.IsNullOrEmpty(result.Register.Token));

        }
        #endregion
    }
}
