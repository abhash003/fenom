using System;
using FenomPlus.Controls;
using FenomPlus.Enums;
using Plugin.SimpleAudioPlayer;

namespace FenomPlus.Helpers
{
    public class PlaySounds
    {
        private static ISimpleAudioPlayer red_low;                  // red low
        private static ISimpleAudioPlayer red_high;                 // red high
        private static ISimpleAudioPlayer yellow_low;               // yellow low
        private static ISimpleAudioPlayer yellow_high;              // yellow high
        private static ISimpleAudioPlayer green_low;                // green low
        private static ISimpleAudioPlayer green_mid;                // green mid
        private static ISimpleAudioPlayer green_high;               // green high
        private static ISimpleAudioPlayer test_failure;
        private static ISimpleAudioPlayer test_success;

        /// <summary>
        /// 
        /// </summary>
        public static void InitSound()
        {
            if (green_high == null)
            {
                red_low = CrossSimpleAudioPlayer.CreateSimpleAudioPlayer();
                red_low.Load("red_low.wav");
                red_low.Loop = false;

                red_high = CrossSimpleAudioPlayer.CreateSimpleAudioPlayer();
                red_high.Load("red_high.wav");
                red_high.Loop = false;

                yellow_low = CrossSimpleAudioPlayer.CreateSimpleAudioPlayer();
                yellow_low.Load("yellow_low.wav");
                yellow_low.Loop = false;

                yellow_high = CrossSimpleAudioPlayer.CreateSimpleAudioPlayer();
                yellow_high.Load("yellow_high.wav");
                yellow_high.Loop = false;

                green_low = CrossSimpleAudioPlayer.CreateSimpleAudioPlayer();
                green_low.Load("green_low.wav");
                green_low.Loop = false;

                green_mid = CrossSimpleAudioPlayer.CreateSimpleAudioPlayer();
                green_mid.Load("green_mid.wav");
                green_mid.Loop = false;

                green_high = CrossSimpleAudioPlayer.CreateSimpleAudioPlayer();
                green_high.Load("green_high.wav");
                green_high.Loop = false;

                test_failure = CrossSimpleAudioPlayer.CreateSimpleAudioPlayer();
                test_failure.Load("test_failure.wav");
                test_failure.Loop = false;

                test_success = CrossSimpleAudioPlayer.CreateSimpleAudioPlayer();
                test_success.Load("test_success.wav");
                test_success.Loop = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static void PlaySound(float guageData)
        {
            InitSound();

            // play none
            if ((guageData <= BreathGuageValues.White1Top) || (guageData >= BreathGuageValues.Red4Top))
            {
                StopAll();
            }
            // play red_low
            else if (guageData <= BreathGuageValues.Red2Top)
            {
                PlaySound(SoundsEnum.red_low, 100);
            }
            // play yellow_low
            else if (guageData <= BreathGuageValues.Yellow1Top)
            {
                PlaySound(SoundsEnum.yellow_low, 100);
            }
            // play green_low
            else if (guageData <= BreathGuageValues.Green1Top)
            {
                PlaySound(SoundsEnum.green_low, 100);
            }
            // play red_high
            else if (guageData >= BreathGuageValues.Yellow2Top)
            {
                PlaySound(SoundsEnum.red_high, 100);
            }            
            // play yellow_high
            else if (guageData >= BreathGuageValues.Green3Top)
            {
                PlaySound(SoundsEnum.yellow_high, 100);
            }
            // play green_high
            else if (guageData >= BreathGuageValues.Green2Top)
            {
                PlaySound(SoundsEnum.green_high, 100);
            }
            // play green_mid
            else
            {
                PlaySound(SoundsEnum.green_mid, 100);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static void ResetToStart()
        {
            red_low.Seek(0);
            red_high.Seek(0);
            yellow_low.Seek(0);
            yellow_high.Seek(0);
            green_low.Seek(0);
            green_mid.Seek(0);
            green_high.Seek(0);
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
                case SoundsEnum.red_low:
                    if (!red_low.IsPlaying)
                    {
                        StopRedHigh();
                        StopYellowLow();
                        StopYellowHigh();
                        StopGreenLow();
                        StopGreenMid();
                        StopGreenHigh();
                        red_low.Volume = volume;
                        red_low.Play();
                    }
                    break;
                case SoundsEnum.red_high:
                    if (!red_high.IsPlaying)
                    {
                        StopRedLow();
                        StopYellowLow();
                        StopYellowHigh();
                        StopGreenLow();
                        StopGreenMid();
                        StopGreenHigh();
                        red_high.Volume = volume;
                        red_high.Play();
                    }
                    break;
                case SoundsEnum.yellow_low:
                    if (!yellow_low.IsPlaying)
                    {
                        StopRedLow();
                        StopRedHigh();
                        StopYellowHigh();
                        StopGreenLow();
                        StopGreenMid();
                        StopGreenHigh();
                        yellow_low.Volume = volume;
                        yellow_low.Play();
                    }
                    break;
                case SoundsEnum.yellow_high:
                    if (!yellow_high.IsPlaying)
                    {
                        StopRedLow();
                        StopRedHigh();
                        StopYellowLow();
                        StopGreenLow();
                        StopGreenMid();
                        StopGreenHigh();
                        yellow_high.Volume = volume;
                        yellow_high.Play();
                    }
                    break;
                case SoundsEnum.green_low:
                    if (!green_low.IsPlaying)
                    {
                        StopRedLow();
                        StopRedHigh();
                        StopYellowLow();
                        StopYellowHigh();
                        StopGreenMid();
                        StopGreenHigh();
                        green_low.Volume = volume;
                        green_low.Play();
                    }
                    break;
                case SoundsEnum.green_mid:
                    if (!green_mid.IsPlaying)
                    {
                        StopRedLow();
                        StopRedHigh();
                        StopYellowLow();
                        StopYellowHigh();
                        StopGreenLow();
                        StopGreenHigh();
                        green_mid.Volume = volume;
                        green_mid.Play();
                    }
                    break;
                case SoundsEnum.green_high:
                    if (!green_high.IsPlaying)
                    {
                        StopRedLow();
                        StopRedHigh();
                        StopYellowLow();
                        StopYellowHigh();
                        StopGreenLow();
                        StopGreenMid();
                        green_high.Volume = volume;
                        green_high.Play();
                    }
                    break;
                case SoundsEnum.test_failure:
                    StopAll();
                    test_failure.Volume = volume;
                    test_failure.Play();
                    break;
                case SoundsEnum.test_success:
                    StopAll();
                    test_success.Volume = volume;
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
                StopRedLow();
                StopRedHigh();
                StopYellowLow();
                StopYellowHigh();
                StopGreenLow();
                StopGreenMid();
                StopGreenHigh();
            }
            catch (Exception ex)
            {
                Console.Write(ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static void StopRedLow()
        {
            if ((red_low != null) && (red_low.IsPlaying))
            {
                red_low.Stop();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static void StopRedHigh()
        {
            if ((red_high != null) && (red_high.IsPlaying))
            {
                red_high.Stop();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static void StopYellowLow()
        {
            if ((yellow_low != null) && (yellow_low.IsPlaying))
            {
                yellow_low.Stop();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static void StopYellowHigh()
        {
            if ((yellow_high != null) && (yellow_high.IsPlaying))
            {
                yellow_high.Stop();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static void StopGreenLow()
        {
            if ((green_low != null) && (green_low.IsPlaying))
            {
                green_low.Stop();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static void StopGreenMid()
        {
            if ((green_mid != null) && (green_mid.IsPlaying))
            {
                green_mid.Stop();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public static void StopGreenHigh()
        {
            if ((green_high != null) && (green_high.IsPlaying))
            {
                green_high.Stop();
            }
        }

    }
}
