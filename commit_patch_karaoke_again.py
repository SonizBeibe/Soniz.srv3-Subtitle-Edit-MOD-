with open("src/ui/Features/Assa/AssaApplyAdvancedEffect/Effects/AdvancedEffectKaraoke.cs", "r") as f:
    k_code = f.read()

# CS1009 fix inside Karaoke
k_code = k_code.replace("${\\k", "{{\\\\k")

with open("src/ui/Features/Assa/AssaApplyAdvancedEffect/Effects/AdvancedEffectKaraoke.cs", "w") as f:
    f.write(k_code)
