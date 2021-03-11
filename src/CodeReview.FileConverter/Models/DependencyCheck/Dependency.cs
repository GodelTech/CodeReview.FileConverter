using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GodelTech.CodeReview.FileConverter.Models.DependencyCheck
{
    public class Dependency
    {
        [JsonConstructor]
        public Dependency(string fileName, string filePath, IEnumerable<Vulnerability> vulnerabilities)
        {
            FileName = fileName;
            FilePath = filePath;
            Vulnerabilities = vulnerabilities;
        }

        public string FileName { get; }
        public string FilePath { get; }
        public IEnumerable<Vulnerability> Vulnerabilities { get; }
    }
}
