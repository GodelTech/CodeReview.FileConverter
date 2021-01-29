using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace GodelTech.CodeReview.FileConverter.Services
{
    public class DetailsDictionaryProvider : IDetailsDictionaryProvider
    {
        private readonly IDirectoryService _directoryService;
        private readonly IFileService _fileService;
        private readonly ILogger<DetailsDictionaryProvider> _logger;

        public DetailsDictionaryProvider(
            IDirectoryService directoryService,
            IFileService fileService,
            ILogger<DetailsDictionaryProvider> logger)
        {
            _directoryService = directoryService ?? throw new ArgumentNullException(nameof(directoryService));
            _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public IReadOnlyDictionary<string, DiagnosticDetails> GetDetails(string folderPath)
        {
            if (string.IsNullOrWhiteSpace(folderPath))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(folderPath));

            var detailsMap = new Dictionary<string, DiagnosticDetails>(StringComparer.OrdinalIgnoreCase);
            
            foreach (var filePath in _directoryService.GetFiles(folderPath, "*", SearchOption.AllDirectories))
            {
                _logger.LogInformation("Starting processing of details file. File = {filePath}", filePath);

                foreach (var details in GetDetailsFromFile(filePath))
                {
                    detailsMap.TryAdd(details.Id, details);
                }
                
                _logger.LogInformation("Processing completed");
            }

            return detailsMap;
        }

        private IEnumerable<DiagnosticDetails> GetDetailsFromFile(string filePath)
        {
            using var fileStream = _fileService.OpenRead(filePath);
            using var streamReader = new StreamReader(fileStream);
            using var jsonReader = new JsonTextReader(streamReader);
            
            var serializer = new JsonSerializer();
            var packageDetails = serializer.Deserialize<PackageDetails[]>(jsonReader);

            return
                (from package in packageDetails ?? Array.Empty<PackageDetails>() 
                    from diagnostic in package.Diagnostics ?? Array.Empty<DiagnosticDetails>()
                    select diagnostic)
                .ToArray();
        }


        //private 
    }
}