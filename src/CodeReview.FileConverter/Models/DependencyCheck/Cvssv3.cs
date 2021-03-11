using System.Text.Json.Serialization;

namespace GodelTech.CodeReview.FileConverter.Models.DependencyCheck
{
    public class Cvssv3
    {
        [JsonConstructor]
        public Cvssv3(
            float baseScore,
            string attackVector,
            string attackComplexity,
            string privilegesRequired,
            string userInteraction,
            string scope,
            string confidentialityImpact,
            string integrityImpact,
            string availabilityImpact,
            string baseSeverity
            )
        {
            BaseScore = baseScore;
            AttackVector = attackVector;
            AttackComplexity = attackComplexity;
            PrivilegesRequired = privilegesRequired;
            UserInteraction = userInteraction;
            Scope = scope;
            ConfidentialityImpact = confidentialityImpact;
            IntegrityImpact = integrityImpact;
            AvailabilityImpact = availabilityImpact;
            BaseSeverity = baseSeverity;
        }

        [JsonNumberHandling(JsonNumberHandling.AllowNamedFloatingPointLiterals
                            | JsonNumberHandling.AllowReadingFromString)]
        public float BaseScore { get; }
        public string AttackVector { get; }
        public string AttackComplexity { get; }
        public string PrivilegesRequired { get; }
        public string UserInteraction { get; }
        public string Scope { get; }
        public string ConfidentialityImpact { get; }
        public string IntegrityImpact { get; }
        public string AvailabilityImpact { get; }
        public string BaseSeverity { get; }
    }
}
