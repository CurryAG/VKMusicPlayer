using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace VKMusicPlayer
{
    public class Config
    {
        readonly string Filename;
        private string Xmlread(string filename, string tag)
        {
            while (true)
            {
                try
                {
                    XmlDocument xDoc = new XmlDocument();
                    xDoc.Load(filename);
                    XmlElement xRoot = xDoc.DocumentElement;
                    string needreturn = "";
                    foreach (XmlElement xnode in xRoot)
                    {
                        if (xnode.Name == tag)
                        {
                            needreturn = xnode.InnerText;
                            break;
                        }
                    }
                    return needreturn;
                    break;
                }
                catch
                {
                    Thread.Sleep(25);
                }
            }
        }
        private void Xmlwrite(string filename, string tag, string text)
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(filename);
            XmlElement xRoot = xDoc.DocumentElement;
            bool isfounded = false;
            foreach (XmlElement xnode in xRoot)
            {
                if (xnode.Name == tag)
                {
                    xnode.InnerText = text;
                    isfounded = true;
                    break;
                }
            }
            if (!isfounded)
            {
                XmlElement Elem = xDoc.CreateElement(tag);
                XmlText Text = xDoc.CreateTextNode(text);
                Elem.AppendChild(Text);
                xRoot.AppendChild(Elem);
            }
            xDoc.Save(filename);
        }
        private string XmlreadDoubleTag(string filename, string firsttag, string secondtag)
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(filename);
            XmlElement xRoot = xDoc.DocumentElement;
            string needreturn = "";
            foreach (XmlElement xnode in xRoot)
            {
                if (xnode.Name == firsttag)
                {
                    foreach (XmlElement item in xnode)
                    {
                        if (item.Name == secondtag)
                        {
                            needreturn = item.InnerText;
                        }
                    }
                }
            }
            return needreturn;
        }
        private void XmlwriteDoubleTag(string filename, string firsttag, string secondtag, string text)
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(filename);
            XmlElement xRoot = xDoc.DocumentElement;
            bool isfirstfounded = false;
            bool isdoublefounded = false;
            XmlElement temp = xRoot;
            foreach (XmlElement xnode in xRoot)
            {
                if (xnode.Name == firsttag)
                {
                    isfirstfounded = true;
                    temp = xnode;
                    foreach (XmlElement item in xnode)
                    {
                        if (item.Name == secondtag)
                        {
                            item.InnerText = text;
                            isdoublefounded = true;
                            break;
                        }
                    }
                }
            }
            if (!isfirstfounded && !isdoublefounded)
            {
                XmlElement FirstElem = xDoc.CreateElement(firsttag);
                XmlElement SecondElem = xDoc.CreateElement(secondtag);
                XmlText Text = xDoc.CreateTextNode(text);
                SecondElem.AppendChild(Text);
                FirstElem.AppendChild(SecondElem);
                xRoot.AppendChild(FirstElem);
            }
            if (isfirstfounded && !isdoublefounded)
            {
                XmlElement SecondElem = xDoc.CreateElement(secondtag);
                XmlText Text = xDoc.CreateTextNode(text);
                SecondElem.AppendChild(Text);
                temp.AppendChild(SecondElem);
            }
            xDoc.Save(filename);
        }
        public string VKLogin
        {
            get
            {
                if (XmlreadDoubleTag(Filename, "VK_auth", "Login") == "")
                {
                    VKLogin = "";
                }
                return XmlreadDoubleTag(Filename, "VK_auth", "Login");
            }
            set
            {
                XmlwriteDoubleTag(Filename, "VK_auth", "Login", value.ToString());
            }
        }
        public string VKPassword
        {
            get
            {
                if (XmlreadDoubleTag(Filename, "VK_auth", "Password") == "")
                {
                    VKPassword = "";
                }
                return XmlreadDoubleTag(Filename, "VK_auth", "Password");
            }
            set
            {
                XmlwriteDoubleTag(Filename, "VK_auth", "Password", value.ToString());
            }
        }
        public Config(string filename)
        {
            Filename = filename;
            if (!File.Exists(Filename))
            {
                XmlDocument xdoc = new XmlDocument();
                XmlElement Root = xdoc.CreateElement("config");
                xdoc.AppendChild(Root);
                xdoc.Save(filename);
            }
        }
    }
}
