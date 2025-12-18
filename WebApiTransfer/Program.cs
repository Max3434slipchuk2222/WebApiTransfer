using Core.Interfaces;
using Core.Models.Account;
using Core.Services;
using Domain;
using Domain.Entities.Idenity;
using Domain.Entities.Location;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<AppDbTransferContext>(options =>
	options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddIdentity<UserEntity, RoleEntity>(options =>
{
	options.Password.RequireDigit = false;
	options.Password.RequireLowercase = false;
	options.Password.RequireUppercase = false;
	options.Password.RequiredLength = 6;
	options.Password.RequireNonAlphanumeric = false;
})
	.AddEntityFrameworkStores<AppDbTransferContext>()
	.AddDefaultTokenProviders();
builder.Services.AddAuthentication(options =>
{
	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
	options.RequireHttpsMetadata = false;
	options.SaveToken = true;
	options.TokenValidationParameters = new TokenValidationParameters
	{
		ValidateIssuer = false,
		ValidateAudience = false,
		ValidateIssuerSigningKey = true,
		ValidateLifetime = true,
		ClockSkew = TimeSpan.Zero,
		IssuerSigningKey = new SymmetricSecurityKey(
			Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
	};
});
builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowTwoDomains", policy =>
	{
		policy.WithOrigins("http://localhost:5173", "http://localhost:5174",
			"http://transfersitereactts.somee.com")
			  .AllowAnyHeader()
			  .AllowAnyMethod()
			  .AllowCredentials(); 
	});
});
builder.Services.AddControllers();
var assemblyName = typeof(LoginModel).Assembly.GetName().Name;

builder.Services.AddSwaggerGen(opt =>
{
	var fileDoc = $"{assemblyName}.xml";
	var filePath = Path.Combine(AppContext.BaseDirectory, fileDoc);
	opt.IncludeXmlComments(filePath);

	opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
	{
		Description = "JWT Authorization header using the Bearer scheme.",
		Name = "Authorization",
		In = ParameterLocation.Header,
		Type = SecuritySchemeType.Http,
		Scheme = "bearer"
	});

	opt.AddSecurityRequirement(new OpenApiSecurityRequirement
	{
		{
			new OpenApiSecurityScheme
			{
				Reference = new OpenApiReference
				{
					Type=ReferenceType.SecurityScheme,
					Id="Bearer"
				}
			},
			new string[]{}
		}
	});

});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddScoped<ICountryService, CountryService>();
builder.Services.AddScoped<ICityService, CityService>();
builder.Services.AddScoped<IImageService, ImageService>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<GoogleService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();



builder.Services.Configure<ApiBehaviorOptions>(options =>
{
	options.SuppressModelStateInvalidFilter = true; 
});
builder.Services.AddValidatorsFromAssemblies(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddControllers(options =>
{
	options.Filters.Add<ValidationFilter>();
});

var app = builder.Build();

var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();

lifetime.ApplicationStarted.Register(async () =>
{
	try
	{
		using var scope = app.Services.CreateScope();
		var userManager = scope.ServiceProvider.GetRequiredService<UserManager<UserEntity>>();
		var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
		var admins = await userManager.GetUsersInRoleAsync("Admin");

		if (admins.Any())
		{
			var subject = "Server Started Successfully";
			var body = $"Адміністраторе, повідомляємо, що сервер успішно запустився.Час запуску: {DateTime.UtcNow}";

			foreach (var admin in admins)
			{
				if (!string.IsNullOrEmpty(admin.Email))
				{
					await emailService.SendEmailAsync(admin.Email, subject, body);
					Console.WriteLine($" Notification sent to Admin: {admin.Email}");
				}
			}
		}
		else
		{
			Console.WriteLine("No admins found to send startup notification.");
		}
	}
	catch (Exception ex)
	{
		Console.WriteLine($"Failed to send startup emails: {ex.Message}");
	}
});
app.UseCors("AllowTwoDomains");
await DbSeeder.SeedData(app.Services);
var dirName = builder.Configuration.GetValue<string>("DirImageName") ?? "images";
var dirPath = Path.Combine(app.Environment.ContentRootPath, dirName);

if (!Directory.Exists(dirPath))
{
	Directory.CreateDirectory(dirPath);
}

app.UseStaticFiles(new StaticFileOptions
{
	FileProvider = new PhysicalFileProvider(dirPath),
	RequestPath = $"/{dirName}" 
});

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API"));

app.Use(async (context, next) =>
{
	context.Response.Headers.Append("Cross-Origin-Opener-Policy", "same-origin-allow-popups");
	context.Response.Headers.Append("Cross-Origin-Embedder-Policy", "require-corp");
	await next();
});
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();