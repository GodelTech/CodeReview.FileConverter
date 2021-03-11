using CommandLine;

namespace GodelTech.CodeReview.FileConverter.Options
{
    [Verb("dependencyCheck", HelpText = "Convert OWASP Dependency check output into unified format.")]
    public class DependencyCheckOptions : OptionsBase
    {
    }
}
