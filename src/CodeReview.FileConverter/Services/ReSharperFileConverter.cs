using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using ReviewItEasy.FileConverter.Models;

namespace ReviewItEasy.FileConverter.Services
{
    public class ReSharperFileConverter : IReSharperFileConverter
    {
        private static readonly Dictionary<string, IssueLevel> SeverityToIssueLevelMap = new(StringComparer.OrdinalIgnoreCase)
        {
            ["INFO"] = IssueLevel.Info,
            ["HINT"] = IssueLevel.Info,
            ["SUGGESTION"] = IssueLevel.Info,
            ["WARNING"] = IssueLevel.Warning,
            ["ERROR"] = IssueLevel.Error
        };

        private readonly IFileService _fileService;
        private readonly IIssueIdGenerator _idGenerator;

        public ReSharperFileConverter(IFileService fileService, IIssueIdGenerator idGenerator)
        {
            _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
            _idGenerator = idGenerator ?? throw new ArgumentNullException(nameof(idGenerator));
        }
        
        public IEnumerable<Issue> Convert(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(filePath));

            using var input = _fileService.OpenRead(filePath);
            
            var document = XDocument.Load(input);
            var issueTypesMap = GetIssueTypes(document).ToDictionary(x => x.id, x => x, StringComparer.OrdinalIgnoreCase);
            return GetIssues(issueTypesMap, document);
        }

        private static IEnumerable<(string id, string category, string subCategory, string description, string severity, string wikiUrl)> GetIssueTypes(XNode document)
        {
            return document.XPathSelectElements("/Report/IssueTypes/IssueType").Select(x => (
                (string)x.Attribute("Id"),
                (string)x.Attribute("Category"),
                (string)x.Attribute("SubCategory"),
                (string)x.Attribute("Description"),
                (string)x.Attribute("Severity"),
                (string)x.Attribute("WikiUrl")
            ));
        }

        private IEnumerable<Issue> GetIssues(IReadOnlyDictionary<string, (string id, string category, string subCategory, string description, string severity, string wikiUrl)> issueTypes, XNode document)
        {
            foreach (var item in document.XPathSelectElements("/Report/Issues/Project/Issue"))
            {
                var issueTypeId = (string)item.Attribute("TypeId");
                var issueType = issueTypes[issueTypeId];

                yield return new Issue
                {
                    Id = _idGenerator.GetNext(),
                    Title = issueType.subCategory ?? issueType.category,
                    Description = issueType.description,
                    Category = issueType.category,
                    Tags = Array.Empty<string>(),
                    DetailsUrl = issueType.wikiUrl,
                    Level = ConvertLevel(issueType.severity),
                    Message = (string) item.Attribute("Message"),
                    RuleId = issueTypeId,
                    Locations = new[]
                    {
                        GetLocation(
                            (string)item.Attribute("File"), 
                            (string)item.Attribute("Line"))
                    }
                };
            }
        }

        private static IssueLevel ConvertLevel(string severity)
        {
            return SeverityToIssueLevelMap.ContainsKey(severity) ? SeverityToIssueLevelMap[severity] : IssueLevel.Warning;
        }

        private IssueLocation GetLocation(string filePath, string lineValue)
        {
            return new()
            {
                FilePath = filePath.Replace("\\", "/"),
                Region = GetRegion(lineValue)
            };
        }

        private IssueRegion GetRegion(string lineValue)
        {
            if (string.IsNullOrWhiteSpace(lineValue))
                return null;

            var parts = lineValue.Split("-");
            
            var startLine = int.Parse(parts[0]);
            var endLine = parts.Length == 1 ? startLine : int.Parse(parts[1]);

            return new IssueRegion
            {
                StartLine = startLine,
                EndLine = endLine
            };
        }
    }
}