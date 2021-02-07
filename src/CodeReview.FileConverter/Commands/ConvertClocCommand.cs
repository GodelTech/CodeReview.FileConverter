using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GodelTech.CodeReview.FileConverter.Models;
using GodelTech.CodeReview.FileConverter.Options;
using GodelTech.CodeReview.FileConverter.Services;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace GodelTech.CodeReview.FileConverter.Commands
{
    /// <summary>
    /// In order to produce proper input file the following command must be used:
    /// cloc . --by-file --report-file=report.json --json
    /// </summary>
    public class ConvertClocCommand : IConvertClocCommand
    {
        private readonly IFileService _fileService;
        private readonly ILogger<ConvertClocCommand> _logger;

        public ConvertClocCommand(
            IFileService fileService,
            ILogger<ConvertClocCommand> logger)
        {
            _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public async Task<int> ExecuteAsync(ClocOptions options)
        {
            if (options == null) 
                throw new ArgumentNullException(nameof(options));

            if (!_fileService.Exists(options.Path))
            {
                _logger.LogError("Input file was not found. File = {filePath}", options.OutputPath);
                return Constants.ErrorExitCode;
            }
            
            _logger.LogInformation("Reading data from input file...");
            
            var clocContent = await _fileService.ReadAllTextAsync(options.Path);
            var model = JsonConvert.DeserializeObject<Dictionary<string, ClocFileDetails>>(clocContent);

            _logger.LogInformation("Data was read and deserialized");
            _logger.LogInformation("Processing data...");

            var result =
                (from item in model
                    let filePath = item.Key
                    let details = item.Value
                    where filePath.StartsWith(options.PathPrefixToRemove)
                    select new
                    {
                        FilePath = filePath.Substring(options.PathPrefixToRemove.Length),
                        Details = new FileLocDetails
                        {
                            Blank = details.Blank,
                            Commented = details.Comment,
                            Code = details.Code,
                            Language = details.Language
                        }
                    })
                .ToDictionary(x => x.FilePath, x => x.Details);

            var json = JsonConvert.SerializeObject(result, Formatting.Indented, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });

            await _fileService.WriteAllTextAsync(options.OutputPath, json);

            _logger.LogInformation("Data was saved. File = {filePath}, Data Item Count = {itemCount}", options.Path, result.Count);

            return Constants.SuccessExitCode;
        }
    }
}
