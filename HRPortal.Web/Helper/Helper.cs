using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace HRPortal.Web.Helper
{
    public class Helper
    {

    }
    public static class RandomGenerator
    {
        private static Random random = new Random();
        public static string GetCharNumber(int maxSize)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, maxSize)
      .Select(s => s[random.Next(s.Length)]).ToArray());

        }
        public static string GetChar(int maxSize)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            return new string(Enumerable.Repeat(chars, maxSize)
      .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public static string GetNumber(int maxSize)
        {
            const string chars = "1234567890";
            return new string(Enumerable.Repeat(chars, maxSize)
      .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }

    public class ReCaptcha
    {
        private readonly HttpClient captchaClient;

        public ReCaptcha(HttpClient captchaClient)
        {
            this.captchaClient = captchaClient;
        }

        public async Task<bool> IsValid(string captcha)
        {
            try
            {
                var postTask = await captchaClient
                    .PostAsync($"?secret=6Ld0PWsdAAAAAKTWfsZQLByjXPhndDlO2HUMjWLZ&response={captcha}", new StringContent(""));
                var result = await postTask.Content.ReadAsStringAsync();
                var resultObject = JObject.Parse(result);
                dynamic success = resultObject["success"];
                return (bool)success;
            }
            catch (Exception e)
            {
                // TODO: log this (in elmah.io maybe?)
                return false;
            }
        }
    }
}
