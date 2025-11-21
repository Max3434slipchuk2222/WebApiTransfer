using Domain;
using Domain.Entities.Location;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbTransferContext>(options =>
	options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddControllers();

builder.Services.AddSwaggerGen();
builder.Services.AddCors();

var app = builder.Build();
using(var scope = app.Services.CreateScope())
{
	var services = scope.ServiceProvider;
	try
	{
		var context = services.GetRequiredService<AppDbTransferContext>();
		context.Database.Migrate();

		if (!context.Countries.Any())
		{
			var contentRoot = app.Environment.ContentRootPath;
			var path = Path.Combine(contentRoot, "JSON", "countries.json");

			if (File.Exists(path))
			{
				var jsonData = File.ReadAllText(path);

				var options = new JsonSerializerOptions
				{
					PropertyNameCaseInsensitive = true
				};

				var countries = JsonSerializer.Deserialize<List<CountryEntity>>(jsonData, options);

				if (countries != null && countries.Any())
				{
					foreach (var country in countries)
					{
						country.DateCreated = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
						country.IsDeleted = false;
					}

					context.Countries.AddRange(countries);
					context.SaveChanges();
				}
			}
			else
			{
				Console.WriteLine("Файл countries.json не знайдено!");
			}
		}
	}
	catch (Exception ex)
	{
		Console.WriteLine($"Помилка під час сідінгу: {ex.Message}");
	}
}
app.UseCors(policy =>
	policy.AllowAnyOrigin()
		  .AllowAnyMethod()
		  .AllowAnyHeader());
app.UseSwagger();
app.UseSwaggerUI();

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

app.Run();
