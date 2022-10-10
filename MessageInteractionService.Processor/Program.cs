using MessageInteractionService.Processor;
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
builder.Services.AddHostedService<MessageProcessingWorker>();
var app = builder.Build();
app.Run();
