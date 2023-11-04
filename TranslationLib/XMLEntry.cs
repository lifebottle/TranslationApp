using Newtonsoft.Json;
namespace TranslationLib
{
    public class XMLEntry
    {

        public string PointerOffset { get; set; }
        public string VoiceId { get; set; }
        public string JapaneseText { get; set; }
        public string EnglishText { get; set; }
        public string Notes { get; set; }
        public int? Id { get; set; }
        public int? StructId { get; set; }
        public int[] SpeakerId { get; set; }
        public int? UnknownPointer { get; set; }
        public int? MaxLength { get; set; }
        public bool EmbedOffset { get; set; }
        public string hi { get; set; }
        public string lo { get; set; }
        public string _Status { get; set; }
        public string Status
        {
            get
            {
                if (_Status == "Edited")
                {
                    return "Editing";
                }
                if (_Status == "Proofread")
                {
                    return "Proofreading";
                }
                return _Status;
            }
            set
            {
                if (value == "Editing")
                {
                    _Status = "Edited";
                }
                else if (value == "Proofreading")
                {
                    _Status = "Proofread";
                }
                else
                {
                    _Status = value;
                }
            }
        }


        [JsonIgnore] public string SpeakerName { get; set; }
    }
}