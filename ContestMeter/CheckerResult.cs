﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
