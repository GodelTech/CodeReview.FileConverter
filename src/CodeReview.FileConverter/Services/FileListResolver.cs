using System;
using System.Collections.Generic;
using System.IO;
using GodelTech.CodeReview.FileConverter.Options;

namespace GodelTech.CodeReview.FileConverter.Services
{
    public class FileListResolver : IFileListResolver
    {
        private readonly IFileService _fileService;
        private readonly IDirectoryService _directoryService;

        public FileListResolver(IFileService fileService, IDirectoryService directoryService)
        {
            _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
            _directoryService = directoryService ?? throw new ArgumentNullException(nameof(directoryService));
        }
        
        public IEnumerable<string> ResolveFiles(OptionsBase options)
        {
            if (options == null) 
                throw new ArgumentNullException(nameof(options));

            if (_fileService.Exists(options.Path))
            {
                yield return options.Path;
                yield break;
            }

            if (!_directoryService.Exists(options.Path))
                yield break;

            var files = _directoryService.GetFiles(
                options.Path,
                options.SearchMask,
                options.RecurseSearch ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

            foreach (var file in files)
            {
                yield return file;
            }
        }
    }
}