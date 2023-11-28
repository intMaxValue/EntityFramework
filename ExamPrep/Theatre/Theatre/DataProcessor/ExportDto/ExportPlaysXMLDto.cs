using System.Xml.Serialization;

namespace Theatre.DataProcessor.ExportDto
{
    [XmlType("Play")]
    public class ExportPlaysXMLDto
    {
        [XmlAttribute("Title")]
        public string Title { get; set; }

        [XmlAttribute]
        public string Duration { get; set; }

        [XmlAttribute]
        public string Rating { get; set; }

        [XmlAttribute]
        public string Genre { get; set; }

        [XmlArray("Actors")]
        [XmlArrayItem("Actor")]
        public ExportActorsXMLDto[] Actors { get; set; }
    }
}
