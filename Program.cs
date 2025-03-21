using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using RestEase;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http;
using SkepsTicket.Infra.RestEase;
using MongoDB.Driver;
using SkepsTicket.Mongo;
using SkepsTicket.Model;
using SkepsTicket.Services.Interfaces;
using SkepsTicket.Services;
using SkepsTicket.Strategy;
using SkepsTicket.Mongo.Interfaces;
using SkepsTicket.Mongo.Model;
using SkepsTicket.Infra.Interfaces;
using SkepsTicket.Infra;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.Configure<EmpresasConfig>(builder.Configuration);

        // Adiciona suporte para injeção de dependência
        builder.Services.AddSingleton(sp =>
        {
            var config = sp.GetRequiredService<IConfiguration>();
            var empresasConfig = new EmpresasConfig();
            config.Bind(empresasConfig);
            return empresasConfig;
        });

        builder.Services.AddSingleton<IMongoClient>(serviceProvider => new MongoClient("mongodb+srv://admin:flowchat123456@cluster0.dwcxd.mongodb.net"));

        builder.Services.AddHttpClient("MovideskApi", client =>
        {
            client.BaseAddress = new Uri("https://api.movidesk.com/");
        });

        builder.Services.AddHttpClient("SkepsSendMessage", client =>
        {
            client.BaseAddress = new Uri("https://skepsticket-node.azurewebsites.net");
        })
        .AddPolicyHandler(GetRetryPolicy());

        builder.Services.AddHttpClient("SkepsUploadImg", client =>
        {
            client.BaseAddress = new Uri("https://d46pjmuoog.execute-api.sa-east-1.amazonaws.com/");
        })
        .AddPolicyHandler(GetRetryPolicy());

        builder.Services.AddSingleton(serviceProvider =>
        {
            var clientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
            var httpClient = clientFactory.CreateClient("SkepsUploadImg");
            return RestClient.For<IUploadAnexo>(httpClient);
        });

        builder.Services.AddSingleton(serviceProvider =>
        {
            var clientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
            var httpClient = clientFactory.CreateClient("MovideskApi");
            return RestClient.For<IMovideskAPI>(httpClient);
        });

        builder.Services.AddSingleton(serviceProvider =>
        {
            var clientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
            var httpClient = clientFactory.CreateClient("SkepsSendMessage");
            return RestClient.For<ISendMessageBlip>(httpClient);
        });

        builder.Services.AddHttpClient("DefaultClient")
        .AddPolicyHandler(GetRetryPolicy());

        builder.Services.AddSingleton(serviceProvider =>
        {
            var clientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
            return new Func<string, IBlipSender>(baseUrl =>
            {
                var httpClient = clientFactory.CreateClient("DefaultClient");
                httpClient.BaseAddress = new Uri(baseUrl);
                return RestClient.For<IBlipSender>(httpClient);
            });
        });
        builder.Logging.ClearProviders().AddConsole().AddFilter("System.Net.Http.HttpClient", LogLevel.None);
        builder.Services.AddSingleton<IMongoService, MongoService>();
        builder.Services.AddSingleton<IMovideskService, MovideskService>();
        builder.Services.AddSingleton<IBlipService, BlipService>();
        builder.Services.AddSingleton<IEmail, Email>();
        builder.Services.AddSingleton<ILoginService, LoginService>();
        builder.Services.AddSingleton<TicketStrategyFactory>();

        //builder.Services.AddControllers().AddJsonOptions(options =>
        //{
        //    options.JsonSerializerOptions.Converters.Add(new DateTimeConverter());
        //});
        var corsPolicy = "AllowAll";

        builder.Services.AddCors(options =>
        {
            options.AddPolicy(corsPolicy, policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            });
        });

        var app = builder.Build();
        app.UseCors(corsPolicy);

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
    }

    private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError() // Erros transitórios: 5xx e 408
            .OrResult(msg => !msg.IsSuccessStatusCode) // Retenta para status HTTP não bem-sucedidos
            .WaitAndRetryAsync(
                retryCount: 3, // Número de tentativas
                sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)), // Backoff exponencial
                onRetry: (outcome, timespan, retryAttempt, context) =>
                {
                    Console.WriteLine($"Tentativa {retryAttempt} falhou. Retentando em {timespan.Seconds} segundos...");
                });
    }
}