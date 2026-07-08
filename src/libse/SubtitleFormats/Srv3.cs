using Nikse.SubtitleEdit.Core.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Nikse.SubtitleEdit.Core.SubtitleFormats
{
    public class Srv3 : SubtitleFormat
    {
        public override string Extension => ".srv3";
        public override string Name => "SRV3";

        public override string ToText(Subtitle subtitle, string title)
        {
            var xml = new XmlDocument();
            xml.AppendChild(xml.CreateXmlDeclaration("1.0", "utf-8", null));
            var root = xml.CreateElement("timedtext");
            root.SetAttribute("format", "3");
            xml.AppendChild(root);

            var body = xml.CreateElement("body");
            root.AppendChild(body);

            foreach (var p in subtitle.Paragraphs)
            {
                var pNode = xml.CreateElement("p");
                pNode.SetAttribute("t", p.StartTime.TotalMilliseconds.ToString());
                pNode.SetAttribute("d", p.Duration.TotalMilliseconds.ToString());
                pNode.InnerText = p.Text;
                body.AppendChild(pNode);
            }

            var sb = new StringBuilder();
            using (var writer = XmlWriter.Create(sb, new XmlWriterSettings { Indent = true }))
            {
                xml.Save(writer);
            }
            return sb.ToString();
        }

        public override void LoadSubtitle(Subtitle subtitle, List<string> lines, string fileName)
        {
            _errorCount = 0;
            var sb = new StringBuilder();
            foreach (var line in lines) sb.AppendLine(line);
            var xml = new XmlDocument();
            try
            {
                xml.LoadXml(sb.ToString());
                var pNodes = xml.SelectNodes("//p");
                if (pNodes != null)
                {
                    foreach (XmlNode pNode in pNodes)
                    {
                        var t = pNode.Attributes["t"]?.Value;
                        var d = pNode.Attributes["d"]?.Value;
                        if (t != null && d != null)
                        {
                            var p = new Paragraph(new TimeCode(double.Parse(t)), new TimeCode(double.Parse(t) + double.Parse(d)), pNode.InnerText);
                            subtitle.Paragraphs.Add(p);
                        }
                    }
                }
            }
            catch { _errorCount++; }
        }
    }
}
