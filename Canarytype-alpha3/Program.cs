using Canarytype_alpha3.Data;
using Canarytype_alpha3.Hubs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Google.Apis.Auth.AspNetCore3;
using Microsoft.AspNetCore.Authentication.Cookies;
using MassTransit;
using CanaryEmailsService.Contracts;
using Canarytype_alpha3.Controllers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<CanaryTypeDBContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("CanaryTypeDB"));
});

builder.Services.AddScoped<IUserProfileStats,  UserProfileStats>();
builder.Services.AddScoped<LoginController>();

builder.Services.AddSignalR();

builder.Services.AddMassTransit(x =>
{
    x.UsingAzureServiceBus((context, config) =>
    {
        config.Host(builder.Configuration.GetConnectionString("AzureServiceBus"));
        config.Message<ISendEmailMessage>(configurator => { });
        config.Publish<ISendEmailMessage>(topology => { });
    });
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Enter Bearer [space] your token",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("CanaryTypeCorsPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:5173", 
            "https://localhost:5173", 
            "https://canarytype-alpha3.pages.dev", 
            "https://canarytype.harshjain17.com", 
            "http://localhost:5500", 
            "https://canarytypeazure.harshjain17.com", 
            "https://proud-island-01ae65f00.5.azurestaticapps.net", 
            "https://accounts.google.com",
            "https://localhost:3000",
            "https://canarytype-react.harshjain17.com"
        ).AllowAnyMethod().AllowAnyHeader().AllowCredentials();
    });
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddCookie(x =>
{
    x.Cookie.Name = "token";
})
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("Attack on titan is the best anime, defjfhjhfjrshgkjrghrsjghsgkjshgkjrhg")),
        ValidateIssuer = false,
        ValidateAudience = false,
    };
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            context.Token = context.Request.Cookies["token"];
            return Task.CompletedTask;
        }
    };
});
builder.Services.AddApplicationInsightsTelemetry(new Microsoft.ApplicationInsights.AspNetCore.Extensions.ApplicationInsightsServiceOptions
{
    ConnectionString = builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"]
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("CanaryTypeCorsPolicy");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.MapHub<ChatHub>("/Chat");

app.Run();
