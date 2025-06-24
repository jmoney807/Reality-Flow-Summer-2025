from fastapi import FastAPI, File, UploadFile
import whisper
import uvicorn
import os

app = FastAPI()
model = whisper.load_model("base")  # use "tiny" if you want faster results

@app.post("/transcribe")
async def transcribe_audio(file: UploadFile = File(...)):
    file_location = f"temp/{file.filename}"
    os.makedirs("temp", exist_ok=True)
    with open(file_location, "wb") as f:
        f.write(await file.read())

    result = model.transcribe(file_location)
    os.remove(file_location)
    return {"text": result["text"]}

if __name__ == "__main__":
    uvicorn.run("whisper_api:app", host="127.0.0.1", port=8000)