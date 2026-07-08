import re

with open("src/ui/Features/Assa/AssaApplyAdvancedEffect/Effects/AdvancedEffectKaraoke.cs", "r") as f:
    k_code = f.read()

k_code = k_code.replace('sb.Append($"{{\\k{durationCentiseconds}}}{words[i]} ");', 'sb.Append($"{{\\\\k{durationCentiseconds}}}{words[i]} ");')

with open("src/ui/Features/Assa/AssaApplyAdvancedEffect/Effects/AdvancedEffectKaraoke.cs", "w") as f:
    f.write(k_code)

with open("src/ui/Features/Main/MainViewModel.cs", "r") as f:
    m_code = f.read()

m_code = m_code.replace(
"""                    var ytsubconverterPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ytsubconverter.exe");
                    if (System.IO.File.Exists(ytsubconverterPath))
                    {
                        var processInfo = new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = ytsubconverterPath,
                            Arguments = $"\"{_subtitleFileName}\"",
                            UseShellExecute = false,
                            CreateNoWindow = true
                        };
                        System.Diagnostics.Process.Start(processInfo);
                    }""",
"""                    // Embedded YtSubConverter export logic natively
                    var yttFormat = new YouTubeYtt();
                    var tempYttPath = System.IO.Path.ChangeExtension(_subtitleFileName, ".ytt");
                    var yttContent = yttFormat.ToText(GetUpdateSubtitle(), System.IO.Path.GetFileNameWithoutExtension(_subtitleFileName));
                    System.IO.File.WriteAllText(tempYttPath, yttContent);"""
)

with open("src/ui/Features/Main/MainViewModel.cs", "w") as f:
    f.write(m_code)

with open("src/Automation/LuaEngine.cs", "r") as f:
    lua_code = f.read()

lua_code = lua_code.replace(
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
    f.write(lua_code)
