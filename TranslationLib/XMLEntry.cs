using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text.RegularExpressions;

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
        public int? BubbleId { get; set; }
        public int? SubId { get; set; }
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

        public bool IsFound(string text, bool matchWholeEntry, bool matchCase, bool matchWholeWord, string language)
        {
            string textCompare = "";
            if (language == "Japanese")
                textCompare = JapaneseText;
            else
                textCompare = EnglishText;
                
            if (matchWholeEntry)            
                return textCompare != null ? textCompare == text : false;
            
            if (matchCase)
                return textCompare != null ? textCompare.Contains(text) : false;

            if (matchWholeWord)
            {
                string keywords = $@"\b{text}\b";
                return textCompare != null ? Regex.Match(textCompare, keywords).Success : false;
            }

            return textCompare != null ? textCompare.IndexOf(text, System.StringComparison.OrdinalIgnoreCase) > 0 : false;
        }
    }
}