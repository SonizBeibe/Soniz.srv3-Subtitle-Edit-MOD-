using System;
using System.Collections.Generic;

namespace Nikse.SubtitleEdit.Automation
{
    public class LuaEngine
    {
        public LuaEngine()
        {
            // Lua engine initialization
        }

        private NLua.Lua _lua;

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
            catch (System.Exception ex) { System.Console.WriteLine($"Lua execution error: {ex.Message}"); throw; }
        }
    }
}
