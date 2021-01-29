using CommandLine;

namespace GodelTech.CodeReview.FileConverter.Options
{
    [Verb("resharper", HelpText = "Convert JetBrains ReSharper output into unified format.")]
    public class ReSharperOptions : OptionsBase
    {
    }
}