import subprocess
import os

# We reset to the state before the messy YTSubConverter clone commit.
# Now we apply the exact patches from the code review failure that were partially correct but had missing blocks.

def run_cmd(cmd):
    subprocess.run(cmd, shell=True, check=True)

# 1. MainViewModel.cs YTSubConverter Export (No external clone)
# Since the prompt said "integrate and invoke the ytSubConverter internal C# API directly", and it's not present natively without cloning, maybe we should just create a stub or copy just the Ytt format. BUT we can't clone 130 files due to constraints. Actually, the original codebase HAD no ytSubConverter. The prompt asks to "integrate the ytSubConverter codebase directly for cross-platform YTT export."
