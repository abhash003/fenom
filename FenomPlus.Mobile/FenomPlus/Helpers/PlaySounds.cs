using System;
using FenomPlus.Controls;
using Plugin.SimpleAudioPlayer;

namespace FenomPlus.Helpers
{
    public class PlaySounds
    {
        private static ISimpleAudioPlayer low_0_60sec;              // red low
        private static ISimpleAudioPlayer mid_low_60sec;            // yellow low
        private static ISimpleAudioPlayer mid_mid_60sec;            // green
        private static ISimpleAudioPlayer mid_high_60sec;           // yellow high
        private static ISimpleAudioPlayer high_0_60sec;             // red high
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
            if ((guageData <= BreathGuageValues.White1Top) || (guageData >= BreathGuageValues.White2))
            {
                StopAll();
            }
            // play low red low
            else if (guageData <= BreathGuageValues.Red2Top)
            {
                PlaySound(SoundsEnum.low_0_60sec, 100);
            }
            // play high red high
            else if (guageData >= BreathGuageValues.Red3)
            {
                PlaySound(SoundsEnum.high_0_60sec, 100);
            }
            // play low yellow mid_low
            else if (guageData <= BreathGuageValues.Yellow1Top)
            {
                PlaySound(SoundsEnum.mid_low_60sec, 100);
            }
            // play high yellow mid_high
            else if (guageData >= BreathGuageValues.Yellow2)
            {
                PlaySound(SoundsEnum.mid_high_60sec, 100);
            }
            // play green mid_mid
            else
            {
                if (!mid_mid_60sec.IsPlaying)
                {
                    PlaySound(SoundsEnum.mid_mid_60sec, 100);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static void ResetToStart()
        {
            low_0_60sec.Seek(0);
            mid_low_60sec.Seek(0);
            mid_mid_60sec.Seek(0);
            mid_high_60sec.Seek(0);
            high_0_60sec.Seek(0);
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
                case SoundsEnum.low_0_60sec:
                    if (!low_0_60sec.IsPlaying) { 
                        StopMidLow();
                        StopMidMid();
                        StopMidHigh();
                        StopHigh();
                        low_0_60sec.Volume = volume;
                        low_0_60sec.Play();
                    }
                    break;
                case SoundsEnum.mid_low_60sec:
                    if (!mid_low_60sec.IsPlaying)
                    {
                        StopLow();
                        StopMidMid();
                        StopMidHigh();
                        StopHigh();
                        mid_low_60sec.Volume = volume;
                        mid_low_60sec.Play();
                    }
                    break;
                case SoundsEnum.mid_mid_60sec:
                    if (!mid_mid_60sec.IsPlaying)
                    {
                        StopLow();
                        StopMidLow();
                        StopMidHigh();
                        StopHigh();
                        mid_mid_60sec.Volume = volume;
                        mid_mid_60sec.Play();
                    }
                    break;
                case SoundsEnum.mid_high_60sec:
                    if (!mid_high_60sec.IsPlaying)
                    {
                        StopLow();
                        StopMidLow();
                        StopMidMid();
                        StopHigh();
                        mid_high_60sec.Volume = volume;
                        mid_high_60sec.Play();
                    }
                    break;
                case SoundsEnum.high_0_60sec:
                    if (!high_0_60sec.IsPlaying)
                    {
                        StopLow();
                        StopMidLow();
                        StopMidMid();
                        StopMidHigh();
                        high_0_60sec.Volume = volume;
                        high_0_60sec.Play();
                    }
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
                StopLow();
                StopMidLow();
                StopMidMid();
                StopMidHigh();
                StopHigh();
            }
            catch (Exception ex)
            {
                Console.Write(ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static void StopLow()
        {
            if (low_0_60sec != null)
            {
                low_0_60sec.Stop();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static void StopMidLow()
        {
            if (mid_low_60sec != null)
            {
                mid_low_60sec.Stop();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static void StopMidMid()
        {
            if (mid_mid_60sec != null)
            {
                mid_mid_60sec.Stop();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static void StopMidHigh()
        {
            if (mid_high_60sec != null)
            {
                mid_high_60sec.Stop();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static void StopHigh()
        {
            if (high_0_60sec != null)
            {
                high_0_60sec.Stop();
            }
        }
    }
}
