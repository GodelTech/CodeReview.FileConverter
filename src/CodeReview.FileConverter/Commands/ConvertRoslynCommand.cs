using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GodelTech.CodeReview.FileConverter.Models;
using GodelTech.CodeReview.FileConverter.Options;
using GodelTech.CodeReview.FileConverter.Services;
using Microsoft.Extensions.Logging;

namespace GodelTech.CodeReview.FileConverter.Commands
{
    public class ConvertRoslynCommand : IConvertRoslynCommand
    {
        private readonly IRoslynIssueConverter _issueConverter;
        private readonly IDetailsDictionaryProvider _detailsDictionaryProvider;
        private readonly IFileListResolver _fileListResolver;
        private readonly IIssuePersister _issuePersister;
        private readonly ILogger<ConvertRoslynCommand> _logger;

        public ConvertRoslynCommand(
            IRoslynIssueConverter issueConverter,
            IDetailsDictionaryProvider detailsDictionaryProvider,
            IFileListResolver fileListResolver,
            IIssuePersister issuePersister,
            ILogger<ConvertRoslynCommand> logger)
        {
            _issueConverter = issueConverter ?? throw new ArgumentNullException(nameof(issueConverter));
            _detailsDictionaryProvider = detailsDictionaryProvider ?? throw new ArgumentNullException(nameof(detailsDictionaryProvider));
            _fileListResolver = fileListResolver;
            _issuePersister = issuePersister ?? throw new ArgumentNullException(nameof(issuePersister));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public async Task<int> ExecuteAsync(RoslynOptions options)
        {
            if (options == null) 
                throw new ArgumentNullException(nameof(options));
            
            _logger.LogInformation("Loading details dictionaries. Folder = {folderPath} ...", options.DictionariesPath);

            var details = _detailsDictionaryProvider.GetDetails(options.DictionariesPath);
            
            _logger.LogInformation("Details dictionaries loaded");
            
            _logger.LogInformation("Converting Roslyn files...");

            await _issuePersister.SaveAsync(options.OutputPath, GetAllIssues(options, details));

            _logger.LogInformation("Roslyn files converted.");

            return Constants.SuccessExitCode;
        }

        private IEnumerable<Issue> GetAllIssues(
            RoslynOptions options,
            IReadOnlyDictionary<string, DiagnosticDetails> diagnosticDetailsMap)
        {
            foreach (var filePath in _fileListResolver.ResolveFiles(options))
            {
                _logger.LogInformation("Processing file. File={filePath}...", filePath);

                foreach (var issue in _issueConverter.Convert(filePath, options.SourceFilesPathPrefix, diagnosticDetailsMap))
                {
                    yield return issue;
                }

                _logger.LogInformation("Processing completed.");
            }
        }
    }
}
