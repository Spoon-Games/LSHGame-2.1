using Melanchall.DryWetMidi.Core;
using System.IO;
using UnityEngine;

namespace MuseL
{
    public class MidiAsset : ScriptableObject
    {
        [SerializeField]
        private byte[] midiFileBytes;

        private MidiFile midiFile;

        public MidiFile MidiFile
        {
            get
            {
                if(midiFile == null)
                {
                    GetMidiFile();
                }
                return midiFile;
            }
            set
            {
                SetMidiFile(value);
            }
        }

        private void GetMidiFile()
        {
            if (midiFileBytes == null || midiFileBytes.Length == 0)
            {
                midiFile = new MidiFile();
                return;
            }

            MemoryStream memoryStream = new MemoryStream(midiFileBytes);

            midiFile = MidiFile.Read(memoryStream);
        }

        private void SetMidiFile(MidiFile midiFile)
        {
            MemoryStream memoryStream = new MemoryStream();
            midiFile.Write(memoryStream);

            midiFileBytes = memoryStream.ToArray();
            this.midiFile = midiFile;
        }

        public void SetMidiFileRaw(byte[] file)
        {
            midiFileBytes = file;
        }
    } 
}
