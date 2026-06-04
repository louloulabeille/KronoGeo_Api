using KronoGeo_Api.Applications.ExtendMethods;
using KronoGeo_Api.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// - prochaine etape
// - installer Npgsql.EntityFrameworkCore.PostgreSQL et créer le dbcontext -- fait table dans la base de données fait 
// - il va falloir mettre en place un systeme pour prendre en compte le systeme d'enregistrement de la chaine de connexion dans les variable d'environnmement et le tester 
// - mettre en place l'authentification - manque toute la partie JWT bearer et enregistrement des param aussi
// - mettre en place les tests unitaires -- projet créé 

#region AddControllers
builder.Services.AddControllers(options =>
{
    // - on peut ajouter le AuthorizeFilter au niveau global pour que toutes les routes soient protégées par défaut
    // et il faudra ajouter l'attribut [AllowAnonymous] pour les routes qui ne nécessitent pas d'authentification
    options.Filters.Add(new AuthorizeFilter());
});
#endregion

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

#region swagger
// - appel au swagger
builder.Services.AddSwaggerGen();
#endregion

#region Mise en place de IservicesMessage pour envoi de mail de confirmation
builder.Services.AddServiceMessage(builder.Configuration);
#endregion

#region Mise en place de MediatR
// - method d'extension
builder.Services.AddMediaTRExtend(builder.Configuration);
#endregion

#region DbContext
#if DEBUG
// - pour le développement on va utiliser les données secrètes pour stocker la ligne de connexion vers la base de données
builder.Services.AddDbContextSecretExtend(builder.Configuration);
#else //programmer pour la production sinon le faire avec les variables d'environnement
    builder.Services.AddDbContextSecretExtend(builder.Configuration);      
#endif

#endregion

#region Logging - log dans la console et dans des fichiers de log quotidiens
builder.Host.AddSerilog();
#endregion

#region Authentification / bearer Jwt
// mise en place de Identity.Ui
builder.Services.AddCustonIdentityUser();
builder.Services.AddCustomlsAuthentification(builder.Configuration);
builder.Services.AddAuthorizationPolicy();
#endregion

var app = builder.Build();

#region app.Environment.IsDevelopment & app.Environment.IsStaging
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseDeveloperExceptionPage();
}

if (app.Environment.IsStaging() || app.Environment.IsDevelopment()) // - pas en production || app.Environment.IsProduction()
{
    // - lancement du swagger
    app.UseSwagger();
    app.UseSwaggerUI(); // - https://localhost:7291/swagger/index.html
}
#endregion

app.UseHttpsRedirection();

#region authentification
app.UseAuthentication();
app.UseAuthorization();
// - initialisation des rôles dans la base de données au lancement de l'application
await app.InitializeRolesAsync();
#endregion

app.MapControllers();

#region Ilogger - lancement de l'application - Message
// - log d'information pour indiquer que l'application a démarré avec succès
app.Logger.LogInformation("L'application KronoGeo_Api a démarré avec succès.");
#endregion

app.Run();
