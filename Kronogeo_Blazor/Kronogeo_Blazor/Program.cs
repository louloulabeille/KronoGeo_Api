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
// --  Le handler HTTP
//builder.Services.AddTransient<TokenHeaderHandler>();

// -- ProtectedSessionStorage
//builder.Services.AddScoped<ProtectedSessionStorage>();
#endregion



var app = builder.Build();

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
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(KronoGeo_Blazor.Client._Imports).Assembly);

#region Ilogger - lancement de l'application - Message
// -- démarrage de l'application 
app.Logger.LogInformation("Le serveur blazor kronogeo a démarré.");
#endregion

app.Run();
