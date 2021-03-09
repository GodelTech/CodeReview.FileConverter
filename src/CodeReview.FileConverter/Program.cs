using System;
using System.IO;
using System.Threading.Tasks;
using CommandLine;
using CommandLine.Text;
using GodelTech.CodeReview.FileConverter.Commands;
using GodelTech.CodeReview.FileConverter.Models;
using GodelTech.CodeReview.FileConverter.Options;
using GodelTech.CodeReview.FileConverter.Services;
using GodelTech.CodeReview.FileConverter.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GodelTech.CodeReview.FileConverter
{
    // 1. If tool is published to NuGet it can be executed within pipline which has access to Docker Service and .NET Core
    // 2. Add support for error hashes. This option should simplify comparison of old and new errors later
    // this feature is not required now but later it might be required to find new issues
    class Program
    {
        private static int Main(string[] args)
        {
            using var container = CreateServiceProvider();

            var parser = new Parser(x =>
            {
                x.HelpWriter = TextWriter.Null;
            });

            var result = parser.ParseArguments<RoslynOptions, ReSharperOptions, ClocOptions, DependencyCheckOptions>(args);

            var exitCode = result
                .MapResult(
                    (RoslynOptions x) => ProcessRoslynResultsAsync(x, container).GetAwaiter().GetResult(),
                    (ReSharperOptions x) => ProcessReSharperResultsAsync(x, container).GetAwaiter().GetResult(),
                    (ClocOptions x) => ProcessClocResultsAsync(x, container).GetAwaiter().GetResult(),
                    (DependencyCheckOptions x) => ProcessDependencyCheckResultsAsync(x, container).GetAwaiter().GetResult(),
                    _ => ProcessErrors(result));

            return exitCode;
        }

        private static Task<int> ProcessDependencyCheckResultsAsync(DependencyCheckOptions options, ServiceProvider container)
        {
            return container.GetRequiredService<IConvertDependencyCheckCommand>().ExecuteAsync(options);
        }

        private static Task<int> ProcessClocResultsAsync(ClocOptions options, IServiceProvider container)
        {
            return container.GetRequiredService<IConvertClocCommand>().ExecuteAsync(options);
        }

        private static Task<int> ProcessRoslynResultsAsync(RoslynOptions options, IServiceProvider container)
        {
            return container.GetRequiredService<IConvertRoslynCommand>().ExecuteAsync(options);
        }

        private static Task<int> ProcessReSharperResultsAsync(ReSharperOptions options, IServiceProvider container)
        {
            return container.GetRequiredService<IConvertReSharperCommand>().ExecuteAsync(options);
        }

        private static int ProcessErrors(ParserResult<object> result)
        {
            var helpText = HelpText.AutoBuild(result, h =>
            {
                h.AdditionalNewLineAfterOption = false;
                return HelpText.DefaultParsingErrorsHandler(result, h);
            }, e => e);

            Console.WriteLine(helpText);

            return Constants.ErrorExitCode;
        }

        private static ServiceProvider CreateServiceProvider()
        {
            var serviceProvider = new ServiceCollection();

            serviceProvider.AddLogging(x =>
            {
                x.ClearProviders();
                x.AddProvider(new SimplifiedConsoleLoggerProvider());
            });

            serviceProvider.AddSingleton<IPathService, PathService>();
            serviceProvider.AddSingleton<IFileService, FileService>();
            serviceProvider.AddSingleton<IDirectoryService, DirectoryService>();

            serviceProvider.AddSingleton<IIssueIdGenerator, IssueIdGenerator>();

            serviceProvider.AddTransient<IRoslynIssueConverter, RoslynIssueConverter>();
            serviceProvider.AddTransient<IReSharperFileConverter, ReSharperFileConverter>();
            serviceProvider.AddTransient<IDependencyCheckFileConverter, DependencyCheckFileConverter>();
            serviceProvider.AddTransient<IFileListResolver, FileListResolver>();
            serviceProvider.AddTransient<IIssuePersister, IssuePersister>();
            serviceProvider.AddTransient<IDetailsDictionaryProvider, DetailsDictionaryProvider>();
            
            serviceProvider.AddTransient<IConvertReSharperCommand, ConvertReSharperCommand>();
            serviceProvider.AddTransient<IConvertRoslynCommand, ConvertRoslynCommand>();
            serviceProvider.AddTransient<IConvertClocCommand, ConvertClocCommand>();
            serviceProvider.AddTransient<IConvertDependencyCheckCommand, ConvertDependencyCheckCommand>();

            return serviceProvider.BuildServiceProvider();
        }
    }
}
