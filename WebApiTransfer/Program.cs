using Core.Interfaces;
using Core.Services;
using Domain;
using Domain.Entities.Location;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbTransferContext>(options =>
	options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddControllers();

builder.Services.AddSwaggerGen();
builder.Services.AddCors();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddScoped<ICountryService, CountryService>();
builder.Services.AddScoped<ICityService, CityService>();
builder.Services.AddScoped<IImageService, ImageService>();
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
	options.SuppressModelStateInvalidFilter = true;
});
builder.Services.AddValidatorsFromAssemblies(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddMvc(options =>
{
	options.Filters.Add<ValidationFilter>();
});
var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
	var services = scope.ServiceProvider;
	try
	{
		var context = services.GetRequiredService<AppDbTransferContext>();
		context.Database.Migrate();

		if (!context.Countries.Any())
		{
			var contentRoot = app.Environment.ContentRootPath;
			var pathCountry = Path.Combine(contentRoot, "JSON", "countries.json");

			if (File.Exists(pathCountry))
			{
				var jsonData = File.ReadAllText(pathCountry);

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

var dirImageName = builder.Configuration.GetValue<string>("DirImageName") ?? "duplo";

var path = Path.Combine(Directory.GetCurrentDirectory(), dirImageName);
Directory.CreateDirectory(dirImageName);

app.UseStaticFiles(new StaticFileOptions
{
	FileProvider = new PhysicalFileProvider(path),
	RequestPath = $"/{dirImageName}"
});

app.Run();
