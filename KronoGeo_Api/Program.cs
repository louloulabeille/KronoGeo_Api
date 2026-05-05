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
// - récupération de la ligne de connexion vers la base de données -- stocker dans les données secrètes
string? stringConnection = builder.Configuration.GetConnectionString("DefaultConnection") ??
    throw new InvalidOperationException("Connection string 'DefaultConnection' not found");

// - appel du DbContext pour la connexion vers la base
builder.Services.AddDbContext<KronoGeoDbContext>(options =>
    options.UseNpgsql(stringConnection));
#endregion

#region Authentification / bearer Jwt
// mise en place de Identity.Ui
builder.Services.AddCustonIdentityUser();
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseDeveloperExceptionPage();
}

if (app.Environment.IsStaging() || app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    // - lancement du swagger
    app.UseSwagger();
    app.UseSwaggerUI(); // - https://localhost:7291/swagger/index.html
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
