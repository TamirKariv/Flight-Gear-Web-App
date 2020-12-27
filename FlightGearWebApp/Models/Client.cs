using System;
using System.Xml;
using System.Threading;
using System.Net.Sockets;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace FlightGearWebApp.Models {

    /// <summary>
    /// the client
    /// </summary>
    public class Client {
        // log in to the flight simulator
        private TcpClient tcpclient = new TcpClient();
        private bool isRunning = false;
        public bool StopReading { get; set; } = true;
        public string[] Values { get; set; }
        public Info Info { get; set; } = new Info();
        public int CurrentValue { get; set; } = 0;

        private static Client instance = null;
        /// <summary>
        /// returns the client
        /// </summary>
        public static Client Instance
        {
            get
            {
                // if there is not a client, creates a new client.
                if (instance == null)
                {
                    instance = new Client();
                }
                return instance;
            }
        }

        /// <summary>
        /// Connect to the simulator
        /// </summary>
        public void EstablishConnection()
        {
            // if we are not connected to the simulator, we try to connect
            if (!isRunning)
            {
                new Task(() =>
                 {
                     while (!tcpclient.Connected)
                     {
                         try
                         {
                             // we connect using the IP and port number
                             tcpclient.Connect(Info.IP, Info.Port);
                             Thread.Sleep(550);
                         }
                         // throws an exception if there was a problem connecting the simulator
                         catch (SocketException)
                         {
                             Console.WriteLine("Error, Connection to the simulator");
                         }
                     }
                 }).Start();
            }
        }

        /// <summary>
        /// get the values from the simulator
        /// </summary>
        public void GetValuesFromSimulator() {
            if (tcpclient != null) {
                // if the simulator is not empty, we try to get the information about the lon, lat, throttle and rudder.
                NetworkStream writeStream = tcpclient.GetStream();
                Info.Lat = RequestDataFromSimulator(writeStream, "get /position/latitude-deg\r\n");
                Info.Lon = RequestDataFromSimulator(writeStream, "get /position/longitude-deg\r\n");
                Info.Throttle = RequestDataFromSimulator(writeStream, "get /controls/engines/current-engine/throttle\r\n");
                Info.Rudder = RequestDataFromSimulator(writeStream, "get /controls/flight/rudder\r\n");
            }
        }

        /// <summary>
        /// get the values from the file
        /// </summary>
        public void GetValuesFromFile()
           {           
                if(!StopReading && CurrentValue < Values.Length)
                {
                // if we are still reading the information, we parse it
                Info.Lat = (int)double.Parse(Values[CurrentValue]);
                Info.Lon = (int)double.Parse(Values[CurrentValue + 1]);
                Info.Throttle = (int)double.Parse(Values[CurrentValue + 2]);
                Info.Rudder = (int)double.Parse(Values[CurrentValue + 3]);
                CurrentValue += 4;
                }
            // if we got to the values' length, we stop the reading and nullify the CurrentValue.
            else if (!StopReading)
                {
                    StopReading = true;
                    CurrentValue = 0;
                }
        }

        /// <summary>
        /// request a specific value from the simulator
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public int RequestDataFromSimulator(NetworkStream stream, string request)
        {
            int getCount = Encoding.ASCII.GetByteCount(request);
            byte[] send = new byte[getCount];
            send = Encoding.ASCII.GetBytes(request);
            stream.Write(send, 0, send.Length);
            StreamReader rder = new StreamReader(stream);
            string[] split = rder.ReadLine().Split('=');
            // here is the separation of the lines of the missions that we put in the internet line
            string data = split[1].Split('\'')[1];
            return (int)Math.Abs(double.Parse(data));
        }

        /// <summary>
        /// write coordinates to the file
        /// </summary>
        public void WriteCoordinates()
        {
            string toWrite = Math.Abs(Info.Lon).ToString() + ","
                + Math.Abs(Info.Lat).ToString() + ","
                + Math.Abs(Info.Throttle).ToString() + ","
                + Math.Abs(Info.Rudder).ToString();
            using (StreamWriter steamWriter = File.AppendText(Info.Directory))
            {
                // try writing to the file
                try
                {
                    steamWriter.WriteLineAsync(toWrite);
                }
                // if not succeeds writing to file, throws an IOException.
                catch (IOException)
                {
                    Console.WriteLine("Error Writing to file");
                }
            }
        }

        /// <summary>
        /// delete the file if exists
        /// </summary>
        public void DeleteFileIfExists()
        {
            if (File.Exists(Info.Directory))
            {
                File.Delete(Info.Directory);
            }
        }
    }
}
