using Microsoft.EntityFrameworkCore;
using ZooAPI;
using ZooAPI.BusinessLogic;
using ZooAPI.Data;
using ZooAPI.Data.ModelConfigs;
using ZooAPI.Data.Repositories;
using ZooAPI.Domain.Models.Profiles;

var builder = WebApplication.CreateBuilder(args);

// Register ZooAPIDbContext as a service
builder.Services.AddDbContext<ZooAPIDbContext>(options =>
    options.UseSqlServer(Consts.ConnectionString));

// Register EntityConfig as a service (if needed)
builder.Services.AddScoped<IAnimalConfig, AnimalConfig>();

// Register repositories
builder.Services.AddScoped<IAnimalRepository, AnimalRepository>();

// Add services to the container.
builder.Services.AddScoped<IAnimalService, AnimalService>();
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register the mapping profile
builder.Services.AddAutoMapper(typeof(AnimalMappingProfile));

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