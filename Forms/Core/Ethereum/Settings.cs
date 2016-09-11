using Plugin.Settings;
using Plugin.Settings.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamplyGame.Ethereum
{
    public static class Settings
    {
        private static ISettings AppSettings
        {
            get
            {
                return CrossSettings.Current;
            }
        }

        #region Setting Constants

        private const string UrlKey = "url_key";
        private static readonly string UrlDefault = "https://morden.infura.io/aEcNY6wGN4KuEpoXQRxZ";
        private const string PrivatekeyKey = "privatekey_key";
        private static readonly string PrivateKeyDefault = "0xaf98a1bdf2140578318e2c5e7d5956a3ee0a6732090c2991a9166a6639ad368f";

        #endregion


        public static string UrlSetting
        {
            get
            {
                return AppSettings.GetValueOrDefault<string>(UrlKey, UrlDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue<string>(UrlDefault, value);
            }
        }


        public static string PrivateKeySetting
        {
            get
            {
                return AppSettings.GetValueOrDefault<string>(PrivatekeyKey, PrivateKeyDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue<string>(PrivatekeyKey, value);
            }
        }

    }
}
