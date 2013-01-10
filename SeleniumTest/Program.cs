using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Selenium;
using HtmlAgilityPack;
using System.Text.RegularExpressions;


namespace SeleniumTest
{
    public struct LinkItem
    {
        public string Href;
        public string Text;

        public override string ToString()
        {
            return Href + "\n\t" + Text;
        }
    }
    
    class Program
    {        

        static void Main(string[] args)
        {
            HtmlDocument doc = new HtmlDocument();
            List<string> list = new List<string>();
            HashSet<string> listposters = new HashSet<string>();
            int forumNumber = 0;
            string forumAddress = "";
            //List<string> list = new List<string>();
            ISelenium sel = new DefaultSelenium("localhost", 4444, "*chrome", forumAddress);
            sel.Start();
            sel.WindowMaximize();
            sel.Open("/");
            sel.Click("id=loginlink");
            sel.WaitForPageToLoad("30000");
            sel.Type("id=username", "");
            sel.Type("id=password", "");
            sel.Click("id=submit");
            sel.WaitForPageToLoad("30000");


            for (int y = 1; y < 14; y++)
            {
                sel.Open(string.Format("/vbulletin/forumdisplay.php?f=" + forumNumber + "&order=desc&page={0}", y));
                sel.WaitForPageToLoad("30000");

                var src = sel.GetHtmlSource();

                doc.LoadHtml(src);

                var links = doc.DocumentNode.SelectNodes("//a[@href]");

                var posters = from i in links
                              where i.OuterHtml.Contains("whoposted")
                              select i;

                var ln = from o in posters
                         select o.OuterHtml.ToString();

                foreach (var item in ln)
                {
                    LinkItem i = new LinkItem();


                    Match m2 = Regex.Match(item, @"href=\""(.*?)\""",
                    RegexOptions.Singleline);
                    if (m2.Success)
                    {
                        i.Href = m2.Groups[1].Value;
                    }

                    list.Add(i.Href.Replace("&amp;", "&"));
                }

            }

            foreach (var it in list)
            {
                sel.Open(string.Format("vbulletin/{0}", it));
                sel.WaitForPageToLoad("30000");

                var src = sel.GetHtmlSource();

                doc.LoadHtml(src);

                var links = doc.DocumentNode.SelectNodes("//a[@href]");

                var posters = from i in links
                              where i.OuterHtml.Contains("member")
                              select i;

                var ln = from o in posters
                         select o.InnerHtml.ToString();

                foreach (var item in ln)
                {
                    listposters.Add(item);
                }
            }


            sel.Close();



            //var links = doc.DocumentNode.SelectNodes("//a[@href]");
            //var links2 = doc.DocumentNode.G
            //var posters = from i in links
            //              where i.OuterHtml.Contains("lastposter")
            //              select i;

            //var link = from o in posters
            //           select o.OuterHtml;

            System.IO.File.WriteAllLines(@"C:\Users\William\Documents\CsharpRandom\SeleniumTest\AllPosters.txt", listposters);

            Console.Read();
        }
    }
}
