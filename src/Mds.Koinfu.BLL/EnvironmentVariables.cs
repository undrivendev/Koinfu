using System;
using System.Collections.Generic;
using System.Text;

namespace Mds.Koinfu.BLL
{
    public static class AppEnvironmentVariables
    {
        private static string ENVVAR_PREFIX = "KOINFU_";
        public static class Api
        {
            private static string ENVVAR_API_SUFFIX = "APIAUTH_";
            public static class OpenExchangeRates
            {
                private static string ENVVAR_OPENEXCHANGERATES_SUFFIX = "OPENEXCHANGERATES_";
                public static string KEY = ENVVAR_PREFIX + ENVVAR_API_SUFFIX + ENVVAR_OPENEXCHANGERATES_SUFFIX + "KEY";
            }
            public static class CoinbasePro
            {
                private static string ENVVAR_COINBASEPRO_SUFFIX = "COINBASEPRO_";

                public static string KEY = ENVVAR_PREFIX + ENVVAR_API_SUFFIX + ENVVAR_COINBASEPRO_SUFFIX + "KEY";
                public static string PASSPHRASE = ENVVAR_PREFIX + ENVVAR_API_SUFFIX + ENVVAR_COINBASEPRO_SUFFIX + "PASSPHRASE";
                public static string SECRET = ENVVAR_PREFIX + ENVVAR_API_SUFFIX + ENVVAR_COINBASEPRO_SUFFIX + "SECRET";
            }
        }
        public static class Mail
        {
            private static string ENVVAR_MAIL_SUFFIX = "MAIL_";
            public static string SERVER = ENVVAR_PREFIX + ENVVAR_MAIL_SUFFIX + "SERVER";
            public static string PORT = ENVVAR_PREFIX + ENVVAR_MAIL_SUFFIX + "PORT";
            public static string USERNAME = ENVVAR_PREFIX + ENVVAR_MAIL_SUFFIX + "USERNAME";
            public static string PASSWORD = ENVVAR_PREFIX + ENVVAR_MAIL_SUFFIX + "PASSWORD";
        }
    }
}
