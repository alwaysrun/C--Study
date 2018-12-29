using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Speech.Synthesis;
using System.Speech.AudioFormat;

namespace FrmTools
{
    public partial class FrmTTS : Form
    {
        public FrmTTS()
        {
            InitializeComponent();
        }

        private void FrmTTS_Load(object sender, EventArgs e)
        {
        }

        private void btnFile_Click(object sender, EventArgs e)
        {
            if(dlgSaveWav.ShowDialog() == DialogResult.OK)
            {
                this.txtFile.Text = dlgSaveWav.FileName;
            }
        }

        private void btnConvert_Click(object sender, EventArgs e)
        {
            if(string.IsNullOrEmpty(this.txtTTS.Text))
            {
                MessageBox.Show("Input the text first");
                return;
            }
            if(string.IsNullOrEmpty(this.txtFile.Text))
            {
                MessageBox.Show("Set wavfile fist");
                return;
            }

            try
            {
                Txt2WavFile(this.txtTTS.Text, this.txtFile.Text);
                MessageBox.Show("Convert OK");
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        void Txt2WavFile(string strTxt_, string strWavFile_)
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
                speaker.SetOutputToWaveFile(strWavFile_, spFmt);
                speaker.Speak(strTxt_);
            }
        }
    }
}
