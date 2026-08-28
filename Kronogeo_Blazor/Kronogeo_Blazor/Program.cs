using KronoGeo_Api.Infrastructure.Service.Http;
using KronoGeo_Api.Models.Infrastructure.Http;
using KronoGeo_Blazor.Client.Pages;
using KronoGeo_Blazor.Components;
using KronoGeo_Blazor.Infrastructure.Extends;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Mvc.Authorization;

var builder = WebApplication.CreateBuilder(args);

#region AddControllers  ajoute la possibilité de mettre un controller au niveau serveur blazor
builder.Services.AddControllers(options =>
{
    // - on peut ajouter le AuthorizeFilter au niveau global pour que toutes les routes soient protégées par défaut
    // et il faudra ajouter l'attribut [AllowAnonymous] pour les routes qui ne nécessitent pas d'authentification
    options.Filters.Add(new AuthorizeFilter());
});

builder.Services.AddRazorPages();
#endregion

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

#region IOptions urlApi
builder.Services.AddUrlApiExtend(builder.Configuration);
#endregion

#region HttpClient injection
builder.Services.AddHttpClientExtend();
#endregion

#region Logging - log dans la console et dans des fichiers de log quotidiens
builder.Host.AddSeriLog();
#endregion


#region injection 
// -- ProtectedSessionStorage
//builder.Services.AddScoped<ProtectedSessionStorage>();
#endregion

#region swagger
// - appel au swagger
builder.Services.AddSwaggerGen();
#endregion

var app = builder.Build();

// -- CORS 
app.UseCors(cors => cors
                .AllowAnyMethod()
                .AllowAnyHeader()
                .SetIsOriginAllowed(origin => true)
                .AllowCredentials()
            );

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

if (app.Environment.IsStaging() || app.Environment.IsDevelopment()) // - pas en production || app.Environment.IsProduction()
{
    // - lancement du swagger
    app.UseSwagger();
    app.UseSwaggerUI(); // - https://localhost:7186/swagger/index.html
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(KronoGeo_Blazor.Client._Imports).Assembly);

#region mise en place de l'authentification sur le serveur Blazor
app.UseAuthentication();
app.UseAuthorization();
#endregion

#region lancement des controleurs indispensable pour les requêtes vers l'api
// -- 
app.MapControllers();
#endregion

#region Ilogger - lancement de l'application - Message
// -- démarrage de l'application 
app.Logger.LogInformation("Le serveur blazor kronogeo a démarré.");
#endregion

app.Run();
