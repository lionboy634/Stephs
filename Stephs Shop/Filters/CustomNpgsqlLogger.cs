using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql.Logging;
using Stephs_Shop.Models;
using System;

namespace Stephs_Shop.Filters
{
	public class CustomNpgsqlLogger : NpgsqlLogger
	{
		private readonly ILogger<CustomNpgsqlLogger> logger;
		public CustomNpgsqlLogger(ILogger<CustomNpgsqlLogger> logger)
		{
			this.logger = logger;
		}
		public override bool IsEnabled(NpgsqlLogLevel level)
		{
			return this.logger.IsEnabled(this.ToMyLogLevel(level));
		}
		public override void Log(NpgsqlLogLevel level, int connectorId, string msg, Exception exception = null)
		{
			this.logger.Log(this.ToMyLogLevel(level), exception, $"{connectorId} : {msg}");
		}
		private LogLevel ToMyLogLevel(NpgsqlLogLevel logLevel)
		{
			return logLevel switch
			{
				NpgsqlLogLevel.Debug => LogLevel.Debug,
				NpgsqlLogLevel.Error => LogLevel.Error,
				NpgsqlLogLevel.Fatal => LogLevel.Critical,
				NpgsqlLogLevel.Info => LogLevel.Information,
				NpgsqlLogLevel.Trace => LogLevel.Trace,
				NpgsqlLogLevel.Warn => LogLevel.Warning,
				_ => LogLevel.None,
			};
		}
	}


	public class MyNgpsqlLoggingProvider : INpgsqlLoggingProvider
	{
		private readonly IServiceProvider serviceProvider;
		public MyNgpsqlLoggingProvider(IServiceProvider serviceProvider)
		{
			this.serviceProvider = serviceProvider;
		}
		public NpgsqlLogger CreateLogger(string name) => this.serviceProvider.GetService<CustomNpgsqlLogger>();
	}


}
