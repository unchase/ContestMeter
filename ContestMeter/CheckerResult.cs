using System.Xml.Serialization;

namespace ContestMeter.Common
{
    [XmlRoot("result")]
    public class CheckerResult
    {
        [XmlText]
        public string Text;

        [XmlAttribute("outcome")]
        public string Outcome;

        [XmlIgnore]
        public bool IsAccepted
        {
            get
            {
                return Outcome == "accepted";
            }
        }
    }
}
