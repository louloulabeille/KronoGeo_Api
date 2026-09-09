using BruTile.Wms;
using KronoGeo_Api.Infrastructure.Service.Http;
using KronoGeo_Api.Infrastructure.Service.Map;
using KronoGeo_Api.Interface.Service;
using KronoGeo_Blazor.Client.Infrastructure.Extends;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

#region injection Ioption des url api
builder.Services.AddUrlApiExtend();
#endregion

#region injection de dépendance pour le HttpClient
builder.Services.AddHttpClientBFF(builder);

builder.Services.AddAutorizationClient();
#endregion

#region injection pour passer les localisations vers la map
builder.Services.AddScoped<IMapStateService,MapStateService>();
#endregion

await builder.Build().RunAsync();
