using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PiCloud.Configurations;
using PiCloud.Data;
using Serilog;
using System.Diagnostics;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var _logger = new LoggerConfiguration().WriteTo.File("..\\Logs\\log-.log", rollingInterval: RollingInterval.Day).CreateLogger();
builder.Logging.AddSerilog(_logger);

string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(o => o.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.Configure<JwtConfig>(builder.Configuration.GetSection("JwtConfig"));

Debug.WriteLine(builder.Configuration.GetSection("JwtConfig"));

builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(jwt =>
{
    var key = Encoding.ASCII.GetBytes(builder.Configuration.GetSection("JwtConfig:Key").Value);

    jwt.SaveToken = true;
    jwt.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false, // Might not work -> change it to false
        ValidateAudience = false, // Might not work -> change it to false
        RequireExpirationTime = false, // during dev
        ValidateLifetime = true,
    };
});

builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedEmail = false;
    options.Password.RequireNonAlphanumeric = false;
    
    // Remove when publishing
    options.Password.RequireDigit = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.Password.RequiredLength = 1;
}).AddEntityFrameworkStores<AppDbContext>();


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
