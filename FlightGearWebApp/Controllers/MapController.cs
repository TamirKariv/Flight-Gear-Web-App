using System;
using System.Web.Mvc;
using FlightGearWebApp.Models;
using System.Xml;
using System.Text;
using System.IO;

namespace FlightGearWebApp.Controllers {
    public class MapController : Controller {

        /// <summary>
        /// default action
        /// </summary>
        /// <returns></returns>
        public ActionResult Index() {
            return View();
        }

        /// <summary>
        /// display the plane once
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public ActionResult DisplayOneTime(string ip, int port)
        {
            Client.Instance.Info.IP = ip;
            Client.Instance.Info.Port = port;
            Client.Instance.EstablishConnection();
            return View();
        }

        /// <summary>
        /// display the path of the plane
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="interval"></param>
        /// <returns></returns>
        public ActionResult DisplayPath(string ip, int port, int interval)
        {
            Client.Instance.Info.IP = ip;
            Client.Instance.Info.Port = port;
            Client.Instance.EstablishConnection();
            Session["Interval"] = interval;
            return View();
        }

        /// <summary>
        /// save the path of the plane to a file
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="interval"></param>
        /// <param name="duration"></param>
        /// <param name="dir"></param>
        /// <returns></returns>
        public ActionResult Save(string ip, int port, int interval, int duration, string dir)
        {
            Client.Instance.Info.IP = ip;
            Client.Instance.Info.Port = port;
            dir = AppDomain.CurrentDomain.BaseDirectory + dir + ".txt";
            Client.Instance.Info.Directory = dir;
            Client.Instance.DeleteFileIfExists();
            Client.Instance.EstablishConnection();
            Session["Interval"] = interval;
            Session["Duration"] = duration;
            return View();
        }

        /// <summary>
        /// load and display the plane's path from a file
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="interval"></param>
        /// <returns></returns>
        public ActionResult LoadDisplay(string dir, int interval)
        {
            Client.Instance.Info.Directory = AppDomain.CurrentDomain.BaseDirectory + dir + ".txt";
            Session["Interval"] = interval;
            return View();
        }

        /// <summary>
        /// get the values via the xml
        /// </summary>
        /// <param name="cl"></param>
        /// <returns></returns>
        private string ToXml(Client cl)
        {
            StringBuilder sb = new StringBuilder();
            XmlWriterSettings writesettings = new XmlWriterSettings();
            XmlWriter xmlWriter = XmlWriter.Create(sb, writesettings);
            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("SimulatorValues");
            cl.Info.ToXml(xmlWriter);
            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndDocument();
            xmlWriter.Flush();
            return sb.ToString();
        }

        /// <summary>
        /// load and parse the values from the file
        /// </summary>
        [HttpPost]
        public void LoadFromFile() {
            string dir = Client.Instance.Info.Directory;
            StreamReader reader = new StreamReader(dir, Encoding.UTF8);
            // throws exception if there is not a file text.
            if (reader == null)
            {
                throw new IOException("Error text file is missing");
            }
            string contents = reader.ReadToEnd();
            string[] seperators = { "\r\n", ","};
            // splits the content of the file according to the separators.
            string[] vals = contents.Split(seperators, System.StringSplitOptions.RemoveEmptyEntries);                
            Client.Instance.Values = vals;
            Client.Instance.StopReading = false;
            Client.Instance.CurrentValue = 0;
            reader.Close();
        }

        /// <summary>
        /// get the coordinates from the simulator
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public string GetCoordinates() {
            Client.Instance.GetValuesFromSimulator();
            return ToXml(Client.Instance);
        }

        /// <summary>
        ///  get the coordinates from the file
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public string GetCoordinatesFromFile()
        {
            Client.Instance.GetValuesFromFile();
            return ToXml(Client.Instance);
        }

        /// <summary>
        /// write the cordinates to the file
        /// </summary>
        [HttpPost]
        public void WriteCoordinates()
        {
            Client.Instance.WriteCoordinates();
        }
    }
}