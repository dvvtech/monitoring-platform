using Monitoring.Api.AppStart;

var builder = WebApplication.CreateBuilder(args);

var startup = new Startup(builder);
startup.Initialize();

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecksUI();

app.Run();
