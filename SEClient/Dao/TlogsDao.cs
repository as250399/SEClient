using Newtonsoft.Json;
using SEClient.Contract;
using SEClient.Helper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Web;

namespace SEClient.Dao
{
    public class TlogsDao
    {
        public static event EventHandler UpdateProgressBar;

        //protected static void OnUpdateProgressBar(UpdateProgressBarEventArgs e)
        //{
        //    EventHandler handler = UpdateProgressBar;
        //    handler?.Invoke(null, e);
        //}

        private static DBConnection LoadDBconnectionFromJsonFile()
        {
            //string path = Directory.GetCurrentDirectory();
            string JsonFileName = HttpContext.Current.Server.MapPath("/") + "\\Helper\\DBConnection.json";
            //string JsonFileName = Path.Combine(path, "DBConnection.json");
            using (StreamReader r = new StreamReader(JsonFileName))
            {
                string json = r.ReadToEnd();
                var root = JsonConvert.DeserializeObject<DBConnection>(json);

                return root;
            }
        }
        private static SqlConnection OpenConnectionToDB()
        {
            var DBconnection = LoadDBconnectionFromJsonFile();
            string connetionString;
            SqlConnection cnn;
            connetionString = string.Format("Data Source={0};Initial Catalog={1};User ID={2};Password={3}", DBconnection.DBserverName, DBconnection.DBname, DBconnection.DBUserName, DBconnection.DBUserPassword);
            cnn = new SqlConnection(connetionString);
            return cnn;
        }

        public static List<List<string>> GetAllTlogsFromDB()
        {

            SqlConnection cnn = OpenConnectionToDB();
            cnn.Open();

            //Get number of Tlogs
            SqlCommand command = new SqlCommand("Select count(*) from TranLogText", cnn);
            Int32 NumberOfTlogs = (Int32)command.ExecuteScalar();

            //Get last TranHeaderIK
            command = new SqlCommand("Select top 1 TranHeaderIK from TranLogText order by TranHeaderIK desc", cnn);
            long lastTranHeaderIK = (long)command.ExecuteScalar();
            //close connection
            cnn.Dispose();
            cnn.Close();

            var ListOfTlogs = new List<string>();
            //need it for very large amount of tlogs
            var ListOfListOfTlogs = new List<List<string>>();
            var ProgresBarCounter = 0;
            var IterationNumber = Math.Ceiling((decimal)lastTranHeaderIK / ConstantsVariables.MaxSizeDBMessages);

            for (int i = 0; i < IterationNumber; i++)
            {
                cnn = OpenConnectionToDB();
                cnn.Open();

                long MinTranHeaderIK = i * ConstantsVariables.MaxSizeDBMessages;
                long MaxTranHeaderIK = (i + 1) * ConstantsVariables.MaxSizeDBMessages;
                //Get All Tlog CompressedXML
                string query = String.Format("select CompressedXML from TranLogText where TranHeaderIK>={0} and TranHeaderIK<{1}", MinTranHeaderIK, MaxTranHeaderIK);
                command = new SqlCommand(query, cnn);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    ListOfTlogs = new List<string>();
                    while (reader.Read())
                    {
                        var Tlog = DecompressXMLtoString((byte[])reader["CompressedXML"]);
                        ListOfTlogs.Add(Tlog);
                        ProgresBarCounter++;
                        //OnUpdateProgressBar(new UpdateProgressBarEventArgs() { ProgresBarCurrentValue = ProgresBarCounter, ProgresBarMax = NumberOfTlogs });
                    }
                }
                cnn.Dispose();
                cnn.Close();
                ListOfListOfTlogs.Add(ListOfTlogs);
            }

            return ListOfListOfTlogs;

        }

        private static string DecompressXMLtoString(byte[] TlogCompressed)
        {
            try
            {
                if (TlogCompressed == null)
                {
                    return null;
                }
                using (var inStream = new MemoryStream(TlogCompressed))
                {
                    using (Stream csStream = new GZipStream(inStream, CompressionMode.Decompress))
                    {
                        using (var outStream = new MemoryStream())
                        {
                            var buffer = new byte[4096];
                            int nRead;
                            while ((nRead = csStream.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                outStream.Write(buffer, 0, nRead);
                            }
                            return Encoding.ASCII.GetString(outStream.ToArray());
                        }
                    }
                }
            }
            catch
            {
                return null;
            }
        }

    }
}