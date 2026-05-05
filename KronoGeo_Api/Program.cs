using KronoGeo_Api.Applications.ExtendMethods;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

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

app.UseAuthorization();

app.MapControllers();

app.Run();
