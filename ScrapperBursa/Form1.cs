using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScrapperBursa
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //webBrowser1.Navigate("https://www.google.ro/");            
            webBrowser1.Navigate("https://www.bursatransport.com/login");
            //label1.Text = webBrowser1.Version.Major.ToString();
            //webBrowser1.Document.GetElementById("Login_username").InnerHtml = "sesconsulting";
            //webBrowser1.Document.GetElementById("Login_password").InnerHtml = "romtas";

            //webBrowser1.Document.GetElementsByTagName("yt1").InvokeMember("click");
            //Login_username
            //Login_password
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var inputEl = webBrowser1.Document.GetElementsByTagName("input");
            foreach (HtmlElement i in inputEl)
            {
                if (i.GetAttribute("name").Equals("Login[username]"))
                {
                    i.InnerText = "sesconsulting";
                }
                if (i.GetAttribute("name").Equals("Login[password]"))
                {
                    i.Focus();
                    i.InnerText = "romtas";
                }
            }

            var buttonEl = webBrowser1.Document.GetElementsByTagName("button");
            foreach (HtmlElement b in buttonEl)
            {
                if (b.GetAttribute("name").Equals("yt1"))
                    b.InvokeMember("click");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            webBrowser1.Navigate("https://www.bursatransport.com/memberlist?doSearch=1&privateexchange_id=&MemberSearch2%5Btext%5D=&MemberSearch2%5Bdescription%5D=&MemberSearch2%5Bactivity%5D=Caraus&MemberSearch2%5BactiveTransportLicence%5D=0&MemberSearch2%5BactiveForwardingLicence%5D=0&MemberSearch2%5BactiveCompanies%5D=0&source_id_txt=&searchType=1&yt0=&range=0&page=1");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var linkEl = webBrowser1.Document.GetElementsByTagName("a");
            string compId = string.Empty;
            string compName = string.Empty;
            string url = string.Empty;
            foreach (HtmlElement a in linkEl)
            {
                if (a.GetAttribute("className").Equals("companyProfileUrl"))
                {
                    compName = a.InnerText;
                    compId = a.Parent.Parent.Children[0].InnerText;
                    Console.WriteLine(compId);
                    Console.WriteLine(a.InnerText);
                    //https://www.bursatransport.com/account/default/profile/company_id/215453/reftype/300
                    url = "https://www.bursatransport.com/account/default/profile/company_id/" + compId + "/reftype/300";
                    webBrowser2.Navigate(url);
                    webBrowser2.ScriptErrorsSuppressed = true;
                    //Thread.Sleep(10);
                    //Application.DoEvents();
                    while (webBrowser2.ReadyState != WebBrowserReadyState.Complete)
                    {
                        //RTBAction("Current Action: Loading " + Category + " | " + Subcategory + " | " + ItemLink + " (" + Browsersw.Elapsed.Seconds.ToString() + " sec)");
                        //UpdatelabelElapsed("Elapsed Time: " + sw.Elapsed.Days + " day(s), " + sw.Elapsed.Hours + " hours, " + sw.Elapsed.Minutes + " min");
                        Thread.Sleep(5);
                        Application.DoEvents();
                    }

                    var dataEl = webBrowser2.Document.GetElementsByTagName("td");
                    string cif = string.Empty;
                    string anulInfiintarii = string.Empty;
                    string sediulSocial = string.Empty;
                    string administrator = string.Empty;
                    string dataInscrierii = string.Empty;

                    foreach (HtmlElement d in dataEl)
                    {
                        if (d.GetAttribute("className").Equals("companyLabel"))
                        {
                            switch (d.InnerText)
                            {
                                case "C.I.F.:":
                                    cif = d.NextSibling.InnerText;
                                    break;
                                case "Anul infiintarii:":
                                    anulInfiintarii = d.NextSibling.InnerText;
                                    break;
                                case "Sediu social:":
                                    sediulSocial = d.NextSibling.InnerText;
                                    break;
                                case "Administrator:":
                                    administrator = d.NextSibling.InnerText;
                                    break;
                                case "Inscris in BursaTransport:":
                                    dataInscrierii = d.NextSibling.InnerText;
                                    break;
                                default: break;
                            }
                        }
                    }

                    System.IO.StreamWriter file = new System.IO.StreamWriter("output.csv", true); //append!
                    file.WriteLine("" + compId + "" + ";" + "" + compName + "" + ";" + "" + cif + "" + ";" + anulInfiintarii + "" + ";" +
                        sediulSocial + "" + ";" + administrator + "" + ";" + dataInscrierii + "");
                    file.Flush();
                    file.Close();
                    Application.DoEvents();
                }
            }
        }
    }
}
