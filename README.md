Reality Flow is a Unity-based VR experience that lets you create and manipulate 3D content via natural speech. It includes two main modules:

Object Forge – (3D Generation) Turns validated user intents into 3D object creation/actions.

Voice Validator – Filters/validates speech so only relevant creation commands reach Object Forge.

You can run it VR or desktop, and you can swap in different STT (speech-to-text) and LLM backends.

# 1) Install prerequisites (below) incl. Git LFS
# 2) Clone the repo with LFS
 git lfs install
 git clone https://github.com/<your-org>/<repo>.git
 cd <repo>

# 3) (Optional) Pull models via helper script
 scripts/setup_models.ps1   # Windows PowerShell
 # or
 bash scripts/setup_models.sh

# 4) Open in Unity 6 (exact version below) and open the Demo scene:
#    Assets/Scenes/Demo_Room.unity

# 5) Press Play (Desktop) or run with your Quest via Link/Air Link (VR)
