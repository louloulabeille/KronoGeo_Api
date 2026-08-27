using KronoGeo_Blazor.Client.Infrastructure.Extends;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

#region injection Ioption des url api
builder.Services.AddUrlApiExtend();
#endregion

#region injection de dépendance pour le HttpClient
builder.Services.AddHttpClientBFF(builder);
#endregion

await builder.Build().RunAsync();
