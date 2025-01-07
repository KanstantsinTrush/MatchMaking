using MatchMaking.Infrastructure.Kafka;
using MatchMaking.Infrastructure.Kafka.Configuration;
using MatchMaking.Infrastructure.Redis;
using MatchMaking.Service;
using MatchMaking.Service.API.BackgroundServices.KafkaConsumers;
using MatchMaking.Service.DBCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

builder.Host
    .UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services));

builder.Services.AddControllers();
builder.Services.AddServices();
builder.Services.AddRepositories();
builder.Services
    .AddKafkaProducerFactory()
    .AddOptions<KafkaProducerOptions>()
    .BindConfiguration("KafkaProducer")
    .ValidateOnStart()
    .ValidateDataAnnotations();

builder.Services
    .AddKafkaConsumerFactory()
    .AddOptions<KafkaConsumerOptions>()
    .BindConfiguration("KafkaConsumer")
    .ValidateOnStart()
    .ValidateDataAnnotations();

builder.Services.AddRedis(builder.Environment, builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHostedService<MatchCompleteEventConsumingService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    options.RoutePrefix = string.Empty;
});

app.MapControllers();

app.Run();