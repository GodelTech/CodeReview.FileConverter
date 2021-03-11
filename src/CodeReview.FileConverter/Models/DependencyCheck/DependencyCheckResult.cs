using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GodelTech.CodeReview.FileConverter.Models.DependencyCheck
{
    public class DependencyCheckResult
    {
        [JsonConstructor]
        public DependencyCheckResult(IEnumerable<Dependency> dependencies)
        {
            Dependencies = dependencies;
        }

        public IEnumerable<Dependency> Dependencies { get; }
    }
}
