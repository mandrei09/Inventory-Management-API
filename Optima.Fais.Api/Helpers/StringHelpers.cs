using Microsoft.AspNetCore.Builder;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Text.RegularExpressions;
using WinSCP;

namespace Optima.Fais.Api
{
    public static class StringHelpers
    {
        //public static List<int?> ToIntArray(this string text)
        //{
        //    if (text.Length > 0)
        //        return JsonConvert.DeserializeObject<string[]>(text).ToList().Select(i => (int?)int.Parse(i)).ToList();
        //    return null;
        //}

        //public static List<int?> TryToIntArray(this string text)
        //{
        //    if ((text != null) && (text.Length > 0))
        //        return JsonConvert.DeserializeObject<string[]>(text).ToList().Select(i => (int?)int.Parse(i)).ToList();
        //    return null;
        //}

        public static List<int?> ToIntArray(this string text)
        {
            if (text.Length > 0)
                return JsonConvert.DeserializeObject<string[]>(text).ToList().Select(i => (int?)int.Parse(i)).ToList();
            return null;
        }

        public static List<int?> TryToIntArray(this string text)
        {
            if ((text != null) && (text.Length > 0))
                return JsonConvert.DeserializeObject<string[]>(text).ToList().Select(i => (int?)int.Parse(i)).ToList();
            return null;
        }

        public static List<string> TryToStringtArray(this string text)
        {
            if ((text != null) && (text.Length > 0))
                return JsonConvert.DeserializeObject<string[]>(text).ToList().Select(i => (i)).ToList();
            return null;
        }

        public static AlternateView ContentToAlternateView(string content)
        {
            var imgCount = 0;
            List<LinkedResource> resourceCollection = new List<LinkedResource>();
            foreach (Match m in Regex.Matches(content, "<img(?<value>.*?)>"))
            {
                imgCount++;
                var imgContent = m.Groups["value"].Value;
                string type = Regex.Match(imgContent, ":(?<type>.*?);base64,").Groups["type"].Value;
                string base64 = Regex.Match(imgContent, "base64,(?<base64>.*?)\"").Groups["base64"].Value;
                if (String.IsNullOrEmpty(type) || String.IsNullOrEmpty(base64))
                {
                    //ignore replacement when match normal <img> tag
                    continue;
                }
                var replacement = " src=\"cid:" + imgCount + "\"";
                content = content.Replace(imgContent, replacement);
                var tempResource = new LinkedResource(Base64ToImageStream(base64), new ContentType(type))
                {
                    ContentId = imgCount.ToString()
                };
                resourceCollection.Add(tempResource);
            }

            AlternateView alternateView = AlternateView.CreateAlternateViewFromString(content, null, MediaTypeNames.Text.Html);
            foreach (var item in resourceCollection)
            {
                alternateView.LinkedResources.Add(item);
            }

            return alternateView;
        }

        public static Stream Base64ToImageStream(string base64String)
        {
            byte[] imageBytes = Convert.FromBase64String(base64String);
            MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
            return ms;
        }

        public static bool BetweenInclusive(this DateTime value, DateTime a, DateTime b)
        {
            return ((a <= value) && (value <= b)) || ((b <= value) && (value <= a));
        }

		public static void SFTP_Connect_And_Download_Sample()
		{

			WinSCP.SessionOptions sessionOptions = new WinSCP.SessionOptions
			{
				Protocol = Protocol.Ftp,
				HostName = "13.80.159.243",
				UserName = "optima",
				Password = "aYqS@68@%W",
				FtpSecure = FtpSecure.Implicit,
				TlsHostCertificateFingerprint = "e2:dc:a3:b6:74:8e:b1:10:c1:5c:59:e3:9a:94:00:90:09:c0:48:10",
			};

			using (Session session = new Session())
			{
				session.Open(sessionOptions);

				var path = Path.Combine("download", DateTime.UtcNow.ToString("yyyyMMdd"));

				if (!Directory.Exists(path))
				{
					Directory.CreateDirectory(path);
				}

				var files = session.GetFilesToDirectory("", Directory.GetCurrentDirectory() + @"\" + path);
			}
		}

		//public static IEnumerable<T> ForEach<T>(this IEnumerable<T> source, Action<T> act)
		//{
		//    foreach (T element in source) act(element);
		//    return source;
		//}




	}
}
