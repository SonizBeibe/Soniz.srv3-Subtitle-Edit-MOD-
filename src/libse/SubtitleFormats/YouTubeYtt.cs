using Nikse.SubtitleEdit.Core.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Nikse.SubtitleEdit.Core.SubtitleFormats
{
    public class YouTubeYtt : SubtitleFormat
    {
        public override string Extension => ".ytt";

        public override string Name => "YouTube ytt";

        public override string ToText(Subtitle subtitle, string title)
        {
            var xml = new XmlDocument();
            xml.AppendChild(xml.CreateXmlDeclaration("1.0", "utf-8", null));
            var root = xml.CreateElement("timedtext");
            root.SetAttribute("format", "3");
            xml.AppendChild(root);

            var head = xml.CreateElement("head");
            root.AppendChild(head);

            var pen = xml.CreateElement("pen");
            pen.SetAttribute("id", "1");
            pen.SetAttribute("fc", "#FEFEFE");
            head.AppendChild(pen);

            var ws = xml.CreateElement("ws");
            ws.SetAttribute("id", "1");
            ws.SetAttribute("ju", "2");
            ws.SetAttribute("pd", "0");
            head.AppendChild(ws);

            var wp = xml.CreateElement("wp");
            wp.SetAttribute("id", "1");
            wp.SetAttribute("ap", "7");
            wp.SetAttribute("ah", "50");
            wp.SetAttribute("av", "100");
            head.AppendChild(wp);

            var body = xml.CreateElement("body");
            root.AppendChild(body);

            foreach (var p in subtitle.Paragraphs)
            {
                var pNode = xml.CreateElement("p");
                pNode.SetAttribute("t", p.StartTime.TotalMilliseconds.ToString());
                pNode.SetAttribute("d", p.Duration.TotalMilliseconds.ToString());
                pNode.SetAttribute("wp", "1");
                pNode.SetAttribute("ws", "1");

                var sNode = xml.CreateElement("s");
                sNode.SetAttribute("p", "1");
                sNode.InnerText = p.Text;
                pNode.AppendChild(sNode);

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
            // Placeholder: Not required for this exact request, but good to have
            _errorCount = 0;
            var sb = new StringBuilder();
            foreach (var line in lines)
            {
                sb.AppendLine(line);
            }
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
                        var text = pNode.InnerText;

                        if (t != null && d != null)
                        {
                            var p = new Paragraph(new TimeCode(double.Parse(t)), new TimeCode(double.Parse(t) + double.Parse(d)), text);
                            subtitle.Paragraphs.Add(p);
                        }
                    }
                }
            }
            catch
            {
                _errorCount++;
            }
        }
    }
}
