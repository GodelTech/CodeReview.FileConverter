using CommandLine;

namespace ReviewItEasy.FileConverter.Options
{
    [Verb("resharper", HelpText = "Convert JetBrains ReSharper output into unified format.")]
    public class ReSharperOptions : OptionsBase
    {
    }
}