import subprocess
import os
import sys

script_path = os.path.join(os.path.dirname(__file__), "Vosk_speech_to_text.py")
subprocess.Popen([sys.executable, script_path], creationflags=subprocess.CREATE_NO_WINDOW)
