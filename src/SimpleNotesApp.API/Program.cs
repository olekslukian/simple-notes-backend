using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SimpleNotesApp.Core.Repositories;
using SimpleNotesApp.Core.Services;
using SimpleNotesApp.Core.Services.Helpers;
using SimpleNotesApp.Infrastructure.Data;
using SimpleNotesApp.Infrastructure.Repositories;
using SimpleNotesApp.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<DbContext>();
builder.Services.AddScoped<IAuthHelper, AuthHelper>();
builder.Services.AddScoped<INotesRepository, NotesRepository>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<INotesService, NotesService>();
builder.Services.AddHttpClient<IEmailService, MailgunEmailService>(client =>
{
  client.Timeout = TimeSpan.FromSeconds(30);
});
builder.Services.AddScoped<IEmailService, MailgunEmailService>();

builder.Services.AddControllers();

builder.Services.Configure<RouteOptions>(options =>
{
  options.LowercaseUrls = true;
  options.LowercaseQueryStrings = true;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors((options) =>
{
  options.AddPolicy("DevCors", (corsBuilder) =>
  {
    corsBuilder.WithOrigins("http://localhost:4200", "http://localhost:3000", "http://localhost:8000")
          .AllowAnyMethod()
          .AllowAnyHeader()
          .AllowCredentials();
  });

  options.AddPolicy("ProdCors", (corsBuilder) =>
      {
        corsBuilder.WithOrigins("https://myproductionsite.com")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
      });
});


string tokenKeyString = builder.Configuration.GetSection("AppSettings:TokenKey").Value ?? "";

SymmetricSecurityKey tokenKey = new(Encoding.UTF8.GetBytes(tokenKeyString));

TokenValidationParameters tokenValidationParameters = new()
{
  // ClockSkew needs to be set to Zero if we need to test refresh token with a smaller expiration time
  // ClockSkew = TimeSpan.Zero,
  IssuerSigningKey = tokenKey,
  ValidateIssuer = false,
  ValidateIssuerSigningKey = false,
  ValidateAudience = false
};

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
  options.TokenValidationParameters = tokenValidationParameters;
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
  app.UseCors("DevCors");
}
else
{
  app.UseHttpsRedirection();
  app.UseCors("ProdCors");
}

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();


