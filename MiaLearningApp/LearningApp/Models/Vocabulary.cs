namespace LearningApp.Models
{
    public class Vocabulary
    {
        public int VocabularyId { get; set; }

        // Indicate the source and target language
        public string SourceLanguage { get; set; } = "English";
        public string TargetLanguage { get; set; } = "Spanish";

        // The actual word or phrase in the source language
        public string SourceTerm { get; set; } = "";

        // The corresponding translation
        public string TargetTerm { get; set; } = "";

        // Additional info: e.g., explanation, usage example
        public string Explanation { get; set; } = "";
        public string Example { get; set; } = "";
        public bool Mastered { get; set; }
    }
}
