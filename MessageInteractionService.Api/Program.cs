using FastEndpoints;
using MessageInteractionService.Api.Mappings;
using MessageInteractionService.Api.Services;
using MessageInteractionService.Core;
using MessageInteractionService.Storage;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Templates;

const string logTemplate =
    """
    {@t:yyyy/MM/dd HH:mm:ss} [{@l} - {SourceContext}] {@m}
    {#if IsDefined(@x)}{@x}
    {#end}
    """;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((context, config) =>
{
    var expressionTemplate = new ExpressionTemplate(logTemplate);
    config.ReadFrom.Configuration(context.Configuration, "Logging");
    config.WriteTo.Console(expressionTemplate);
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddFastEndpoints();
builder.Services.AddAutoMapper(config =>
{
    config.AddProfile<IncomingMessageProfile>();
    config.AddProfile<OutgoingMessageProfile>();
});

builder.Services.AddScoped<IMessageProcessor, MessageProcessor>()
       .AddMessageInteractionCoreServices()
       .AddMessageInteractionStorageServices(options =>
        {
            options.UseNpgsql(builder.Configuration
                                     .GetConnectionString("StorageConnection"));
        });


var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<MessagingDbContext>();
    await dbContext.Database.EnsureCreatedAsync();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseFastEndpoints();
app.Run();