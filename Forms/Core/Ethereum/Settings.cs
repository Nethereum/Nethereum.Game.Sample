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
        private static readonly string PrivateKeyDefault = "";

        #endregion


        public static string UrlSetting
        {
            get
            {
                return AppSettings.GetValueOrDefault<string>(UrlKey, UrlDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue<string>(UrlKey, value);
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
