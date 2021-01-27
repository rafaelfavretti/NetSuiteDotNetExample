using NetsuiteLibrary.com.netsuite.api.suitetalk.tstdrv2066000;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Security.Cryptography;

namespace NetsuiteLibrary
{
    public class ConnectionManager
    {
        private static TokenPassport passport;
        private static NetSuiteService service;

        public static NetSuiteService GetNetSuiteService()
        {
            if (passport == null)
            {
                passport = new TokenPassport();
                passport.account = ConfigurationManager.AppSettings["AccountID"];
                passport.consumerKey = ConfigurationManager.AppSettings["ConsumerKey"];
                passport.token = ConfigurationManager.AppSettings["TokenId"];
                passport.nonce = GenerateNonce(10);
                passport.timestamp = (Int64)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                passport.signature = GenerateSignature(passport.nonce, passport.timestamp);
            }

            if (service == null)
            {
                service = new NetSuiteService();
                service.tokenPassport = passport;
                service.Url = ConfigurationManager.AppSettings["URL"];
            }

            return service;
        }

        public static string GenerateNonce(int length)
        {
            RNGCryptoServiceProvider random = new RNGCryptoServiceProvider();
            var data = new byte[length];
            random.GetNonZeroBytes(data);
            return Convert.ToBase64String(data);
        }

        public static TokenPassportSignature GenerateSignature(string nonce, long timestamp)
        {
            string accountId = ConfigurationManager.AppSettings["AccountID"];
            string consumerKey = ConfigurationManager.AppSettings["ConsumerKey"];
            string tokenId = ConfigurationManager.AppSettings["TokenId"];
            string consumerSecret = ConfigurationManager.AppSettings["ConsumerSecret"];
            string tokenSecret = ConfigurationManager.AppSettings["TokenSecret"];

            //Creating Base String
            string baseString = accountId + "&" + consumerKey + "&" + tokenId + "&" + nonce + "&" + timestamp.ToString();

            //Creating Signing Key
            string key = consumerSecret + "&" + tokenSecret;

            string signature = "";

            var encoding = new System.Text.ASCIIEncoding();

            byte[] keyByte = encoding.GetBytes(key);

            byte[] messageBytes = encoding.GetBytes(baseString);

            //Signing the base string using the signing key using HMACSHA1 algorithm
            using (var myhmacsha1 = new HMACSHA1(keyByte))
            {
                byte[] hashmessage = myhmacsha1.ComputeHash(messageBytes);
                signature = Convert.ToBase64String(hashmessage);
            }

            TokenPassportSignature tokenPassportSignature = new TokenPassportSignature();
            tokenPassportSignature.Value = signature;
            tokenPassportSignature.algorithm = "HMAC-SHA1";

            return tokenPassportSignature;
        }
    }
}
