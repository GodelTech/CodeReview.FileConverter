using CommandLine;

namespace ReviewItEasy.FileConverter.Options
{
    [Verb("roslyn", HelpText = "Convert .NET Compliper output into unified format.")]
    public class RoslynOptions : OptionsBase
    {
        [Option('d', "dictionaries", Required = false, HelpText = "Path to folder containing dictionaries with additional information about issues")]
        public string DictionariesPath { get; set; } = "./Dictionaries";

        [Option('s', "src", Required = true, HelpText = "Prefix in file path which needs to be removed from location value.")]
        public string SourceFilesPathPrefix { get; set; }
    }
}
