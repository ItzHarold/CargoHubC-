using Backend.Features.Clients;
using Backend.Features.Contacts;
using Backend.Features.Inventories;
using Backend.Features.ItemGroups;
using Backend.Features.ItemLines;
using Backend.Features.Items;
using Backend.Features.ItemTypes;
using Backend.Features.Locations;
using Backend.Features.Orders;
using Backend.Features.Shipments;
using Backend.Features.Suppliers;
using Backend.Features.Transfers;
using Backend.Features.Logs;
using Backend.Features.Warehouses;
using Backend.Infrastructure.Database;
using Backend.Infrastructure.Middleware;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;

namespace Backend;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Configuration.AddJsonFile("rolesConfig.json", optional: false, reloadOnChange: true);

        builder.Services.AddDbContext<CargoHubDbContext>(options =>
            options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

        ConfigureServices(builder.Services);

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseMiddleware<LoggingMiddleware>();
        app.UseMiddleware<ApiKeyMiddleware>();
        app.UseAuthorization();

        app.MapControllers();

        app.Urls.Add("http://localhost:5031");

        app.Run();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddAuthorization();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddLogging();
        services.AddControllers();

        // FluentValidation configuration
        services.AddValidatorsFromAssemblyContaining<ClientValidator>();
        services.AddValidatorsFromAssemblyContaining<ContactValidator>();
        services.AddValidatorsFromAssemblyContaining<InventoryValidator>();
        services.AddValidatorsFromAssemblyContaining<ItemGroupValidator>();
        services.AddValidatorsFromAssemblyContaining<ItemLineValidator>();
        services.AddValidatorsFromAssemblyContaining<ItemTypeValidator>();
        services.AddValidatorsFromAssemblyContaining<LocationValidator>();
        services.AddValidatorsFromAssemblyContaining<WarehouseValidator>();



        services.AddTransient<IClientService, ClientService>();
        services.AddTransient<IWarehouseService, WarehouseService>();
        services.AddTransient<IContactService, ContactService>();
        services.AddTransient<ITransferService, TransferService>();
        services.AddTransient<ILocationService, LocationService>();
        services.AddTransient<IItemService, ItemService>();
        services.AddTransient<IInventoryService, InventoryService>();
        services.AddTransient<IItemGroupService, ItemGroupService>();
        services.AddTransient<IItemTypeService, ItemTypeService>();
        services.AddTransient<IItemLineService, ItemLineService>();
        services.AddTransient<IShipmentService, ShipmentService>();
        services.AddTransient<IOrderService, OrderService>();
        services.AddTransient<ISupplierService, SupplierService>();
        services.AddTransient<ILogService, LogService>();
    }
}
