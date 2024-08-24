using Microsoft.EntityFrameworkCore;
using SpraywallAppWeb.Data;
using SpraywallAppWeb.Services;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("UserDB");

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
