using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;


namespace TranslationApp
{

	//Serialize each Entry node
	[XmlRoot(ElementName = "Entry")]
	public class Entry
	{
		[XmlElement(ElementName = "PointerOffset")]
		public string PointerOffset { get; set; }

		[XmlElement(ElementName = "JapaneseText")]
		public string JapaneseText { get; set; }

		[XmlElement(ElementName = "EnglishText")]
		public string EnglishText { get; set; }

		[XmlElement(ElementName = "Notes")]
		public string Notes { get; set; }

		[XmlElement(ElementName = "Status")]
		public string Status { get; set; }

		[XmlElement(ElementName = "Id")]
		public string Id { get; set; }

		[XmlElement(ElementName = "GroupId")]
		public string GroupId { get; set; }
	}

	[XmlRoot(ElementName = "Strings")]
	public class Strings
	{
		[XmlElement(ElementName = "Section")]
		public string Section { get; set; }
		[XmlElement(ElementName = "Entry")]
		public List<Entry> Entries { get; set; }
	}

	[XmlRoot(ElementName = "SceneText")]
	public class TORStory : TalesFile
	{
	
	}

	[XmlRoot(ElementName = "MenuText")]
	public class Menu : TalesFile
	{

	}

	public class TalesFile
	{
		[XmlElement(ElementName = "OriginalName")]
		public string OriginalName { get; set; }

		[XmlElement(ElementName = "Strings")]
		public List<Strings> Strings { get; set; }
	}


	[XmlRoot(ElementName = "SceneText")]
	public class TOPXSceneText
	{
		[XmlElement(ElementName = "Type")]
		public string Type { get; set; }
		[XmlElement(ElementName = "Struct")]
		public List<Struct> Struct { get; set; }

	}



}
