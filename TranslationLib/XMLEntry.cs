using Newtonsoft.Json;
namespace TranslationLib
{
    public class XMLEntry
    {
        public int? Id { get; set; }
        public string PointerOffset { get; set; }
        public string JapaneseText { get; set; }
        public string EnglishText { get; set; }
        public string Status { get; set; }
        public string Notes { get; set; }
        public int[] SpeakerId { get; set; }
        public int? UnknownPointer { get; set; }
        [JsonIgnore] public string SpeakerName { get; set; }
    }
}