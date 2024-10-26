using bibliotekAPI.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Konfigurer DbContext og SQLite
builder.Services.AddDbContext<bibliotekContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("bibliotekContext")));

// Legg til controller-tjenester og konfigurer JSON-serialisering for sirkulære referanser
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
    });

// Legg til Swagger-tjenester for API-dokumentasjon
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Legg til CORS-policyen
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder
            .WithOrigins("http://127.0.0.1:5500") // Bytt ut med riktig domene om nødvendig
            .AllowAnyHeader()
            .AllowAnyMethod());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Aktiver CORS-policyen
app.UseCors("AllowSpecificOrigin");
app.UseDeveloperExceptionPage();
app.UseAuthorization();
app.MapControllers();

app.Run();