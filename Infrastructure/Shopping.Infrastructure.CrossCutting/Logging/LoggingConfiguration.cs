using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Enrichers.Span;
using Serilog.Events;
using Serilog.Exceptions;

namespace Shopping.Infrastructure.CrossCutting.Logging;

public class LoggingConfiguration
{
    public static Action<HostBuilderContext, LoggerConfiguration> ConfigureLogger(SerilogConfiguration serilogConfiguration) =>
        (context, configuration) =>
        {
            var applicationName = context.HostingEnvironment.ApplicationName;

            configuration
                .Enrich.WithSpan()
                .Enrich.WithEnvironmentName()
                .Enrich.WithMachineName()
                .Enrich.WithExceptionDetails()
                .Enrich.WithProperty("ApplicationName", applicationName);

            if (context.HostingEnvironment.IsDevelopment())
            {
                configuration.WriteTo.Console().MinimumLevel.Information();
                configuration.WriteTo.Seq(serilogConfiguration.ServerUrl, LogEventLevel.Information,
                    apiKey: serilogConfiguration.ApiKey);
            }
            else if (context.HostingEnvironment.IsProduction())
            {
                configuration.WriteTo.Console().MinimumLevel.Error();
                configuration.WriteTo.Seq(serilogConfiguration.ServerUrl, LogEventLevel.Error,
                    apiKey: serilogConfiguration.ApiKey);
            }
        };
}

public record SerilogConfiguration(string ApiKey,string ServerUrl);