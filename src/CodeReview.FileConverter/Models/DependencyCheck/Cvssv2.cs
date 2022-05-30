using System.Text.Json.Serialization;

namespace GodelTech.CodeReview.FileConverter.Models.DependencyCheck
{
    public class Cvssv2
    {
        [JsonConstructor]
        public Cvssv2(
            float score,
            string accessVector,
            string accessComplexity,
            string authenticationr,
            string confidentialImpact,
            string integrityImpact,
            string availabilityImpact,
            string severity,
            string version,
            string exploitabilityScore,
            string impactScore
            )
        {
            Score = score;
            AccessVector = accessVector;
            AccessComplexity = accessComplexity;
            Authenticationr = authenticationr;
            ConfidentialImpact = confidentialImpact;
            IntegrityImpact = integrityImpact;
            AvailabilityImpact = availabilityImpact;
            Severity = severity;
            Version = version;
            ExploitabilityScore = exploitabilityScore;
            ImpactScore = impactScore;
        }

        [JsonNumberHandling(JsonNumberHandling.AllowNamedFloatingPointLiterals
                            | JsonNumberHandling.AllowReadingFromString)]
        public float Score { get; }
        public string AccessVector { get; }
        public string AccessComplexity { get; }
        public string Authenticationr { get; }
        public string ConfidentialImpact { get; }
        public string IntegrityImpact { get; }
        public string AvailabilityImpact { get; }
        public string Severity { get; }
        public string Version { get; }
        public string ExploitabilityScore { get; }
        public string ImpactScore { get; }
    }
}
