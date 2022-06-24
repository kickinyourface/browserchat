using BrowserChat.Backend.Core;
using BrowserChat.Backend.Core.Data;
using BrowserChat.Backend.Core.HubConfig;
using BrowserChat.Backend.Core.Util;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
bool isProduction = builder.Environment.IsProduction();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<BrowserChatDbContext>(opt =>
{
    if (isProduction)
    {
        opt.UseSqlServer(builder.Configuration.GetConnectionString("DbConn"));
    }
    else
    {
        opt.UseInMemoryDatabase("InMem");
    }
});

builder.Services.AddScoped<IBrowserChatRepository, BrowserChatRepository>();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddCors(opt =>
{
    opt.AddPolicy("CorsRule", rule =>
    {
        rule
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials()
            .WithOrigins(
                builder.Configuration.GetValue<string>("ClientDomain")
            );
    });
});

builder.Services.AddSignalR(opt =>
{
    opt.EnableDetailedErrors = true;
});

builder.Services.AddScoped(typeof(HubHelper));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("CorsRule");
app.MapHub<HubBase>("/hub");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

ConfigurationHelper.Initialize(builder.Configuration);
Persistence.PrepPopulation(app, isProduction);

app.Run();
