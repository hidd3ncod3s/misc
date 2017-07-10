using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Web;
using mshtml;
using System.Runtime.InteropServices;
using System.Threading;

namespace QueryGoogle
{
    public static class GoogleQueryConf
    {
        /* DONT set anything here. */
        public static string OutputDir = @"C:\tmp\";
        public static AutoResetEvent autoResetEvent = new AutoResetEvent(false);

        /* Need to change for each and every domain and query */
        public static System.IO.StreamWriter urllist;
        public static string googleDomain = "";
        public static string query= "";
        public static int queryIndex = 0;

        public static List<string> googleDomains = new List<string>(new string[] {  "https://www.google.com", 
                                                                                    "https://www.google.no",                                                                        
                                                                                    "https://www.google.co.in", 
                                                                                    "https://www.google.co.uk", 
                                                                                    "https://www.google.co.za",
                                                                                    "https://www.google.ae", 
                                                                                    "https://www.google.com.pe",
                                                                                    "https://www.google.co.il",
                                                                                    "https://www.google.hn",
                                                                                    "https://www.google.gr",
                                                                                    "https://www.google.gl",
                                                                                    "https://www.google.com.au", 
                                                                                    "https://www.google.cl", 
                                                                                    "https://www.google.fr", 
                                                                                    "https://www.google.com.hk", 
                                                                                    "https://www.google.lk", 
                                                                                    "https://www.google.com.my",
                                                                                    "https://www.google.ca", 
                                                                                    "https://www.google.com.mx",
                                                                                    "https://www.google.nl",
                                                                                    "https://www.google.co.nz",
                                                                                    "https://www.google.com.ph",
                                                                                    "https://www.google.ru",
                                                                                    "https://www.google.com.sa",
                                                                                    "https://www.google.gg",
                                                                                    "https://www.google.fm",
                                                                                    "https://www.google.fi",
                                                                                    "https://www.google.cz",
                                                                                    "https://www.google.com.br"
                                                                                 });
        public static List<string> queryterms = new List<string>(new string[] { 
                                                                                    "Selsdon Group	 -site:wikipedia.org",
                                                                                    "Shalem Center	 -site:wikipedia.org",
                                                                                    "Shanghai Academy of Social Science (China)	 -site:wikipedia.org",
                                                                                    "Shanghai Finance Institute (China) 	 -site:wikipedia.org",
                                                                                    "Shangrila Talks (Singapore) 	 -site:wikipedia.org",
                                                                                    "Sobieski Institute	 -site:wikipedia.org",
                                                                                    "Society of Conservative Lawyers	 -site:wikipedia.org",
                                                                                    "South African Institute of Race Relations (South Africa) 	 -site:wikipedia.org",
                                                                                    "South Asian Association for Regional Cooperation (Nepal) 	 -site:wikipedia.org",
                                                                                    "South Asian Center for Reintegration and Independent Research	 -site:wikipedia.org",
                                                                                    "Strategic and Defence Studies Centre (SDSC)	 -site:wikipedia.org",
                                                                                    "Strategic Institute for Maritime Affairs	 -site:wikipedia.org",
                                                                                    "Taiwan Institute of Economic Research	 -site:wikipedia.org",
                                                                                    "Taiwan Institute of Economic Research (TIER) (Taiwan) 	 -site:wikipedia.org",
                                                                                    "Tax Foundation	 -site:wikipedia.org",
                                                                                    "Tax Foundation (United States) 	 -site:wikipedia.org",
                                                                                    "The Centre for Cross Border Studies	 -site:wikipedia.org",
                                                                                    "The Conference Board	 -site:wikipedia.org",
                                                                                    "The Hampton Institute	 -site:wikipedia.org",
                                                                                    "The Independent Institute	 -site:wikipedia.org",
                                                                                    "The National Institute for Research Advancement (NIRA) - Tokyo	 -site:wikipedia.org",
                                                                                    "The New Zealand Institute	 -site:wikipedia.org",
                                                                                    "The One Country Two Systems Research Institute (OCTS)	 -site:wikipedia.org",
                                                                                    "The Razumkov Centre	 -site:wikipedia.org",
                                                                                    "The Reform Institute	 -site:wikipedia.org",
                                                                                    "The Sejong Institute (Republic of Korea) 	 -site:wikipedia.org",
                                                                                    "The Senlis Council: International Council on Security and Development (Afghanistan) 	 -site:wikipedia.org",
                                                                                    "The Wilberforce Society	 -site:wikipedia.org",
                                                                                    "Theos	 -site:wikipedia.org",
                                                                                    "Third Way	 -site:wikipedia.org",
                                                                                    "Third Way (United States) 	 -site:wikipedia.org",
                                                                                    "Timbro	 -site:wikipedia.org",
                                                                                    "United Nations Development Program (United States) 	 -site:wikipedia.org",
                                                                                    "United Service Institution	 -site:wikipedia.org",
                                                                                    "Urban Institute																		 -site:wikipedia.org",
                                                                                    "Vision think tank	 -site:wikipedia.org",
                                                                                    "Vlaamse Volksbeweging	 -site:wikipedia.org",
                                                                                    "Walter Eucken Instit	 -site:wikipedia.org",
                                                                                    "Wiardi Beckman Foundation	 -site:wikipedia.org",
                                                                                    "Wiardi Beckman Foundation (The Netherlands) 	 -site:wikipedia.org",
                                                                                    "Wilson Park (United Kingdom) 	 -site:wikipedia.org",
                                                                                    "World Security Institute (WSI) (United States) 	 -site:wikipedia.org",
                                                                                    "Young Fabians	 -site:wikipedia.org",
                                                                              });
        public static List<string> finishedQueries= new List<string> ();

        public static int maxpagesCount = 1;
        public static int currentPageNumber = 0;
        public static string cururlloaded = "";
        public static System.IO.StreamWriter logFile = new System.IO.StreamWriter(@"C:\tmp\query.log");
        public static Random randomGenerator = new Random();
    }

    public partial class Form1 : Form
    {
        [DllImport("wininet.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern bool InternetSetCookie(string lpszUrl, string lpszCookieName, string lpszCookieData);

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            write2log("Form1_Load");
            Thread workerThread = new Thread(new ThreadStart(this.DoWork));
            workerThread.SetApartmentState(ApartmentState.STA);
            workerThread.Start();
        }

        private void DoWork()
        {
            int count = 0;
            int totalCount= GoogleQueryConf.queryterms.Count * GoogleQueryConf.googleDomains.Count;
            string Domain = GoogleQueryConf.googleDomains[0];
            string curQuery = "";
            GoogleQueryConf.urllist = new System.IO.StreamWriter(GoogleQueryConf.OutputDir + "UrlList" + curQuery + count + ".txt");

            // using while
            while (GoogleQueryConf.queryIndex < GoogleQueryConf.queryterms.Count)
            {
                curQuery = GoogleQueryConf.queryterms[GoogleQueryConf.queryIndex];
                int googleDomainIndex = GoogleQueryConf.randomGenerator.Next(0, GoogleQueryConf.googleDomains.Count - 1);
                Domain = GoogleQueryConf.googleDomains[googleDomainIndex];

                write2log(Domain);
                write2log(curQuery);

                GoogleQueryConf.query = HttpUtility.UrlEncode(curQuery);
                GoogleQueryConf.googleDomain = Domain;

                //GoogleQueryConf.autoResetEvent.Set();
                write2log("Navigating");
                DoNavigate(0);
                write2log("Finished Navigating...waiting");
                GoogleQueryConf.autoResetEvent.WaitOne();
                write2log("Released");

                GoogleQueryConf.query = "";
                GoogleQueryConf.googleDomain = "";
                GoogleQueryConf.currentPageNumber = 0;
                GoogleQueryConf.cururlloaded = "";

                System.Threading.Thread.Sleep(1000);
                curQuery = "";
                Domain = "";
                GoogleQueryConf.queryIndex++;
            }
            GoogleQueryConf.urllist.Close();
            GoogleQueryConf.urllist = null;
            Application.Exit();
        }

        private void write2log(string logstring)
        {
            GoogleQueryConf.logFile.WriteLine(logstring);
            GoogleQueryConf.logFile.Flush();
        }

        private void write2urlList(string logstring)
        {
            GoogleQueryConf.urllist.WriteLine(logstring);
            GoogleQueryConf.urllist.Flush();
        }

        private void DoNavigate(int pageNumber=-1, string url="")
        {
            if (GoogleQueryConf.currentPageNumber == GoogleQueryConf.maxpagesCount)
            {
                write2log("ERROR: crossed max number of pages.");
                return;
            }

            if (pageNumber == 0)
            {
                // First page: https://www.google.com/search?q=filetype%3Apdf+forms&oq=filetype%3Apdf+forms&aqs=chrome..asd34sdg." 344  "j0j1&sourceid=chrome&es_sm=93&ie=UTF-8"
                
                int randomNumber = GoogleQueryConf.randomGenerator.Next(100, 999);

                url = GoogleQueryConf.googleDomain + "/search?q=" + GoogleQueryConf.query +
                             "&oq=" + GoogleQueryConf.query + "&aqs=chrome..asdfassdf." +
                             randomNumber + "j0j1&sourceid=chrome&es_sm=93&ie=UTF-8&filter=0";

                //int count = webBrowser1.Document.Cookie.Length;
                //webBrowser1.Document.Cookie.Remove(0, count);

                bool success = InternetSetCookie(GoogleQueryConf.googleDomain, null, "PREF=ID=234343242:U=asdfasdfa343:FF=0:LD=en:NR=100:TM=342423423:LM=234234234:SG=2:S=asdfasd2343-; expires = Sat, 01-Jan-2015 00:00:00 GMT");
                if (success == false)
                {
                    MessageBox.Show("FAILED");
                    return;
                }
            }

            GoogleQueryConf.currentPageNumber++;
            webBrowser1.AllowNavigation = true;
            webBrowser1.Navigate(url);
            GoogleQueryConf.cururlloaded = url;
        }

        private void webBrowser1_DocumentCompleted(object sender,
            WebBrowserDocumentCompletedEventArgs e)
        {
            //write2log(GoogleQueryConf.cururlloaded);
            //write2log(e.Url.AbsoluteUri);
            write2log("before check webBrowser1_DocumentCompleted");
            write2log(GoogleQueryConf.cururlloaded);
            write2log(e.Url.AbsoluteUri);
            if (string.Compare(GoogleQueryConf.cururlloaded, e.Url.AbsoluteUri, StringComparison.OrdinalIgnoreCase) != 0) 
                return;

            write2log("webBrowser1_DocumentCompleted");

            this.Text = e.Url.ToString() + "loaded1";

            collecttheurl();

            DoneWiththeQuery();
            
        }

        private void DoneWiththeQuery()
        {
            write2log("DONE");
            //MessageBox.Show("DONE FETCHING");
            //Application.Exit();
            write2log("Releasing");
            GoogleQueryConf.autoResetEvent.Set();
        }

        private void collecttheurl()
        {
            bool foundTheURL = false;
            mshtml.IHTMLDocument2 htmlDoc = webBrowser1.Document.DomDocument as mshtml.IHTMLDocument2;

            List<mshtml.IHTMLDivElement> allDiv = htmlDoc.all.OfType<mshtml.IHTMLDivElement>().ToList();
            foreach (IHTMLElement div in allDiv)
            {
                //write2log(curElement.outerHTML);
                //write2log(curElement.tostring());
                //write2log(curElement.className);
                if (div.className == "rc")
                {
                    write2log("found a rc div");
                    IHTMLDOMNode divNode = (IHTMLDOMNode)div;
                    //write2log(div.innerHTML);
                    //write2log(div.className);

                    //var child = ((IHTMLDOMNode)divnode).firstChild;

                    if (!divNode.hasChildNodes())
                        continue;

                    IHTMLDOMChildrenCollection children = (IHTMLDOMChildrenCollection)divNode.childNodes;
                    foreach (IHTMLDOMNode child in children)
                    {
                        //write2log(child.GetType().Name);
                        if (child != null && child.GetType().Name == "HTMLHeaderElementClass")
                            if (child.hasChildNodes())
                            {
                                IHTMLAnchorElement ancharchild = (IHTMLAnchorElement)child.firstChild;
                                //write2log(ancharchild.GetType().Name);
                                if (ancharchild != null && ancharchild.GetType().Name == "HTMLAnchorElementClass")
                                {
                                    write2urlList(GoogleQueryConf.queryterms[GoogleQueryConf.queryIndex] + "\t => \t" + ancharchild.href);
                                    write2urlList(ancharchild.href);
                                    foundTheURL = true;
                                    break;
                                }
                            }
                        //write2log(child.ToString());
                    }
                }

                if (foundTheURL)
                    break;
            }
            return;
        }

        private string getnextpageurl()
        {
            mshtml.IHTMLDocument2 htmlDoc = webBrowser1.Document.DomDocument as mshtml.IHTMLDocument2;

            //navcnt
            var navcntdivelement = htmlDoc.body.all.item("pnnext");
            if (navcntdivelement == null)
            {
                write2log("ERROR: DIV(pnnext) is NULL");
                DoneWiththeQuery();
                return null;
            }

            IHTMLAnchorElement ancharchild = (IHTMLAnchorElement)navcntdivelement;
            //write2log(ancharchild.href);
            return ancharchild.href;
        }
    }
}
