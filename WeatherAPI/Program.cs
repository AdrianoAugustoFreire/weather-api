using Microsoft.EntityFrameworkCore;
using WeatherAPI;
using WeatherAPI.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient("OpenMeteoAPI", client =>
{
	client.BaseAddress = new Uri("https://api.open-meteo.com/v1/");
});

builder.Services.AddScoped<IWeatherDataProvider, OpenMeteoWeatherDataProvider>();
builder.Services.AddScoped<IWeatherForecastParserService, OpenMeteoParserService>();
builder.Services.AddScoped<IWeatherForecastDataService, WeatherForecastDataService>();

builder.Services.AddControllers();

builder.Services.AddDbContext<WeatherForecastDbContext>(options =>
	options.UseSqlite(builder.Configuration["ConnectionStrings:SQLiteDefault"]),
	ServiceLifetime.Scoped);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
	var dataContext = scope.ServiceProvider.GetRequiredService<WeatherForecastDbContext>();
	dataContext.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

