using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SpraywallAppWeb.Data;
using SpraywallAppWeb.Helpers;
using SpraywallAppWeb.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("UserDB");

// Add authentication support - allows endpoints to check if user has a valid security token
builder.Services
    .AddAuthentication(x =>
    {
        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(x =>
    {
        x.RequireHttpsMetadata = false;
        x.SaveToken = true;
        x.TokenValidationParameters = new TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(AuthSettings.PrivateKey)),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });
builder.Services.AddAuthorization();

// Add services to the container.
builder.Services.AddControllers();

// Add the authservice as a service: allows token generation to be
// accessed through dependency injection (DI)
builder.Services.AddTransient<AuthService>();

// Add swagger support (for ease of development)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add the dbcontext as a service (more DI) - allows controllers/services to access the database(s)
builder.Services.AddDbContextFactory<UserContext>(options => options.UseSqlite(connectionString));

var app = builder.Build();

// Add authentication support - allows endpoints to check if user has a valid security token
app.UseAuthentication();
app.UseAuthorization();

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
