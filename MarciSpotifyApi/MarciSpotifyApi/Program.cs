using MarciSpotifyApi.Api.Interfaces;
using MarciSpotifyApi.Authorization;
using MarciSpotifyApi.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<SpotifyCredentials>(builder.Configuration.GetRequiredSection(nameof(SpotifyCredentials)));
builder.Services.Configure<SpotifySettings>(builder.Configuration.GetRequiredSection(nameof(SpotifySettings)));

builder.Services.AddSingleton<ISpotifyLoginService, SpotifyLoginService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
