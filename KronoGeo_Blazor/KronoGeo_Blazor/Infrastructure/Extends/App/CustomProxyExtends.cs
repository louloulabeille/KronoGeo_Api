namespace KronoGeo_Blazor.Infrastructure.Extends.App
{
    public static class CustomProxyExtends
    {
        extension (IEndpointRouteBuilder app)
        {
            /// <summary>
            /// Création du proxy pour éviter les problèmes de CORS et de UserAgent
            /// Au niveau de MapSui configurer le nouveau OpenstreetMap  
            /// </summary>
            /// <returns></returns>
            public IEndpointRouteBuilder ProxyOpenStreetMap()
            {
                app.MapGet("/tiles/{z}/{x}/{y}.png", async (int z, int x, int y, HttpClient httpClient) =>
                {
                    var url = $"https://tile.openstreetmap.org/{z}/{x}/{y}.png";
                    var request = new HttpRequestMessage(HttpMethod.Get, url);
                    request.Headers.UserAgent.ParseAdd("Kronogeo/1.0 (louloulabeille@alwaysdata.net)");

                    var response = await httpClient.SendAsync(request);
                    var bytes = await response.Content.ReadAsByteArrayAsync();
                    return Results.File(bytes, "image/png");
                });

                return app;
            }
        }
    }
}
