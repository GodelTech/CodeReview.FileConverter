using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using NUnit.Framework;
using GodelTech.CodeReview.FileConverter.Models;
using GodelTech.CodeReview.FileConverter.Options;

namespace CodeReview.FileConverter.IntegrationTests.DependencyCheck
{
    [TestFixture]
    public class Tests
    {
        private const string OutputFolder = "./DependencyCheck/Output";
        private const string ResourcesFolder = "./DependencyCheck/Resources";

        [SetUp]
        public void SetUp()
        {
            if (!Directory.Exists(OutputFolder))
            {
                Directory.CreateDirectory(OutputFolder);
            }
        }

        [TearDown]
        public void TeatDown()
        {
            if (Directory.Exists(OutputFolder))
            {
                Directory.Delete(OutputFolder, true);
            }
        }

        [Test]
        public async Task WhenResultContainsVulnerabilities_ThenIssueAreGeneratedSuccessfully()
        {
            // Arrange
            var testFilePath = $"{ResourcesFolder}/result-with-vulnerabilities.json";
            var outputFilePath = $"{OutputFolder}/result-with-vulnerabilities";
            var options = new DependencyCheckOptions 
            {
                OutputPath = outputFilePath,
                Path = testFilePath
            };

            // Act
            await Initializer.TestSubject.ExecuteAsync(options);

            // Assert
            Assert.True(File.Exists(outputFilePath));
            var issues = await GetIssuesFromFile(outputFilePath);
            Assert.That(issues, Has.Count.EqualTo(3));
            var errrors = issues.Where(x => x.Level == IssueLevel.Error).ToList();
            Assert.That(errrors, Has.Count.EqualTo(1));
            var error = errrors.First();
            Assert.Multiple(() =>
            {
                Assert.That(error.Title, Is.EqualTo("Component with known vulnerabilities discovered"));
                Assert.That(error.Description, Is.EqualTo("Project uses vulnerable dependency Nancy:1.4.0. " +
                                                          "Csrf.cs in NancyFX Nancy before 1.4.4" +
                                                          " and 2.x before 2.0-dangermouse has Remote Code Execution via" +
                                                          " Deserialization of JSON data in a CSRF Cookie."));
                Assert.That(error.Category, Is.EqualTo("External valnurable dependency"));
                Assert.That(error.DetailsUrl, Is.EqualTo("https://ossindex.sonatype.org/vuln/" +
                                                         "0a5d262f-b9cb-46fe-924e-98d33711ae04?component-type=nuget&component-name=Nancy&" +
                                                         "utm_source=dependency-check&utm_medium=integration&utm_content=6.1.1"));
                Assert.That(error.RuleId, Is.EqualTo("CVE-2017-9785"));
                Assert.That(error.Level, Is.EqualTo(IssueLevel.Error));
                Assert.That(error.Message, Is.EqualTo("Common Vulnerability Scoring System of 3 version. Defenition: BaseScore 9.8," +
                                                      " AttackVector N, AttackComplexity L," +
                                                      " PrivilegesRequired N, UseInteraction N," +
                                                      " Scope U, ConfidentialityImpact H," +
                                                      " IntegrityImpact H, AvailabilityImpact H," +
                                                      " BaseSeverity CRITICAL"));
                Assert.That(error.Locations, Has.Length.EqualTo(1));
                var location = error.Locations.First();
                Assert.That(location.FilePath, Is.EqualTo("/src/VulnerableLibraries.csproj/Nancy:1.4.0"));
                var property = error.Properties.First(x => x.Key.Equals("score"));
                Assert.That(property.Value, Is.EqualTo("9.8"));
            });
        }

        [Test]
        public async Task WhenResultContainsFloatValue_ThenIssueAreGeneratedSuccessfully()
        {
            // Arrange
            var testFilePath = $"{ResourcesFolder}/result-with-float-value.json";
            var outputFilePath = $"{OutputFolder}/result-with-float-value";

            var options = new DependencyCheckOptions
            {
                OutputPath = outputFilePath,
                Path = testFilePath
            };

            // Act
            await Initializer.TestSubject.ExecuteAsync(options);

            // Assert
            Assert.True(File.Exists(outputFilePath));
            var issues = await GetIssuesFromFile(outputFilePath);
            Assert.That(issues, Has.Count.EqualTo(3));
            var errrors = issues.Where(x => x.Level == IssueLevel.Error).ToList();
            Assert.That(errrors, Has.Count.EqualTo(1));
            var error = errrors.First();
            Assert.That(error.Message, Is.EqualTo("Common Vulnerability Scoring System of 3 version. Defenition: BaseScore 9.8," +
                                                  " AttackVector N, AttackComplexity L," +
                                                  " PrivilegesRequired N, UseInteraction N," +
                                                  " Scope U, ConfidentialityImpact H," +
                                                  " IntegrityImpact H, AvailabilityImpact H," +
                                                  " BaseSeverity CRITICAL"));
        }

        [Test]
        public async Task WhenResultDoesNotContainsVulnerabilities_ThenIssueAreEmpty()
        {
            // Arrange
            var testFilePath = $"{ResourcesFolder}/result-without-vulnerabilities.json";
            var outputFilePath = $"{OutputFolder}/result-without-vulnerabilities";

            var options = new DependencyCheckOptions
            {
                OutputPath = outputFilePath,
                Path = testFilePath
            };

            // Act
            await Initializer.TestSubject.ExecuteAsync(options);

            // Assert
            Assert.True(File.Exists(outputFilePath));
            var issues = await GetIssuesFromFile(outputFilePath);
            Assert.That(issues, Has.Count.EqualTo(3));
            var errrors = issues.Where(x => x.Level == IssueLevel.Error).ToList();
            Assert.That(errrors, Is.Empty);
        }

        [Test]
        public async Task WhenResultIsEmptyObject_ThenIssueAreEmpty()
        {
            // Arrange
            var testFilePath = $"{ResourcesFolder}/empty-result.json";
            var outputFilePath = $"{OutputFolder}/empty-result";

            var options = new DependencyCheckOptions
            {
                OutputPath = outputFilePath,
                Path = testFilePath
            };

            // Act
            await Initializer.TestSubject.ExecuteAsync(options);

            // Assert
            Assert.True(File.Exists(outputFilePath));
            var issues = await GetIssuesFromFile(outputFilePath);
            Assert.That(issues, Is.Empty);
        }

        [Test]
        public async Task WhenResultDoesNotContainVulnerabilities_ThenIssuesAreEmpty()
        {
            // Arrange
            var testFilePath = $"{ResourcesFolder}/dependency-check-with-vulnerabilities.json";
            var outputFilePath = $"{OutputFolder}/dependency-check-with-vulnerabilities";

            var options = new DependencyCheckOptions
            {
                OutputPath = outputFilePath,
                Path = testFilePath
            };

            // Act
            await Initializer.TestSubject.ExecuteAsync(options);

            // Assert
            Assert.True(File.Exists(outputFilePath));
            var issues = await GetIssuesFromFile(outputFilePath);
            Assert.That(issues, Is.Empty);
        }

        [Test]
        public async Task WhenResultDoesNotContainDependencies_ThenIssuesAreEmpty()
        {
            // Arrange
            var testFilePath = $"{ResourcesFolder}/result-with-empty-dependencies.json";
            var outputFilePath = $"{OutputFolder}/result-with-empty-dependencies";

            var options = new DependencyCheckOptions
            {
                OutputPath = outputFilePath,
                Path = testFilePath
            };

            // Act
            await Initializer.TestSubject.ExecuteAsync(options);

            // Assert
            Assert.True(File.Exists(outputFilePath));
            var issues = await GetIssuesFromFile(outputFilePath);
            Assert.That(issues, Is.Empty);
        }

        private async Task<IEnumerable<Issue>> GetIssuesFromFile(string resultFilePath)
        {
            await using var file = File.OpenRead(resultFilePath);
            await using var decompressedStream = new GZipStream(file, CompressionMode.Decompress);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters =
                {
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                }
            };

            return await JsonSerializer.DeserializeAsync<IEnumerable<Issue>>(decompressedStream, options);
        }
    }
}