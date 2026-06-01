using KronoGeo_Api.Applications.ExtendMethods;
using KronoGeo_Api.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// - prochaine etape
// - installer Npgsql.EntityFrameworkCore.PostgreSQL et créer le dbcontext -- fait table dans la base de données fait 
// - il va falloir mettre en place un systeme pour prendre en compte le systeme d'enregistrement de la chaine de connexion dans les variable d'environnmement et le tester 
// - mettre en place l'authentification - manque toute la partie JWT bearer et enregistrement des param aussi
// - mettre en place les tests unitaires -- projet créé 

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

#region swagger
// - appel au swagger
builder.Services.AddSwaggerGen();
#endregion

#region Mise en place de MediatR
// - method d'extension
builder.Services.AddMediaTRExtend();
#endregion

#region DbContext
#if DEBUG
// - pour le développement on va utiliser les données secrètes pour stocker la ligne de connexion vers la base de données
builder.Services.AddDbContextSecretExtend(builder.Configuration);
#else //programmer pour la production sinon le faire avec les variables d'environnement
    builder.Services.AddDbContextSecretExtend(builder.Configuration);      
#endif

#endregion

#region Authentification / bearer Jwt
// mise en place de Identity.Ui
builder.Services.AddCustonIdentityUser();
builder.Services.AddCustomlsAuthentification(builder.Configuration);
builder.Services.AddAuthorizationPolicy();
#endregion

var app = builder.Build();

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

app.UseHttpsRedirection();

#region authentification
app.UseAuthentication();
app.UseAuthorization();
// - initialisation des rôles dans la base de données au lancement de l'application
await app.InitializeRolesAsync();
#endregion
app.MapControllers();

app.Run();
