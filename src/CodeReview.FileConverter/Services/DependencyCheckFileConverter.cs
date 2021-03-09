using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using GodelTech.CodeReview.FileConverter.Models;
using GodelTech.CodeReview.FileConverter.Models.DependencyCheck;

namespace GodelTech.CodeReview.FileConverter.Services
{
    public class DependencyCheckFileConverter : IDependencyCheckFileConverter
    {
        private readonly IFileService _fileService;
        private readonly IIssueIdGenerator _idGenerator;

        public DependencyCheckFileConverter(IFileService fileService, IIssueIdGenerator idGenerator)
        {
            _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
            _idGenerator = idGenerator ?? throw new ArgumentNullException(nameof(idGenerator));
        }

        public async Task<IEnumerable<Issue>> Convert(string filePath)
        {
            var issues = new List<Issue>();

            var vulnerableDependencies = await GetVulnerableDependencies(filePath);

            foreach (var vulnerableDependency in vulnerableDependencies)
            {
                issues.AddRange(GetIssues(vulnerableDependency));
            }

            return issues;
        }

        private async Task<IEnumerable<Dependency>> GetVulnerableDependencies(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(filePath));

            await using var fileStream = _fileService.OpenRead(filePath);

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,

            };

            var dependencyCheckResult = await JsonSerializer.DeserializeAsync<DependencyCheckResult>(fileStream, options);

            return dependencyCheckResult
                .Dependencies?
                .Where(x => x.Vulnerabilities != null && x.Vulnerabilities.Any()) 
                   ?? Enumerable.Empty<Dependency>();
        }

        private IEnumerable<Issue> GetIssues(Dependency vulnerableDependency)
        {
            foreach (var vulnerability in vulnerableDependency.Vulnerabilities)
            {
                yield return new Issue
                {
                    Id = _idGenerator.GetNext(),
                    Title = $"Solution contains vulnerable library {vulnerableDependency.FileName}",
                    Description = vulnerability.Description,
                    Category = "Using Components with Known Vulnerabilities",
                    Tags = GetTags(),
                    DetailsUrl = vulnerability.DetailsUrl,
                    Level = ConvertLevel(vulnerability.Severity),
                    Message = GetMessage(vulnerability.Cvssv3),
                    RuleId = vulnerability.Name,
                    Locations = new[]
                    {
                        GetIssueLocations(vulnerableDependency)
                    }
                };
            }
        }

        private string[] GetTags()
        {
            return new[]
            {
                "Security",
                "OWASP A9:2017",
            };
        }

        private string GetMessage(Cvssv3 cvssv3)
        {
            return $"Common Vulnerability Scoring System rates: BaseScore {cvssv3.BaseScore}," +
                   $" AttackVector {cvssv3.AttackVector}, AttackComplexity {cvssv3.AttackComplexity}," +
                   $" PrivilegesRequired {cvssv3.PrivilegesRequired}, UseInteraction {cvssv3.UserInteraction}," +
                   $" Scope {cvssv3.Scope}, ConfidentialityImpact {cvssv3.ConfidentialityImpact}," +
                   $" IntegrityImpact {cvssv3.IntegrityImpact}, AvailabilityImpact {cvssv3.AvailabilityImpact}," +
                   $" BaseSeverity {cvssv3.BaseSeverity}";
        }

        private IssueLocation GetIssueLocations(Dependency vulnerableDependency)
        {
            return new IssueLocation
            {
                FilePath = vulnerableDependency.FilePath,
            };
        }

        private IssueLevel ConvertLevel(string severity)
        {
            return severity switch
            {
                "None" => IssueLevel.None,
                "Low" => IssueLevel.Warning,
                _ => IssueLevel.Error
            };
        }
    }
}
