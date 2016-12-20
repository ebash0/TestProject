using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Xml;

namespace TestProject.EvaluateTool
{
    public class SitemapHelper
    {
        internal static string GetSitemap(string host)
        {
            string sitemap = null;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(host + "/sitemap.xml");
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                sitemap = new StreamReader(response.GetResponseStream())
                    .ReadToEnd();
            }

            return sitemap;
        }

        internal static List<string> GetSitemapURLs(string host)
        {
            List<string> urls = new List<string>();

            XmlDocument rssXmlDoc = new XmlDocument();

            // Load the Sitemap file from the Sitemap URL
            rssXmlDoc.Load(host + "/sitemap.xml");

            StringBuilder sitemapContent = new StringBuilder();

            // Iterate through the top level nodes and find the "urlset" node. 
            foreach (XmlNode topNode in rssXmlDoc.ChildNodes)
            {
                if (topNode.Name.ToLower() == "urlset")
                {
                    // Use the Namespace Manager, so that we can fetch nodes using the namespace
                    XmlNamespaceManager nsmgr = new XmlNamespaceManager(rssXmlDoc.NameTable);
                    nsmgr.AddNamespace("ns", topNode.NamespaceURI);

                    // Get all URL nodes and iterate through it.
                    XmlNodeList urlNodes = topNode.ChildNodes;
                    foreach (XmlNode urlNode in urlNodes)
                    {
                        // Get the "loc" node and retrieve the inner text.
                        XmlNode locNode = urlNode.SelectSingleNode("ns:loc", nsmgr);
                        string link = locNode != null ? locNode.InnerText : "";
                        urls.Add(link);
                    }
                }
            }

            return urls;
        }
        
    }
}