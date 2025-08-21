using ClassicalCalendarRepo;
using DBModel.ClassicCalendarDbContext;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.EntityFrameworkCore;
using NseApi;
using OptiChainScheduler;
using OptiChainScheduler.BackgroundExecutorService;
using OptiChainScheduler.BackGroundJobs;

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
//builder.Services.AddScoped<ClassicalCalendarJobs>();
builder.Services.AddScoped<ClassicalCalendarNewOrderJob>();

builder.Services.AddScoped<NseIndexStrikeLtpHistoryApiService>();
builder.Services.AddScoped<NseIndexOptionChainStrikeApiService>();
builder.Services.AddScoped<StoreLtpSnapshotsDataRepo>();
builder.Services.AddScoped<RecentDateLtpSnapshotsDataRepo>();

builder.Services.AddScoped<ActiveClassicalCalendarRepo>();
builder.Services.AddScoped<ExecuteClassicalCalendarRepo>();

// ✅ Add Worker
//builder.Services.AddHostedService<DataFetchWorker>();
builder.Services.AddHostedService<ExecuteNewOrderWorker>();

var host = builder.Build();
host.Run();
