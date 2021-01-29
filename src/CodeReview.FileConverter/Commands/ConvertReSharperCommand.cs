using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GodelTech.CodeReview.FileConverter.Models;
using GodelTech.CodeReview.FileConverter.Options;
using GodelTech.CodeReview.FileConverter.Services;
using Microsoft.Extensions.Logging;

namespace GodelTech.CodeReview.FileConverter.Commands
{
    public class ConvertReSharperCommand : IConvertReSharperCommand
    {
        private readonly IReSharperFileConverter _fileConverter;
        private readonly IFileListResolver _fileListResolver;
        private readonly IIssuePersister _issuePersister;
        private readonly ILogger<ConvertReSharperCommand> _logger;

        public ConvertReSharperCommand(
            IReSharperFileConverter fileConverter,
            IFileListResolver fileListResolver,
            IIssuePersister issuePersister,
            ILogger<ConvertReSharperCommand> logger)
        {
            _fileConverter = fileConverter ?? throw new ArgumentNullException(nameof(fileConverter));
            _fileListResolver = fileListResolver ?? throw new ArgumentNullException(nameof(fileListResolver));
            _issuePersister = issuePersister ?? throw new ArgumentNullException(nameof(issuePersister));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<int> ExecuteAsync(ReSharperOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            
            _logger.LogInformation("Converting ReSharper files...");

            await _issuePersister.SaveAsync(options.OutputPath, GetAllIssues(options));

            _logger.LogInformation("ReSharper files converted.");
            
            return Constants.SuccessExitCode;
        }

        private IEnumerable<Issue> GetAllIssues(ReSharperOptions options)
        {
            foreach (var filePath in _fileListResolver.ResolveFiles(options))
            {
                _logger.LogInformation("Processing file. File={filePath}...", filePath);
                
                foreach (var issue in _fileConverter.Convert(filePath))
                {
                    yield return issue;
                }
                
                _logger.LogInformation("Processing completed.");
            }
        }
    }
}