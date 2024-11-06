using Microsoft.Extensions.DependencyInjection;
using MediatR;
using CreolytixECommerce.Application.Handlers.Commands.Reservations;
using CreolytixECommerce.Infrastructure.Configuration;
using CreolytixECommerce.Application.Interfaces.Messaging;
using CreolytixECommerce.Infrastructure.Messaging;
using CreolytixECommerce.Infrastructure.Messaging.Consumers.Product;
using CreolytixECommerce.Infrastructure.Messaging.Consumers.Reservations;
using CreolytixECommerce.Infrastructure.Messaging.Consumers.Stores;
using CreolytixECommerce.Infrastructure.Messaging.Consumers;
using CreolytixECommerce.Infrastructure.Data;
using System.Net.NetworkInformation;
using CreolytixECommerce.Domain.Interfaces;
using CreolytixECommerce.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Microsoft.Extensions.Options;
using CreolytixECommerce.Domain.Entities;
using System.Reflection;
using CreolytixECommerce.Application.Interfaces;
using Microsoft.AspNetCore.Hosting;
using CreolytixECommerce.Application.Handlers.Commands.Inventory;
using CreolytixECommerce.Application.Handlers.Queries.Products;
using CreolytixECommerce.Application.Handlers.Queries.Reservations;
using CreolytixECommerce.Application.Handlers.Queries.Stores;
using CreolytixECommerce.Infrastructure.Messaging.Consumers.Inventory;
using CreolytixECommerce.API.Mapping;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.



// Add Infrastructure services
//builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddAutoMapper(typeof(MappingProfiles));

builder.Services.AddControllers();

// Add Mediator for CQRS
//builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
//builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(AddProductCommandHandler).Assembly));

// Configure RabbitMQ settings from configuration
var rabbitMqSettings = builder.Configuration.GetSection("RabbitMqSettings").Get<RabbitMqSettings>();
builder.Services.AddSingleton(rabbitMqSettings);
///builder.Services.AddSingleton<RabbitMqPublisher>();
//builder.Services.AddHostedService<RabbitMqListener>();

// Register RabbitMQ services
builder.Services.AddSingleton<IMessagePublisher, RabbitMqPublisher>();
builder.Services.AddSingleton<IMessageListener, RabbitMqListener>();

// Configure MongoDB
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDbSettings"));

// Register MongoDbContext as a singleton
builder.Services.AddSingleton<MongoDbContext>();


// Register Repositories
builder.Services.AddScoped<IStoreRepository, StoreRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IReservationRepository, ReservationRepository>();
builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();
//builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();

//builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining(typeof(ICommandHandler<>)));
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
builder.Services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(UpdateInventoryCommandHandler).Assembly));
builder.Services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(CancelReservationCommandHandler).Assembly));
builder.Services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(CreateReservationCommandHandler).Assembly));
builder.Services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(GetProductAvailabilityQueryHandler).Assembly));
builder.Services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(GetProductByIdQueryHandler).Assembly));
builder.Services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(GetProductsByCategoryQueryHandler).Assembly));
builder.Services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(GetReservationByIdQueryHandler).Assembly));
builder.Services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(GetNearbyStoresQueryHandler).Assembly));
builder.Services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(GetStoreByIdQueryHandler).Assembly));
builder.Services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(GetStoreProductsQueryHandler).Assembly));
builder.Services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(GetAvailableStoresConsumer).Assembly));
builder.Services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(UpdateInventoryCommandHandler).Assembly));


// Register Consumers as Scoped

builder.Services.AddHostedService<GetProductByIdConsumer>();
builder.Services.AddHostedService<GetProductsByCategoryConsumer>();
builder.Services.AddHostedService<GetStoreByIdConsumer>();
builder.Services.AddHostedService<GetNearbyStoresConsumer>();
builder.Services.AddHostedService<CreateReservationConsumer>();
builder.Services.AddHostedService<GetReservationByIdConsumer>();
builder.Services.AddHostedService<CancelReservationConsumer>();
builder.Services.AddHostedService<GetProductsStoreByIdConsumer>();
builder.Services.AddHostedService<GetAvailableStoresConsumer>();
builder.Services.AddHostedService<UpdateInventoryConsumer>();


// Register ConsumerManager as Singleton
//builder.Services.AddSingleton<ConsumerManager>();

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

app.UseAuthorization();

app.MapControllers();

// Start all consumers using the ConsumerManager on application startup
//var consumerManager = app.Services.GetRequiredService<ConsumerManager>();
//consumerManager.StartAll();

app.Run();
