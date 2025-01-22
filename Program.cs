using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using System.Text;
using NoteTakingApp.Data;

var builder = WebApplication.CreateBuilder(args);

// Register ApplicationDbContext to use the connection string from appsettings.json
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add controllers
builder.Services.AddControllers();

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

// Add authentication using JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"], // Replace with your issuer
            ValidAudience = builder.Configuration["Jwt:Audience"], // Replace with your audience
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])) // Replace with your secret key
        };
    });

// Add authorization
builder.Services.AddAuthorization();

var app = builder.Build();

// Enable middleware for development
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// Enable CORS
app.UseCors("AllowAll");

// Add middleware for HTTPS redirection
app.UseHttpsRedirection();

// Add authentication and authorization middleware
app.UseAuthentication(); // Must be added before UseAuthorization
app.UseAuthorization();

// Map controllers
app.MapControllers();

// Run the application
app.Run();
