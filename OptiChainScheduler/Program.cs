using DBModel.ClassicCalendarDbContext;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.EntityFrameworkCore;
using OptiChainScheduler;
using OptiChainScheduler.BackGroundJobs;
using OptiChainScheduler.NseApiService.NseIndexApiService;

var builder = Host.CreateApplicationBuilder(args);

// DB connection
builder.Services.AddDbContext<ClassicalCalendarContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PgConnectionString")));

// ✅ Add Hangfire with PostgreSQL storage
builder.Services.AddHangfire(config =>
    config.UsePostgreSqlStorage(options =>
    {
        options.UseNpgsqlConnection(builder.Configuration.GetConnectionString("PgConnectionString"));
    }));

// ✅ Add Hangfire server
builder.Services.AddHangfireServer();

// ✅ Register job classes
builder.Services.AddScoped<ClassicalCalendarJobs>();
builder.Services.AddScoped<NseIndexApiService>();

// ✅ Add Worker
builder.Services.AddHostedService<Worker>();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
