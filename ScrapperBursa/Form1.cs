using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
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
            if (File.Exists("output.csv")) File.Delete("output.csv");
            if (File.Exists("contacte.csv")) File.Delete("contacte.csv");
            webBrowser1.Navigate("https://www.bursatransport.com/login");
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
            for (int i = 41; i <= 92; i++)
            {
                // doar tipuri. cate 2 fisiere pt fiecare tip
                //clienti transport
                //https://www.bursatransport.com/memberlist?doSearch=1&privateexchange_id=&MemberSearch2[text]=&MemberSearch2[description]=&MemberSearch2[activity]=Client+transport&MemberSearch2[activeTransportLicence]=0&MemberSearch2[activeForwardingLicence]=0&MemberSearch2[activeCompanies]=0&source_id_txt=&searchType=1&yt0=&range=0&page=
                //carausi
                //https://www.bursatransport.com/memberlist?doSearch=1&privateexchange_id=&MemberSearch2%5Btext%5D=&MemberSearch2%5Bdescription%5D=&MemberSearch2%5Bactivity%5D=Caraus&MemberSearch2%5BactiveTransportLicence%5D=0&MemberSearch2%5BactiveForwardingLicence%5D=0&MemberSearch2%5BactiveCompanies%5D=0&source_id_txt=&searchType=1&yt0=&range=0&page=
                webBrowser1.Navigate("https://www.bursatransport.com/memberlist?doSearch=1&privateexchange_id=&MemberSearch2[text]=&MemberSearch2[description]=&MemberSearch2[activity]=Client+transport&MemberSearch2[activeTransportLicence]=0&MemberSearch2[activeForwardingLicence]=0&MemberSearch2[activeCompanies]=0&source_id_txt=&searchType=1&yt0=&range=0&page=" + i.ToString());
                webBrowser1.ScriptErrorsSuppressed = true;
                while (webBrowser1.ReadyState != WebBrowserReadyState.Complete)
                {
                    Thread.Sleep(5);
                    Application.DoEvents();
                }

                ParsePage();
            }
        }

        private void ParsePage()
        {
            var linkEl = webBrowser1.Document.GetElementsByTagName("a");
            string compId = string.Empty;
            string compName = string.Empty;
            string url = string.Empty;
            foreach (HtmlElement a in linkEl)
            {
                if (a.GetAttribute("className").StartsWith("companyProfileUrl"))
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
                    string registruComert = string.Empty;
                    string anulInfiintarii = string.Empty;
                    string sediulSocial = string.Empty;
                    string administrator = string.Empty;
                    string dataInscrierii = string.Empty;

                    string parcAuto = string.Empty;

                    string birouAdresa = string.Empty;
                    string birouTel = string.Empty;
                    string birouMobil = string.Empty;
                    string birouEmail = string.Empty;

                    foreach (HtmlElement d in dataEl)
                    {
                        if (d.GetAttribute("className").Equals("companyLabel"))
                        {
                            switch (d.InnerText)
                            {
                                case "C.I.F.:":
                                    cif = d.NextSibling.InnerText;
                                    break;
                                case "Nr. Registru Comert:":
                                    registruComert = d.NextSibling.InnerText;
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
                                case "Parc auto:":
                                    var parcAutoList = d.NextSibling.GetElementsByTagName("li");
                                    foreach (HtmlElement pa in parcAutoList)
                                    {
                                        parcAuto += pa.InnerText + "|";
                                    }
                                    if (parcAuto.Length > 0) parcAuto = parcAuto.Remove(parcAuto.Length - 1, 1);
                                    break;
                                default: break;
                            }
                        }

                        if (d.GetAttribute("className").Equals("siteLabel"))
                        {
                            switch (d.InnerText)
                            {
                                case "Adresa locatie:":
                                    birouAdresa = d.NextSibling.InnerText;
                                    break;
                                case "Telefoane locatie:":
                                    var birouTelList = d.NextSibling.GetElementsByTagName("span");
                                    foreach (HtmlElement tl in birouTelList)
                                    {
                                        if (tl.GetAttribute("className").Equals("phone"))
                                            birouTel += tl.InnerHtml + "|";
                                        if (tl.GetAttribute("className").Equals("mobile"))
                                            birouMobil += tl.InnerHtml + "|";
                                    }
                                    if (birouTel.Length > 0) birouTel = birouTel.Remove(birouTel.Length - 1, 1);
                                    if (birouMobil.Length > 0) birouMobil = birouMobil.Remove(birouMobil.Length - 1, 1);
                                    break;
                                case "Email locatie:":
                                    birouEmail = d.NextSibling.InnerText;
                                    break;
                                default: break;
                            }
                        }
                    }

                    System.IO.StreamWriter file = new System.IO.StreamWriter("output.csv", true); //append!
                    file.WriteLine("" + compId + "" + ";" + "" + compName + "" + ";" + "" + cif + "" + ";" + registruComert + "" + ";" + anulInfiintarii + "" + ";" +
                        sediulSocial.Replace("\r\n", ",") + "" + ";" + administrator + "" + ";" + dataInscrierii + "" + ";" + parcAuto + ""
                        + ";" + birouAdresa.Replace("\r\n", ",") + "" + ";" + birouTel + "" + ";" + birouMobil + "" + ";" + birouEmail + "");

                    //compId;compName; cif; registruComert; anulInfiintarii; sediulSocial; administrator; dataInscrierii; parcAuto; birouAdresa; birouTel; birouMobil; birouEmail
                    file.Flush();
                    file.Close();
                    Application.DoEvents();

                    #region persoane contact
                    string contactNume = string.Empty;
                    string contactPozitia = string.Empty;
                    string contactTel = string.Empty;
                    string contactMobil = string.Empty;
                    string contactEmail = string.Empty;
                    string contactSkype = string.Empty;
                    string contactYahoo = string.Empty;

                    var contactDivs = webBrowser2.Document.GetElementsByTagName("div");
                    foreach (HtmlElement divCont in contactDivs)
                    {
                        if (divCont.GetAttribute("className").Equals("contact"))
                        {
                            contactNume = string.Empty;
                            contactPozitia = string.Empty;
                            contactTel = string.Empty;
                            contactMobil = string.Empty;
                            contactEmail = string.Empty;
                            contactSkype = string.Empty;
                            contactYahoo = string.Empty;

                            contactNume = divCont.Children[0].InnerText;

                            var ddd = divCont.Children[1].Children[0].Children[0].GetElementsByTagName("td");
                            foreach (HtmlElement ctd in ddd)
                            {
                                //if (div.GetAttribute("className").Equals("contactDescription"))
                                //{
                                //foreach (HtmlElement ctd in div.Children)
                                {
                                    if (ctd.GetAttribute("className").Equals("contactLabel"))
                                    {
                                        switch (ctd.InnerText)
                                        {
                                            case "Pozitia:":
                                                contactPozitia = ctd.NextSibling.InnerText;
                                                break;
                                            case "Telefoane personale:":
                                                var contactTelList = ctd.NextSibling.GetElementsByTagName("span");
                                                foreach (HtmlElement tl in contactTelList)
                                                {
                                                    if (tl.GetAttribute("className").Equals("phone"))
                                                        contactTel += tl.InnerHtml + "|";
                                                    if (tl.GetAttribute("className").Equals("mobile"))
                                                        contactMobil += tl.InnerHtml + "|";
                                                }
                                                if (contactTel.Length > 0) contactTel = contactTel.Remove(contactTel.Length - 1, 1);
                                                if (contactMobil.Length > 0) contactMobil = contactMobil.Remove(contactMobil.Length - 1, 1);
                                                break;
                                            case "Email personal:":
                                                contactEmail = ctd.NextSibling.InnerText;
                                                break;
                                            case "ID Skype:":
                                                contactSkype = ctd.NextSibling.InnerText;
                                                break;
                                            case "ID Yahoo Messenger:":
                                                contactYahoo = ctd.NextSibling.InnerText;
                                                break;
                                            default:
                                                break;
                                        }
                                    }
                                }
                                //}
                            }
                            System.IO.StreamWriter fileContact = new System.IO.StreamWriter("contacte.csv", true); //append!
                            fileContact.WriteLine("" + compId + "" + ";" + "" + contactNume + "" + ";" + "" + contactPozitia + "" + ";" + contactTel + "" + ";" +
                                contactMobil + "" + ";" + contactEmail + "" + ";" + contactSkype + "" + ";" + contactYahoo + "");
                            //compId; contactNume; contactPozitia; contactTel; contactMobil; contactEmail; contactSkype; contactYahoo
                            fileContact.Flush();
                            fileContact.Close();
                            Application.DoEvents();
                        }
                    }
                    #endregion
                }
            }
        }
    }
}
