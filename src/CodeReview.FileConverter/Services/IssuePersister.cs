using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using GodelTech.CodeReview.FileConverter.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace GodelTech.CodeReview.FileConverter.Services
{
    public class IssuePersister : IIssuePersister
    {
        public async Task SaveAsync(string filePath, IEnumerable<Issue> issues)
        {
            if (issues == null) 
                throw new ArgumentNullException(nameof(issues));
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(filePath));

            await using var file = File.OpenWrite(filePath);
            await using var output = new GZipStream(file, CompressionLevel.Optimal);
            await using var textWriter = new StreamWriter(output);
            using var jsonTextWriter = new JsonTextWriter(textWriter);

            var jsonSerializer = new JsonSerializer
            {
                Formatting = Formatting.Indented,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            jsonSerializer.Serialize(
                jsonTextWriter,
                issues);
        }
    }
}