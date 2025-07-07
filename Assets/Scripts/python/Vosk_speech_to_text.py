import queue
import sounddevice as sd
#print(sd.query_devices())
import sys
import json
import socket
from vosk import Model, KaldiRecognizer
import numpy as np
import os

# Path to your extracted model folder
script_dir = os.path.dirname(os.path.abspath(__file__))
assets_path = os.path.join(script_dir.split("Assets")[0], "Assets")
MODEL_PATH = os.path.join(assets_path, "Plugins", "Vosk", "Models", "vosk-model-small-en-us-0.15")

# More direct way to set the model path
# MODEL_PATH = r"C:\Users\antho\Ubiq\RealityFlow_Ubiq\Unity\Assets\Plugins\Vosk\Models\vosk-model-small-en-us-0.15"

#script_dir = os.path.dirname(os.path.abspath(__file__))
#assets_path = os.path.join(script_dir.split("Assets")[0], "Assets")
#MODEL_PATH = os.path.join(assets_path, "Plugins", "Vosk", "Models", "vosk-model-small-en-us-0.22")

#print("✅ Vosk Model Path:", MODEL_PATH)


# Sampling rate for microphone (usually 16000 Hz)
SAMPLE_RATE = 16000

# Load Vosk model
model = Model(MODEL_PATH)
recognizer = KaldiRecognizer(model, SAMPLE_RATE)
recognizer.SetWords(True)

# Setup socket
sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
sock.connect(("localhost", 65432))  # Port must match Unity listener

# Create a queue to hold audio data
q = queue.Queue()

# Callback for audio stream
def callback(indata, frames, time, status):
    if status:
        print(status, file=sys.stderr)

    # Convert to float32 and calculate RMS energy
    volume_norm = np.linalg.norm(indata) / len(indata)

    if volume_norm < 0.01:  # 🔇 tweak this value as needed
        return  # skip quiet input (likely mic hiss or background noise)

    q.put(bytes(indata))

# Start audio stream
with sd.RawInputStream(samplerate=SAMPLE_RATE, blocksize=8000, dtype='int16',
                       channels=1, callback=callback):
    print("🎤 Listening... Press Ctrl+C to stop.")
    try:
        while True:
            data = q.get()
            if recognizer.AcceptWaveform(data):
                result = json.loads(recognizer.Result())
                text = result.get("text", "")
                if text:
                    # Send recognized text to Unity
                    print("You said:", text)
                    sock.sendall((text + "\n").encode("utf-8"))
            else:
                # For partial results (while speaking)
                partial = json.loads(recognizer.PartialResult())
                text = partial.get("partial", "")
                if text:
                    sock.sendall((text + "\n").encode("utf-8"))
    except KeyboardInterrupt:
        print("\n🛑 Stopped by user")
    finally:
        sock.close()
        print("Socket closed. Exiting...")
        sd.stop()
        print("Audio stream stopped.")
