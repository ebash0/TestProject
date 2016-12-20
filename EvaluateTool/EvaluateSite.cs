using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using TestProject.Models;

namespace TestProject.EvaluateTool
{
    public class EvaluateSite
    {
        public static List<URL> EvaluateURLs = new List<URL>();
        private EvaluateDBEntities db = new EvaluateDBEntities();

        public EvaluateSite(List<string> sitemapURLs, int evaluateId)
        {
            foreach(string url in sitemapURLs)
            {
                int[] times = new int[2];
                Stopwatch time = new Stopwatch();

                URL urlData = new URL();
                urlData.EvaluateId = evaluateId;
                urlData.Link = url;

                try
                {
                    for (int i = 0; i < times.Length; i++)
                    {
                        time.Reset();
                        time.Start();

                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlData.Link);
                        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                        response.Close();

                        time.Stop();
                        times[i] = time.Elapsed.Milliseconds;
                    }
                }
                catch (Exception ex)
                {
                    urlData.Error = ex.Message;
                }

                if (times[0] > times[1])
                {
                    urlData.Max = times[0];
                    urlData.Min = times[1];
                }
                else
                {
                    urlData.Max = times[1];
                    urlData.Min = times[0];
                }

                EvaluateURLs.Add(urlData);
            }

            Thread.Sleep(5000);

            List<URL> artist = null;

            lock (EvaluateURLs)
            {
                artist = EvaluateURLs.Where(x => x.EvaluateId == evaluateId).ToList();

                for (int i = 0; i < artist.Count; i++)
                {
                    EvaluateURLs.Remove(artist[i]);
                }
            }

            db.URLs.AddRange(artist);
            db.SaveChanges();

        }
    }
}