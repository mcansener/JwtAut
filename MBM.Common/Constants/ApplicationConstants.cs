using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBM.Common.Constants
{
    public static class ApplicationConstants
    {
        /// <summary>
        /// Represents the application name.
        /// </summary>
        public static readonly string ApplicationName;

        /// <summary>
        /// Represents the environment of the application.
        /// </summary>
        public static readonly string EnvironmentName;

        /// <summary>
        /// Represents the operation system of the application.
        /// </summary>
        public static readonly string OperatingSystem;

        /// <summary>
        /// Represents process id of the application.
        /// </summary>
        public static readonly string ProcessId;

        /// <summary>
        /// Represents application settings json file name.
        /// Ex: appsettings.json
        /// </summary>
        public static readonly string AppSettingsFileName;

        /// <summary>
        /// Represents hosting json file name.
        /// Ex: hosting.json
        /// </summary>
        public static readonly string HostingFileName;

        /// <summary>
        /// Represents hosting Operating System specific json file name.
        /// Ex: hosting.{OsPlatform}.json
        /// </summary>
        public static readonly string HostingOsFileName;

        /// <summary>
        /// Represents console logging level switch to change logging level in runtime.
        /// </summary>
        public static readonly LoggingLevelSwitch ConsoleLogLevel;

        /// <summary>
        /// Execution directory path of the application
        /// </summary>
        public static readonly string ExecutionPath;

        /// <summary>
        /// Domain restirctions list
        /// </summary>
        public static readonly List<string> AllowedDomainList;

        /// <summary>
        /// This is the constructor of application constants to use them application wide.
        /// </summary>
        static ApplicationConstants()
        {
            // Initialize application name
            ApplicationName = System.Reflection.Assembly.GetEntryAssembly()!.GetName().Name!;

            // Initialize the environment. It can be only Development or Staging or Production
            var environment = GetEnvironmentFromArguments() ?? Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            EnvironmentName = environment is "Development" or "Staging" or "Production" ? environment : "Production";
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", environment);

            // Initialize execution directory path
            ExecutionPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            // Initialize operating system name
            OperatingSystem = Environment.OSVersion.Platform.ToString();

            // Initialize ProcessId of the application
            ProcessId = Environment.ProcessId.ToString();

            // Initialize application settings file names
            AppSettingsFileName = "appsettings.json";
            HostingFileName = "hosting.json";
            HostingOsFileName = $"hosting.{OperatingSystem}.json";

            // Initialize console log level switch to be able to change log level in runtime
            ConsoleLogLevel = new LoggingLevelSwitch();

            AllowedDomainList = new List<string>() { "localhost", "mybestman" };
        }

        private static string GetEnvironmentFromArguments()
        {
            var arguments = Environment.GetCommandLineArgs();
            var environmentArgument = arguments.FirstOrDefault(arg => arg.StartsWith("--environment", StringComparison.OrdinalIgnoreCase));
            if (string.IsNullOrWhiteSpace(environmentArgument))
                return null;

            if (environmentArgument.Contains('='))
            {
                var environment = environmentArgument.Split('=')[1].Trim();
                return string.IsNullOrWhiteSpace(environment) ? null : environment;
            }

            for (var i = 0; i < arguments.Length; ++i)
            {
                if (!arguments[i].Equals("--environment", StringComparison.OrdinalIgnoreCase))
                    continue;

                if (i + 1 < arguments.Length)
                    return arguments[i + 1].Trim();
            }

            return null;
        }
    }
}