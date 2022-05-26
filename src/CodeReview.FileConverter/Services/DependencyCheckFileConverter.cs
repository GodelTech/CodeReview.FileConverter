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

            var dependencies = await GetVulnerableDependencies(filePath);

            foreach (var dependency in dependencies)
            {
                issues.AddRange(GetIssues(dependency));
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
                .Dependencies ?? Enumerable.Empty<Dependency>();
        }

        private IEnumerable<Issue> GetIssues(Dependency dependency)
        {
            if (IsValnurableDependency(dependency))
            {
                foreach (var vulnerability in dependency.Vulnerabilities)
                {
                    yield return new Issue
                    {
                        Id = _idGenerator.GetNext(),
                        Title = "Component with known vulnerabilities discovered",
                        Description = GetDescriptionForValnurableDependency(dependency.FileName, vulnerability.Description),
                        Category = "External valnurable dependency",
                        DetailsUrl = vulnerability.DetailsUrl,
                        Level = ConvertLevel(vulnerability.Severity),
                        Message = GetMessage(vulnerability.Cvssv3, vulnerability.Cvssv2),
                        RuleId = vulnerability.Name,
                        Locations = new[]
                        {
                            GetIssueLocations(dependency)
                        },
                        Properties = AddProperties(vulnerability.Cvssv2, vulnerability.Cvssv3)
                    };
                }
            }
            else
            {
                yield return new Issue
                {
                    Id = _idGenerator.GetNext(),
                    RuleId = "Component",
                    Message = "Component does not contain known vulnarability",
                    Title = "Component without discovered vulnerabilities",
                    Description = GetDescriptionForDependency(dependency.FileName),
                    Category = "External dependency",
                    Level = IssueLevel.None,
                    Locations = new[]
                    {
                        GetIssueLocations(dependency)
                    }
                };
            }
        }

        private Dictionary<string, string> AddProperties(Cvssv2 cvssv2, Cvssv3 cvssv3)
        {
            var properties = new Dictionary<string, string>();

            var score = cvssv3?.BaseScore ?? cvssv2?.Score;

            if(score != null)
            {
                properties.Add("score", score.ToString());
            }

            return properties;
        }

        private string GetDescriptionForDependency(string fileName)
        {
            return $"Project uses dependency {fileName}.";
        }

        private bool IsValnurableDependency(Dependency dependency)
        {
            return dependency.Vulnerabilities != null && dependency.Vulnerabilities.Any();
        }

        private string GetDescriptionForValnurableDependency(string fileName, string description)
        {
            return $"Project uses vulnerable dependency {fileName}. {description}";
        }

        private string GetMessage(Cvssv3 cvssv3, Cvssv2 cvssv2)
        {
            if (cvssv3 == null)
            {
                return cvssv2 != null ? BuildMessageForCvssv2(cvssv2) : string.Empty;
            }

            return BuildMessageForCvssv3(cvssv3);
        }

        private string BuildMessageForCvssv3(Cvssv3 cvssv3)
        {
            return $"Common Vulnerability Scoring System of 3 version. Defenition: BaseScore {cvssv3.BaseScore}," +
                   $" AttackVector {cvssv3.AttackVector}, AttackComplexity {cvssv3.AttackComplexity}," +
                   $" PrivilegesRequired {cvssv3.PrivilegesRequired}, UseInteraction {cvssv3.UserInteraction}," +
                   $" Scope {cvssv3.Scope}, ConfidentialityImpact {cvssv3.ConfidentialityImpact}," +
                   $" IntegrityImpact {cvssv3.IntegrityImpact}, AvailabilityImpact {cvssv3.AvailabilityImpact}," +
                   $" BaseSeverity {cvssv3.BaseSeverity}";
        }

        private string BuildMessageForCvssv2(Cvssv2 cvssv2)
        {
            return $"Common Vulnerability Scoring System of 2 version. Defenition: Score {cvssv2.Score}," +
                    $" AccessVector {cvssv2.AccessVector}, AccessComplexity {cvssv2.AccessComplexity}," +
                    $" Authenticationr {cvssv2.Authenticationr}, ConfidentialImpact {cvssv2.ConfidentialImpact}," +
                    $" IntegrityImpact {cvssv2.IntegrityImpact}, AvailabilityImpact {cvssv2.AvailabilityImpact}," +
                    $" Severity {cvssv2.Severity}, Version {cvssv2.Version}," +
                    $" ExploitabilityScore {cvssv2.ExploitabilityScore}, ImpactScore {cvssv2.ImpactScore}";
        }

        private IssueLocation GetIssueLocations(Dependency dependency)
        {
            return new IssueLocation
            {
                FilePath = $"{dependency.FilePath}/{dependency.FileName}",
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
