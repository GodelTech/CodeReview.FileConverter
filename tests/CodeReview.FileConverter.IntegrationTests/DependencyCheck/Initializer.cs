using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using GodelTech.CodeReview.FileConverter.Commands;
using GodelTech.CodeReview.FileConverter.Services;
using GodelTech.CodeReview.FileConverter.Utils;

namespace CodeReview.FileConverter.IntegrationTests.DependencyCheck
{
    [SetUpFixture]
    public class Initializer
    {
        public static IConvertDependencyCheckCommand TestSubject;
        private ServiceProvider _serviceProvider;

        [OneTimeSetUp]
        public void RunBeforeAnyTests()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddLogging(x =>
            {
                x.ClearProviders();
                x.AddProvider(new SimplifiedConsoleLoggerProvider());
            });

            serviceCollection.AddSingleton<IPathService, PathService>();
            serviceCollection.AddSingleton<IFileService, FileService>();
            serviceCollection.AddTransient<IFileListResolver, FileListResolver>();
            serviceCollection.AddSingleton<IDirectoryService, DirectoryService>();
            serviceCollection.AddSingleton<IIssueIdGenerator, IssueIdGenerator>();
            serviceCollection.AddTransient<IDependencyCheckFileConverter, DependencyCheckFileConverter>();
            serviceCollection.AddTransient<IIssuePersister, IssuePersister>();
            serviceCollection.AddTransient<IConvertDependencyCheckCommand, ConvertDependencyCheckCommand>();

            _serviceProvider = serviceCollection.BuildServiceProvider();
            TestSubject = _serviceProvider.GetRequiredService<IConvertDependencyCheckCommand>();
        }

        [OneTimeTearDown]
        public void RunAfterAllTests()
        {
            _serviceProvider.Dispose();
        }
    }
}
