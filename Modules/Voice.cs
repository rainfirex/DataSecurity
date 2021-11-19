using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Speech.Synthesis;
using System.Collections.ObjectModel;
using System.Threading;

namespace DataSecurity.Modules
{
    class Voice
    {
        private SpeechSynthesizer synth;
        private ReadOnlyCollection<InstalledVoice> _voiceList;
        private bool _isSpeak;

        public ReadOnlyCollection<InstalledVoice> VoiceList
        {
            get { return _voiceList; }
        }

        public Voice(bool isSpeak = false)
        {
            synth = new SpeechSynthesizer();
            synth.Rate = 1;
            synth.Volume = 100;
            _voiceList = synth.GetInstalledVoices();
            _isSpeak = isSpeak;

            // Configure the audio output.   
            synth.SetOutputToDefaultAudioDevice();
        }

        public void setVoice(InstalledVoice voice)
        {   
            synth.SelectVoice(voice.VoiceInfo.Name);
            synth.SelectVoiceByHints(VoiceGender.Male);
        }

        public void SpeakAsync(string text)
        {
            if(_isSpeak)
            {
                Thread thread = new Thread(new ParameterizedThreadStart(ThreadSpeak));
                thread.IsBackground = true;
                thread.Start(text);
            }
        }

        private void ThreadSpeak(object text)
        {
            Speak(text.ToString());
        }

        public void Speak(string text)
        {
            if (_isSpeak)
            {
                synth.Speak(text);
            }
        }
    }
}
