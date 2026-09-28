namespace KronoGeo_Blazor.Infrastructure.Extends.App
{
    public static class CustomProxyExtends
    {
        extension (IEndpointRouteBuilder app)
        {
            /// <summary>
            /// Création du proxy pour éviter les problèmes de CORS et de UserAgent
            /// Au niveau de MapSui configuration 
            /// </summary>
            /// <returns></returns>
            public IEndpointRouteBuilder ProxyOpenStreetMap()
            {
                app.MapGet("/tiles/{z}/{x}/{y}.png", async (int z, int x, int y, HttpClient httpClient) =>
                {
                    var url = $"https://tile.openstreetmap.org/{z}/{x}/{y}.png";
                    var request = new HttpRequestMessage(HttpMethod.Get, url);
                    // -- paramétrage du useragent fait par le serveur impossible de le faire avec FireFox
                    request.Headers.UserAgent.ParseAdd("Kronogeo/1.0 (louloulabeille@alwaysdata.net)");
                    // -- envoie de la requette
                    var response = await httpClient.SendAsync(request);
                    // -- réception
                    var bytes = await response.Content.ReadAsByteArrayAsync();
                    return Results.File(bytes, "image/png");
                });

                return app;
            }
        }
    }
}
