using System;
using Plugin.SimpleAudioPlayer;

namespace FenomPlus.Helpers
{
    public class PlaySounds
    {
        private static ISimpleAudioPlayer high_0_60sec;
        private static ISimpleAudioPlayer low_0_60sec;
        private static ISimpleAudioPlayer mid_high_60sec;
        private static ISimpleAudioPlayer mid_mid_60sec;
        private static ISimpleAudioPlayer mid_low_60sec;
        private static ISimpleAudioPlayer test_failure;
        private static ISimpleAudioPlayer test_success;

        /// <summary>
        /// 
        /// </summary>
        public static void InitSound()
        {
            if (high_0_60sec == null)
            {
                high_0_60sec = CrossSimpleAudioPlayer.CreateSimpleAudioPlayer();
                high_0_60sec.Load("high_0_60sec.wav");
                low_0_60sec = CrossSimpleAudioPlayer.CreateSimpleAudioPlayer();
                low_0_60sec.Load("low_0_60sec.wav");
                mid_high_60sec = CrossSimpleAudioPlayer.CreateSimpleAudioPlayer();
                mid_high_60sec.Load("mid_high_60sec.wav");
                mid_low_60sec = CrossSimpleAudioPlayer.CreateSimpleAudioPlayer();
                mid_low_60sec.Load("mid_low_60sec.wav");
                mid_mid_60sec = CrossSimpleAudioPlayer.CreateSimpleAudioPlayer();
                mid_mid_60sec.Load("mid_mid_60sec.wav");
                test_failure = CrossSimpleAudioPlayer.CreateSimpleAudioPlayer();
                test_failure.Load("test_failure.wav");
                test_success = CrossSimpleAudioPlayer.CreateSimpleAudioPlayer();
                test_success.Load("test_success.wav");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static void PlaySound(float guageData)
        {
            InitSound();

            // play none
            if ((guageData < 1) || (guageData > 5))
            {
                StopAll();
            }
            // play low
            else if (guageData < 2.8)
            {
                if (!low_0_60sec.IsPlaying)
                {
                    StopMid();
                    StopHigh();
                    low_0_60sec.Loop = true;
                    low_0_60sec.Volume = 100;
                    low_0_60sec.Play();
                }
            }
            // play high
            else if (guageData > 3.2)
            {
                if (!high_0_60sec.IsPlaying)
                {
                    StopMid();
                    StopLow();
                    high_0_60sec.Volume = 100;
                    high_0_60sec.Loop = true;
                    high_0_60sec.Play();
                }
            }
            // play mid
            else
            {
                if (!mid_low_60sec.IsPlaying)
                {
                    StopHigh();
                    StopLow();
                    mid_low_60sec.Loop = true;
                    mid_low_60sec.Volume = 100;
                    mid_low_60sec.Play();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static void ResetToStart()
        {
            high_0_60sec.Seek(0);
            low_0_60sec.Seek(0);
            mid_high_60sec.Seek(0);
            mid_low_60sec.Seek(0);
            mid_mid_60sec.Seek(0);
            test_failure.Seek(0);
            test_success.Seek(0);
        }

        /// <summary>
        /// play succes sound
        /// </summary>
        public static void PlaySuccessSound()
        {
            PlaySound(SoundsEnum.test_success, 100);
        }

        // play failed sound
        public static void PlayFailedSound()
        {
            PlaySound(SoundsEnum.test_failure, 100);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sound"></param>
        public static void PlaySound(SoundsEnum sound, double volume)
        {
            InitSound();
            switch (sound)
            {
                case SoundsEnum.high_0_10sec:
                    StopMid();
                    StopLow();
                    high_0_60sec.Volume = volume;
                    high_0_60sec.Play();
                    break;
                case SoundsEnum.low_0_10sec:
                    StopMid();
                    StopHigh();
                    low_0_60sec.Volume = volume;
                    low_0_60sec.Play();
                    break;
                case SoundsEnum.mid_high_05sec:
                    StopHigh();
                    StopLow();
                    mid_high_60sec.Volume = volume;
                    mid_high_60sec.Play();
                    break;
                case SoundsEnum.mid_low_05sec:
                    StopHigh();
                    StopLow();
                    mid_low_60sec.Volume = volume;
                    mid_low_60sec.Play();
                    break;
                case SoundsEnum.mid_mid_05sec:
                    StopHigh();
                    StopLow();
                    mid_mid_60sec.Volume = volume;
                    mid_mid_60sec.Play();
                    break;
                case SoundsEnum.test_failure:
                    StopAll();
                    test_failure.Volume = volume;
                    test_failure.Loop = false;
                    test_failure.Play();
                    break;
                case SoundsEnum.test_success:
                    StopAll();
                    test_success.Volume = volume;
                    test_success.Loop = false;
                    test_success.Play();
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static void StopAll()
        {
            try
            {
                if (high_0_60sec != null)
                {
                    high_0_60sec.Stop();
                    low_0_60sec.Stop();
                    mid_high_60sec.Stop();
                    mid_low_60sec.Stop();
                    mid_mid_60sec.Stop();
                }
            }
            catch(Exception ex)
            {
                Console.Write(ex);
            }
        }

        ///
        public static void StopLow()
        {
            if(low_0_60sec != null)
                low_0_60sec.Stop();
        }

        /// <summary>
        /// 
        /// </summary>
        public static void StopHigh()
        {
            if(high_0_60sec != null)
                high_0_60sec.Stop();
        }

        /// <summary>
        /// 
        /// </summary>
        public static void StopMid()
        {
            if (mid_high_60sec != null)
            {
                mid_high_60sec.Stop();
                mid_low_60sec.Stop();
                mid_mid_60sec.Stop();
            }
        }
    }
}
