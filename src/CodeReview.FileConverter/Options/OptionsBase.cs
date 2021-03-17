using CommandLine;

namespace GodelTech.CodeReview.FileConverter.Options
{
    public abstract class OptionsBase
    {
        [Option('f', "folder", Required = true, HelpText = "Path to folder or file to process")]
        public string Path { get; set; }
        
        [Option('m', "mask", Default = "*", Required = false, HelpText = "Search mask used to look for files within folder")]
        public string SearchMask { get; set; }

        [Option('r', "recurse", Default = true, Required = false, HelpText = "Specifies if recurse search must be used for for files in folder")]
        public bool RecurseSearch { get; set; }

        [Option('o', "output", Required = true, HelpText = "Output file path")]
        public string OutputPath { get; set; }
    }
}