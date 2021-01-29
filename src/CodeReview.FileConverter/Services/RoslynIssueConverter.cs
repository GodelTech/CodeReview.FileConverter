using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using ReviewItEasy.FileConverter.Models;

namespace ReviewItEasy.FileConverter.Services
{
    public class RoslynIssueConverter : IRoslynIssueConverter
    {
        private const string FileUriProtocol = "file://";

        private readonly IFileService _fileService;
        private readonly IIssueIdGenerator _idGenerator;

        public RoslynIssueConverter(
            IFileService fileService,
            IIssueIdGenerator idGenerator)
        {
            _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
            _idGenerator = idGenerator ?? throw new ArgumentNullException(nameof(idGenerator));
        }

        public IEnumerable<Issue> Convert(string filePath, string srcFolderPrefix, IReadOnlyDictionary<string, DiagnosticDetails> diagnosticDetailsMap)
        {
            if (diagnosticDetailsMap == null) 
                throw new ArgumentNullException(nameof(diagnosticDetailsMap));
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(filePath));
            if (string.IsNullOrWhiteSpace(srcFolderPrefix))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(srcFolderPrefix));

            using var fileStream = _fileService.OpenRead(filePath);
            using var streamReader = new StreamReader(fileStream);
            using var jsonReader = new JsonTextReader(streamReader);
            
            var roslynResult = new JsonSerializer()
                .Deserialize<RoslynResult>(jsonReader);

            return 
                (from run in roslynResult?.Runs ?? Array.Empty<RoslynRun>() 
                    from issue in run.Results ?? Array.Empty<RoslynIssue>()
                 select ConvertToIssue(issue, srcFolderPrefix, diagnosticDetailsMap))
                .ToArray();
        }

        private Issue ConvertToIssue(RoslynIssue issue, string srcFolderPrefix, IReadOnlyDictionary<string, DiagnosticDetails> diagnosticDetailsMap)
        {
            diagnosticDetailsMap.TryGetValue(issue.RuleId, out var diagnosticDetails);

            return new Issue
            {
                Category = diagnosticDetails?.Category,
                Description = diagnosticDetails?.Description,
                DetailsUrl = diagnosticDetails?.HelpLinkUri,
                Id = _idGenerator.GetNext(),
                Title = diagnosticDetails?.Title,
                Tags = diagnosticDetails?.CustomTags,
                Level = ConvertLevel(issue.Level),
                Message = issue.Message,
                RuleId = issue.RuleId,
                Locations = ConvertLocations(issue.Locations ?? Array.Empty<RoslynIssueLocation>(), srcFolderPrefix)
            };
        }

        private static IssueLevel ConvertLevel(string level)
        {
            return Enum.TryParse<IssueLevel>(level, true, out var result) ? result : IssueLevel.Info;
        }

        private static IssueLocation[] ConvertLocations(RoslynIssueLocation[] issueLocations, string srcFolderPrefix)
        {
            return
                (from location in issueLocations
                select new IssueLocation
                {
                    FilePath = CalculateFilePath(location.ResultFile.Uri, srcFolderPrefix),
                    Region = ConvertRegion(location.ResultFile.Region)
                })
                .ToArray();
        }

        private static IssueRegion ConvertRegion(FileRegion fileRegion)
        {
            if (fileRegion == null)
                return null;

            return new IssueRegion
            {
                StartLine = fileRegion.StartLine,
                EndLine = fileRegion.EndLine
            };
        }

        private static string CalculateFilePath(string fileUri, string srcFolderPrefix)
        {
            if (string.IsNullOrWhiteSpace(fileUri))
                return string.Empty;

            if (fileUri.Length < FileUriProtocol.Length)
                return string.Empty;

            fileUri = fileUri.Substring(FileUriProtocol.Length);

            if (fileUri.Length < srcFolderPrefix.Length)
                return string.Empty;


            return fileUri.Substring(srcFolderPrefix.Length).Replace("\\", "/");
        }
    }
}
