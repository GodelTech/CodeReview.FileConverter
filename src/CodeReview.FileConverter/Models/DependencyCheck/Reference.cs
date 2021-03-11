using System.Text.Json.Serialization;

namespace GodelTech.CodeReview.FileConverter.Models.DependencyCheck
{
    public class Reference
    {
        [JsonConstructor]
        public Reference(string url, string name)
        {
            Url = url;
            Name = name;
        }

        public string Url { get; }
        public string Name { get; }
    }
}
