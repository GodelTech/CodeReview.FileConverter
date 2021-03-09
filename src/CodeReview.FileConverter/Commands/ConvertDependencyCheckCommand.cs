using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using GodelTech.CodeReview.FileConverter.Models;
using GodelTech.CodeReview.FileConverter.Options;
using GodelTech.CodeReview.FileConverter.Services;

namespace GodelTech.CodeReview.FileConverter.Commands
{
    public class ConvertDependencyCheckCommand : IConvertDependencyCheckCommand
    {
        private readonly IDependencyCheckFileConverter _dependencyCheckFileConverter;
        private readonly IFileListResolver _fileListResolver;
        private readonly IIssuePersister _issuePersister;
        private readonly ILogger<ConvertReSharperCommand> _logger;

        public ConvertDependencyCheckCommand(
            IDependencyCheckFileConverter dependencyCheckFileConverter,
            IFileListResolver fileListResolver,
            IIssuePersister issuePersister,
            ILogger<ConvertReSharperCommand> logger)
        {
            _dependencyCheckFileConverter = dependencyCheckFileConverter ?? throw new ArgumentNullException(nameof(dependencyCheckFileConverter));
            _fileListResolver = fileListResolver ?? throw new ArgumentNullException(nameof(fileListResolver));
            _issuePersister = issuePersister ?? throw new ArgumentNullException(nameof(issuePersister));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<int> ExecuteAsync(DependencyCheckOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            _logger.LogInformation("Converting Dependency check files...");

            await _issuePersister.SaveAsync(options.OutputPath, await GetAllIssues(options));

            _logger.LogInformation("Dependency check files converted.");

            return Constants.SuccessExitCode;
        }

        private async Task<IEnumerable<Issue>> GetAllIssues(DependencyCheckOptions options)
        {
            var allIssues = new List<Issue>();

            foreach (var filePath in _fileListResolver.ResolveFiles(options))
            {
                _logger.LogInformation("Processing file. File={filePath}...", filePath);

                allIssues.AddRange(await _dependencyCheckFileConverter.Convert(filePath));

                _logger.LogInformation("Processing completed.");
            }

            return allIssues;
        }
    }
}
