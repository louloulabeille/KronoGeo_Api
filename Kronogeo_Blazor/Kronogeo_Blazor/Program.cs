using KronoGeo_Api.Infrastructure.Service.Http;
using KronoGeo_Api.Models.Infrastructure.Http;
using KronoGeo_Blazor.Client.Pages;
using KronoGeo_Blazor.Components;
using KronoGeo_Blazor.Infrastructure.Extends;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

var builder = WebApplication.CreateBuilder(args);

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
// -- Conteneur de token : un seul par circuit/utilisateur
builder.Services.AddScoped<UserTokenContainer>();

// --  Le handler HTTP
builder.Services.AddTransient<TokenHeaderHandler>();

// -- ProtectedSessionStorage
builder.Services.AddScoped<ProtectedSessionStorage>();
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
