using MatchMaking.Infrastructure.Kafka;
using MatchMaking.Infrastructure.Kafka.Configuration;
using MatchMaking.Infrastructure.Redis;
using MatchMaking.Worker;
using MatchMaking.Worker.BackgroundServices;
using MatchMaking.Worker.BLCore;
using MatchMaking.Worker.BLCore.Options;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddServices();

builder.Services
    .AddKafkaProducerFactory()
    .AddOptions<KafkaProducerOptions>()
    .BindConfiguration("KafkaProducer")
    .ValidateOnStart();

builder.Services
    .AddKafkaConsumerFactory()
    .AddOptions<KafkaConsumerOptions>()
    .BindConfiguration("KafkaConsumer")
    .ValidateOnStart();

builder.Services.Configure<MatchOptions>(builder.Configuration.GetSection(nameof(MatchOptions)));

builder.Services.AddRedis(builder.Environment, builder.Configuration);

builder.Services.AddHostedService<MatchMakingRequestEventConsumingService>();
builder.Services.AddHostedService<MatchMakerService>();

var host = builder.Build();
host.Run();