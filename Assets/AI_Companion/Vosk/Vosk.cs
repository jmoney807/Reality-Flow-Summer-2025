using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Vosk
{
    public class Vosk
    {
        [DllImport("libvosk", CallingConvention = CallingConvention.Cdecl)]
        public static extern void vosk_set_log_level(int log_level);

        public static void SetLogLevel(int logLevel) => vosk_set_log_level(logLevel);
    }

    public class Model : IDisposable
    {
        private IntPtr _handle;

        [DllImport("libvosk", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr vosk_model_new(string path);

        [DllImport("libvosk", CallingConvention = CallingConvention.Cdecl)]
        private static extern void vosk_model_free(IntPtr model);

        public Model(string modelPath)
        {
            _handle = vosk_model_new(modelPath);
        }

        public IntPtr Handle => _handle;

        public void Dispose()
        {
            if (_handle != IntPtr.Zero)
            {
                vosk_model_free(_handle);
                _handle = IntPtr.Zero;
            }
        }
    }

    public class VoskRecognizer : IDisposable
    {
        private IntPtr _handle;

        [DllImport("libvosk", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr vosk_recognizer_new(IntPtr model, float sampleRate);

        [DllImport("libvosk", CallingConvention = CallingConvention.Cdecl)]
        private static extern int vosk_recognizer_accept_waveform(IntPtr recognizer, byte[] data, int len);

        [DllImport("libvosk", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr vosk_recognizer_result(IntPtr recognizer);

        [DllImport("libvosk", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr vosk_recognizer_partial_result(IntPtr recognizer);

        [DllImport("libvosk", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr vosk_recognizer_final_result(IntPtr recognizer);

        [DllImport("libvosk", CallingConvention = CallingConvention.Cdecl)]
        private static extern void vosk_recognizer_free(IntPtr recognizer);

        public VoskRecognizer(Model model, float sampleRate)
        {
            _handle = vosk_recognizer_new(model.Handle, sampleRate);
        }

        public bool AcceptWaveform(byte[] data, int len) =>
            vosk_recognizer_accept_waveform(_handle, data, len) != 0;

        public string Result() => PtrToStringUTF8(vosk_recognizer_result(_handle));

        public string PartialResult() => PtrToStringUTF8(vosk_recognizer_partial_result(_handle));

        public string FinalResult() => PtrToStringUTF8(vosk_recognizer_final_result(_handle));

        public void Dispose()
        {
            if (_handle != IntPtr.Zero)
            {
                vosk_recognizer_free(_handle);
                _handle = IntPtr.Zero;
            }
        }

        private static string PtrToStringUTF8(IntPtr ptr)
        {
            if (ptr == IntPtr.Zero)
                return null;

            int len = 0;
            while (Marshal.ReadByte(ptr, len) != 0)
                ++len;

            byte[] buffer = new byte[len];
            Marshal.Copy(ptr, buffer, 0, len);

            return Encoding.UTF8.GetString(buffer);
        }
    }
}
