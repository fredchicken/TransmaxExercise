using EagleTraffic.Services;
using EagleTraffic;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var configuration = builder.Configuration;
builder.Services.AddDbContext<EagleContext>(options => options.UseSqlServer(configuration.GetConnectionString("SqlDatabase")));
builder.Services.AddStackExchangeRedisCache(options => { options.Configuration = configuration.GetConnectionString("RedisCache"); });
builder.Services.AddTransient<IEagleTrafficService, EagleTrafficService>();
builder.Services.AddScoped<IMessageService>(provider => {
    return new MessageService(configuration.GetValue<string>("RabbitMQUrl"));
});

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
