using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpeechLib;
using System.Threading;
using System.Speech.Synthesis;
using System.Speech.AudioFormat;
using System.Speech.Recognition;

namespace ConsoleTest
{
    partial class Program
    {
        static void TtsTest()
        {
            SpVoice tVoice = new SpVoice();
            tVoice.Rate = -5;
            tVoice.Volume = 100;
            var lstVoice = tVoice.GetVoices();
            tVoice.Voice = lstVoice.Item(0);
            tVoice.Speak("测试微软tts语音输出");
        }

        static void Tts2WavFile()
        {
            SpFileStream tStream = new SpFileStream();
            tStream.Open(@"e:\ttstest.wav", SpeechStreamFileMode.SSFMCreateForWrite);
            SpVoice tVoice = new SpVoice();
            tVoice.AudioOutputStream = tStream;
            tVoice.Speak("开始安装前，先设置计算机的启动顺序为CD-ROM（使用光盘安装）");
            tVoice.WaitUntilDone(Timeout.Infinite);
            tStream.Close();
        }

        static void Speak2WavFile()
        {
            using (var speaker = new SpeechSynthesizer())
            {
                // 8000Hz 电话所用采样率，对于人的说话已经足够;
                //11025Hz 获得的声音称为电话音质，基本上能让你分辨出通话人的声音
                //22050Hz 无线电广播所用采样率，广播音质
                //32000Hz miniDV数码视频camcorder、DAT(LPmode)所用采样率
                //44100Hz 音频CD，也常用于MPEG-1音频（VCD，SVCD，MP3）所用采样率
                var spFmt = new SpeechAudioFormatInfo(16000, AudioBitsPerSample.Eight, AudioChannel.Mono);

                speaker.Rate = -2;  // -10~10
                speaker.SetOutputToWaveFile(@"e:\speaker.wav", spFmt);
                var nVol = speaker.Volume;
                var sVoice = speaker.Voice;
                speaker.Speak("呼叫1、 2、 5、 9、 4、 7");
                speaker.Speak("电话所用采样率，对于人的说话已经足够");
                //speaker.Pause();
            }
        }

        static void Wav2Text()
        {
            var lstRec = SpeechRecognitionEngine.InstalledRecognizers();
            var speech = new SpeechRecognitionEngine();
            //speech.LoadGrammar(new DictationGrammar());
            GrammarBuilder gb = new GrammarBuilder();
            gb.Append(new Choices("呼叫", "拨打", " "));
            gb.Append(new Choices("0", "1", "2", "3", "4", "5", "6", "7", "8", "9", " "));
            gb.Append(new Choices("0", "1", "2", "3", "4", "5", "6", "7", "8", "9", " "));
            gb.Append(new Choices("0", "1", "2", "3", "4", "5", "6", "7", "8", "9", " "));
            gb.Append(new Choices("0", "1", "2", "3", "4", "5", "6", "7", "8", "9", " "));
            gb.Append(new Choices("0", "1", "2", "3", "4", "5", "6", "7", "8", "9", " "));
            gb.Append(new Choices("0", "1", "2", "3", "4", "5", "6", "7", "8", "9", " "));
            gb.Append(new Choices("0", "1", "2", "3", "4", "5", "6", "7", "8", "9", " "));
            gb.Append(new Choices("0", "1", "2", "3", "4", "5", "6", "7", "8", "9", " "));
            gb.Append(new Choices("0", "1", "2", "3", "4", "5", "6", "7", "8", "9", " "));
            gb.Append(new Choices("0", "1", "2", "3", "4", "5", "6", "7", "8", "9", " "));
            gb.Append(new Choices("0", "1", "2", "3", "4", "5", "6", "7", "8", "9", " "));
            gb.Append(new Choices("0", "1", "2", "3", "4", "5", "6", "7", "8", "9", " "));
            //gb.Append(new Choices("0"));
            //gb.Append(new Choices("1"));
            //gb.Append(new Choices("2"));
            //gb.Append(new Choices("3"));
            //gb.Append(new Choices("4"));
            //gb.Append(new Choices("5"));
            //gb.Append(new Choices("6"));
            //gb.Append(new Choices("7"));
            //gb.Append(new Choices("8"));
            //gb.Append(new Choices("9"));
            speech.LoadGrammar(new Grammar(gb));
            speech.SetInputToWaveFile(@"e:\speaker.wav");
            var recRes = speech.Recognize();
            if(recRes == null)
            {
                Console.WriteLine("Recognize fail");
                return;
            }
            Console.WriteLine(recRes.Text);
        }
    }
}
