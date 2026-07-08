with open("src/ui/Controls/VideoPlayer/VideoPlayerControl.cs", "r") as f:
    c = f.read()

c = c.replace(r'ToolTip.SetTip(_btnVisualPos, "Visual Position Tool");', r'ToolTip.SetTip(_btnVisualPos, "Visual Position Tool");')
c = c.replace(r'Content = "\pos"', r'Content = "\\pos"')

with open("src/ui/Controls/VideoPlayer/VideoPlayerControl.cs", "w") as f:
    f.write(c)
