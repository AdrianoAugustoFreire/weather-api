using Microsoft.EntityFrameworkCore;
using ProductAPI;
using ProductAPI.Services;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddScoped<INotify, BuyerNotifyService>();
builder.Services.AddScoped<IProductAPIRepositoryService, ProductRepositoryService>();
builder.Services.AddScoped<IProductActiveValidatorService, ProductActiveValidatorService>();
builder.Services.AddScoped<IProductBuyerChangeValidatorService, ProductBuyerChangeValidatorService>();

builder.Services.AddControllers();

builder.Services.AddDbContext<ProductAPIDbContext>(options =>
	options.UseSqlite(builder.Configuration["ConnectionStrings:SQLiteDefault"]),
	ServiceLifetime.Scoped);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
	var dataContext = scope.ServiceProvider.GetRequiredService<ProductAPIDbContext>();
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

