using CommandLine;

namespace GodelTech.CodeReview.FileConverter.Options
{
    [Verb("cloc", HelpText = "Convert cloc tool YAML output into format supported by Evaluator")]
    public class ClocOptions
    {
        [Option('f', "file", Required = true, HelpText = "Path to file containing YAML output of cloc tool")]
        public string Path { get; set; }

        [Option('o', "output", Required = true, HelpText = "Output file path")]
        public string OutputPath { get; set; }

        [Option('p', "prefix", Default = "./", Required = false, HelpText = "File path prefix to remove")]
        public string PathPrefixToRemove { get; set; }
    }
}