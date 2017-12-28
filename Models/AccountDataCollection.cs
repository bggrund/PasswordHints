using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace PasswordHints
{
    public class AccountDataCollection
    {
        #region Private Members

        #endregion

        public static List<AccountData> GetAccountData(string accountDataFilePath)
        {
            List<AccountData> items;
            if (File.Exists(accountDataFilePath))
            {
                using (FileStream f = new FileStream(accountDataFilePath, FileMode.Open))
                {
                    items = (List<AccountData>)(new XmlSerializer(typeof(List<AccountData>))).Deserialize(f);
                }
            }
            else
            {
                File.Create(accountDataFilePath);
                items = new List<AccountData>();
            }

            return items;
        }

        public static void SaveAccountData(string accountDataFilePath, List<AccountData> items)
        {
            using (FileStream f = new FileStream(accountDataFilePath, FileMode.Create))
            {
                (new XmlSerializer(typeof(List<AccountData>))).Serialize(f, items);
            }
        }
    }
}
