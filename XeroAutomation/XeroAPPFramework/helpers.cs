using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.IO;

namespace XeroAPPFramework
{
    public class CompanyData
    {
        public string companyName;
        public string statusCode;
        public string serverName;
        public List<string> receiveDate;
        public List<decimal> payloadCount;

     }

    public class PayloadRow
    {
        public string receivedDate;
        public string serverName;
        public decimal statusCode;
        public decimal payload;
        public string domain;
        public PayloadRow(string receivedDate, string serverName, decimal status, decimal payload, string domain)
        {
            this.receivedDate = receivedDate;
            this.serverName = serverName;
            this.statusCode = status;
            this.payload = payload;
            this.domain = domain;

        }
    }

    public enum PayloadType { SUCCESSFUL=1, ERROR=0};

    public class Customer
    {
        public string email;
        public string domain;
        public Customer(string email, string domain)
        {
            this.email = email;
            this.domain = domain;
        }
    }

    /// <summary>
    /// AN. Implemented a simple singleton class to ensure that the same file is accessed and written to from different classes
    /// TODO: Lookup Singleton pattern and correct any errors here
    /// </summary>
    public class Logger
    {
        static Logger logger;
        static bool flag;
        public static string folderpath;
        string logfile;
        private Logger()
        {
            this.logfile = Logger.folderpath + "\\logfile_"+System.DateTime.Today.ToString("dd.MM.yyyy")+".txt";
            if (!Directory.Exists(folderpath))
            {
                Directory.CreateDirectory(folderpath);
            }
        }

        public static Logger getInstanceObject()
        {
            if (flag == false)
            {
                logger = new Logger();
                flag = true;
                return logger;
            }
            else
            {
                //future object request returns an already created object
                return logger;
            }
        }

        public static void log(string exceptionname, string message)
        {
            Logger.getInstanceObject().log2File(exceptionname, message);
        }

        void log2File(string exceptionname, string message)
        {
            FileStream WriteHandle = default(FileStream);
            StreamWriter sWriter = default(StreamWriter);
            WriteHandle = new FileStream(logfile, FileMode.Append, FileAccess.Write);
            if ((WriteHandle != null))
            {

                sWriter = new StreamWriter(WriteHandle);
                sWriter.WriteLine("[" + DateTime.Now + "]:[" + exceptionname + "] - " + message);
                sWriter.Close();
            }
        }

    }

}