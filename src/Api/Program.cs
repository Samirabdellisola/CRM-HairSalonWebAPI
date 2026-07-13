using SalonCRM.Api.Configurations;

var builder = WebApplication.CreateBuilder(args);

// Render injects PORT; bind to 0.0.0.0 so the container accepts external traffic.
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

builder.Services.AddControllers();
builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddSwaggerDocumentation();

var app = builder.Build();

app.UseSwaggerDocumentation(app.Environment, app.Configuration);

// HTTPS redirection is typically handled by the Render reverse proxy in production.
if (!app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
