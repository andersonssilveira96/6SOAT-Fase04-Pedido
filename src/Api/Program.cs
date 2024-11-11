using Api.Extensions;
using Api.Helper;
using Application;
using Infra.Data;
using Infra.MessageBroker;
using Infra.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Domain.Consumer;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "TechChallenge Pedido API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme.",
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                new string[] {}
        }
    });
});

builder.Services.AddApplicationService();
builder.Services.AddInfraDataServices();
builder.Services.AddInfraMessageBrokerServices();

builder.Services.AddDbContext<TechChallengeContext>(
    options => options.UseNpgsql(builder.Configuration.GetConnectionString("Database")));

builder.Services.Configure<AuthenticationCognitoOptions>(builder.Configuration.GetSection("CognitoConfig"));
builder.Services.AddAuthenticationConfig();

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var app = builder.Build();

// Invocar o serviço
using var scope = app.Services.CreateScope();
var messageConsumer = scope.ServiceProvider.GetRequiredService<IMessageBrokerConsumer>();
_ = Task.Run(() => messageConsumer.ReceiveMessageAsync());

app.UseSwagger();

app.UseSwaggerUI();

app.ApplyMigrations();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
