using System;
using System.Collections.Generic;
using System.Media;

namespace Gemstones
{
    public enum SoundName { 
        Click, GemBreak
    }

    public class SoundManager
    {
        private List<SoundPlayer> musics;
        //private List<SoundPlayer> sounds;

        public SoundManager() {
            musics = new List<SoundPlayer>();
            //sounds = new List<SoundPlayer>();

            musics.Add(LoadSoundFromFile("./Sounds/MainTheme.wav"));
            musics.Add(LoadSoundFromFile("./Sounds/Gameplay.wav"));

            //sounds.Add(LoadSoundFromFile("./Sounds/GemBreak.wav"));
            //sounds.Add(LoadSoundFromFile("./Sounds/Mouse1.wav"));
        }

        public SoundPlayer LoadSoundFromFile(string filename) {
            SoundPlayer sp = null;
            try
            {
                sp = new SoundPlayer();
                sp.SoundLocation = filename;
                sp.LoadAsync();
            }
            catch (Exception ex) {
                System.Windows.Forms.MessageBox.Show(ex.Message, "Sound loader");
            }

            return sp;
        }
        /*
        public void PlaySound(SoundName name) { 
            switch(name) {
                case SoundName.GemBreak:
                    sounds[0].Play();
                    break;
                case SoundName.Click:
                    sounds[1].Play();
                    break;
            }
        }
        */
        public void PlayMainTheme() {
            for (int i = 0; i < musics.Count; i++)
                musics[i].Stop();
            musics[0].PlayLooping();
        }

        public void PlayGameplay() {
            for (int i = 0; i < musics.Count; i++)
                musics[i].Stop();
            musics[1].PlayLooping();
        }

        public void StopAll() {
            for (int i = 0; i < musics.Count; i++)
                musics[i].Stop();
        }
    }
}
