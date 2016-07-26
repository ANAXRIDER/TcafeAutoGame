using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace TcafeAutoGame
{
    class Config
    {
        private string url;
        public string URL
        {
            get { return url; }
            private set { url = value; }
        }
        private string id;
        public string ID
        {
            get { return id; }
            private set { id = value; }
        }

        private string password;
        public string PASSWORD
        {
            get { return password; }
            private set { password = value; }
        }

        private readonly string FILENAME = ".\\TcafeAutoGame.ini";
        private readonly string SECTION_SITE = "Site";
        private readonly string SECTION_ACCOUNT = "Account";

        public Config()
        {
            Load();
        }

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string appName, string keyName, string lpString, string lpFileName);

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string appName, string keyName, string defStr, StringBuilder retStr, int size, string fileName);

        public bool Save(string url, string id, string pw)
        {
            if (URL.Equals(url) && ID.Equals(id) && PASSWORD.Equals(pw))
                return false;

            URL = url;
            ID = id;
            PASSWORD = pw;
            WritePrivateProfileString(SECTION_SITE, "Url", url, FILENAME);
            WritePrivateProfileString(SECTION_ACCOUNT, "Id", id, FILENAME);
            WritePrivateProfileString(SECTION_ACCOUNT, "Password", pw, FILENAME);
            return true;
        }

        public void Load()
        {
            StringBuilder addr = new StringBuilder(128);
            GetPrivateProfileString(SECTION_SITE, "Url", "http://tcaferr.com", addr, 128, FILENAME);
            URL = addr.ToString();
            StringBuilder account = new StringBuilder(32);
            GetPrivateProfileString(SECTION_ACCOUNT, "Id", "", account, 32, FILENAME);
            ID = account.ToString();
            StringBuilder pw = new StringBuilder(64);
            GetPrivateProfileString(SECTION_ACCOUNT, "Password", "", pw, 64, FILENAME);
            PASSWORD = pw.ToString();            
        }

    }
}
