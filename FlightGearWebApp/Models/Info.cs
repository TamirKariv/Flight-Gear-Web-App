using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

namespace FlightGearWebApp.Models
{
    public class Info
    {
        /// <summary>
        /// propeties (getters and setters)
        /// </summary>
        public string IP { get; set; }
        public int Port { get; set; }
        public string Directory { get; set; }
        public double Lat { get; set; }
        public double Lon { get; set; }
        public double Throttle { get; set; }
        public double Rudder { get; set; }

        /// <summary>
        /// write the data to the xml
        /// </summary>
        /// <param name="writer"></param>
        public void ToXml(XmlWriter writer)
        {
            // start writing the elements
            writer.WriteStartElement("SimulatorValues");
            writer.WriteElementString("Lat", Lat.ToString());
            writer.WriteElementString("Lon", Lon.ToString());
            writer.WriteElementString("Throttle", Throttle.ToString());
            writer.WriteElementString("Rudder", Rudder.ToString());
            // end of writing the elements
            writer.WriteEndElement();
        }
    }
}