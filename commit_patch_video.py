with open("src/ui/Controls/VideoPlayer/VideoPlayerControl.cs", "r") as f:
    v_code = f.read()

v_code = v_code.replace(r'Content = "\pos"', r'Content = "\\pos"')
v_code = v_code.replace(r'"\pos"', r'"\\pos"')

with open("src/ui/Controls/VideoPlayer/VideoPlayerControl.cs", "w") as f:
    f.write(v_code)
