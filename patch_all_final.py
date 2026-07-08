import os
import re

def write_srv3():
    with open("src/libse/SubtitleFormats/Srv3.cs", "w") as f:
        f.write("""using Nikse.SubtitleEdit.Core.Common;
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
""")

def patch_main():
    with open("src/ui/Features/Main/MainViewModel.cs", "r") as f:
        c = f.read()

    # Just remove ytSubConverter calls natively inside threads
    c = re.sub(
        r"var ytsubconverterPath = System\.IO\.Path\.Combine\(AppDomain\.CurrentDomain\.BaseDirectory, \"ytsubconverter\.exe\"\);\s*if \(System\.IO\.File\.Exists\(ytsubconverterPath\)\)\s*\{\s*var processInfo = new System\.Diagnostics\.ProcessStartInfo\s*\{\s*FileName = ytsubconverterPath,\s*Arguments = \$\"\\\"\{_subtitleFileName\}\\\"\",\s*UseShellExecute = false,\s*CreateNoWindow = true\s*\};\s*System\.Diagnostics\.Process\.Start\(processInfo\);\s*\}",
        """var yttFormat = new YouTubeYtt();
                    var tempYttPath = System.IO.Path.ChangeExtension(_subtitleFileName, ".ytt");
                    var yttContent = yttFormat.ToText(GetUpdateSubtitle(), System.IO.Path.GetFileNameWithoutExtension(_subtitleFileName));
                    System.IO.File.WriteAllText(tempYttPath, yttContent);""",
        c
    )
    with open("src/ui/Features/Main/MainViewModel.cs", "w") as f:
        f.write(c)

def patch_lua():
    with open("src/Automation/LuaEngine.cs", "r") as f:
        c = f.read()
    c = c.replace(
"""        public void ExecuteScript(string script)
        {
            // Execute script
        }""",
"""        private NLua.Lua _lua;

        public void RegisterSubtitles(Nikse.SubtitleEdit.Core.Subtitle subtitle)
        {
            if (_lua == null) { _lua = new NLua.Lua(); _lua.LoadCLRPackage(); }
            _lua["subtitle"] = subtitle;
            _lua.DoString(@"
                aegisub = aegisub or {}
                function aegisub.log(lvl, msg) print(msg) end
                function aegisub.register_macro(name, desc, proc) _G[name] = proc end
            ");
        }

        public void ExecuteScript(string script)
        {
            if (_lua == null) { _lua = new NLua.Lua(); _lua.LoadCLRPackage(); }
            try { _lua.DoString(script); }
            catch (Exception ex) { Console.WriteLine($"Lua execution error: {ex.Message}"); throw; }
        }"""
    )
    with open("src/Automation/LuaEngine.cs", "w") as f:
        f.write(c)

def patch_proj():
    with open("src/ui/UI.csproj", "r") as f:
        c = f.read()
    if "NLua" not in c:
        c = c.replace("""</Project>""", """  <ItemGroup>
    <PackageReference Include="NLua" Version="1.7.9" />
  </ItemGroup>
</Project>""")
    with open("src/ui/UI.csproj", "w") as f:
        f.write(c)

def patch_karaoke():
    with open("src/ui/Features/Assa/AssaApplyAdvancedEffect/Effects/AdvancedEffectKaraoke.cs", "r") as f:
        c = f.read()

    # Fix the whole file natively
    c = """using Nikse.SubtitleEdit.Core.Common;
using Nikse.SubtitleEdit.Features.Main;
using Nikse.SubtitleEdit.Logic.Config;
using Nikse.SubtitleEdit.Logic.Media;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nikse.SubtitleEdit.Features.Assa.AssaApplyAdvancedEffect.Effects;

public class AdvancedEffectKaraoke : IAdvancedEffectDisplay
{
    public string Name => Se.Language.Assa.AdvancedEffectKaraoke;
    public string Description => Se.Language.Assa.AdvancedEffectKaraokeDescription;
    public bool UsesAudio => true;

    public override string ToString() => Name;

    public List<SubtitleLineViewModel> ApplyEffect(string header, List<SubtitleLineViewModel> subtitles, int width, int height, WavePeakData2? wavePeaks)
    {
        var result = new List<SubtitleLineViewModel>();
        foreach (var subtitle in subtitles)
        {
            var cleanText = Utilities.RemoveSsaTags(subtitle.Text);
            if (string.IsNullOrEmpty(cleanText))
            {
                result.Add(subtitle);
                continue;
            }

            var words = cleanText.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var wordCount = words.Length;
            if (wordCount == 0)
            {
                result.Add(subtitle);
                continue;
            }

            var totalMs = subtitle.Duration.TotalMilliseconds;
            var msPerWord = totalMs / wordCount;

            var sb = new StringBuilder();
            for (var i = 0; i < wordCount; i++)
            {
                int durationCentiseconds = (int)Math.Round(msPerWord / 10.0);
                sb.Append("{\\\\k" + durationCentiseconds + "}" + words[i] + " ");
            }

            var line = new SubtitleLineViewModel(subtitle, generateNewId: true);
            line.Text = sb.ToString().TrimEnd();
            result.Add(line);
        }
        return result;
    }
}
"""
    with open("src/ui/Features/Assa/AssaApplyAdvancedEffect/Effects/AdvancedEffectKaraoke.cs", "w") as f:
        f.write(c)

write_srv3()
patch_main()
patch_lua()
patch_proj()
patch_karaoke()
