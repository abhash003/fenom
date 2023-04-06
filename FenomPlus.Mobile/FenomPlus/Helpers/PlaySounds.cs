using System;
using System.Diagnostics;
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
            try
            {
                if (green_high != null) return;
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
            catch(Exception e)
            {
                Debug.WriteLine(e);
            }
        }

        private static SoundsEnum CurrentSound = SoundsEnum.none;

        public static void PlaySound(float gaugeData)
        {
            // play none
            if ((gaugeData <= BreathGaugeValues.White1Top) || (gaugeData >= BreathGaugeValues.Red4Top))
            {
                StopAll();
                CurrentSound = SoundsEnum.none;
            }
            // play red_low
            else if (gaugeData <= BreathGaugeValues.Red2Top)
            {
                PlaySound(SoundsEnum.red_low, 100);
                if (CurrentSound != SoundsEnum.red_low)
                    StopSound();
                CurrentSound = SoundsEnum.red_low;
            }
            // play yellow_low
            else if (gaugeData <= BreathGaugeValues.Yellow1Top)
            {
                PlaySound(SoundsEnum.yellow_low, 100);
                if (CurrentSound != SoundsEnum.yellow_low)
                    StopSound();
                CurrentSound = SoundsEnum.yellow_low;
            }
            // play green_low
            else if (gaugeData <= BreathGaugeValues.Green1Top)
            {
                PlaySound(SoundsEnum.green_low, 100);
                if (CurrentSound != SoundsEnum.green_low)
                    StopSound();
                CurrentSound = SoundsEnum.green_low;
            }
            // play red_high
            else if (gaugeData >= BreathGaugeValues.Yellow2Top)
            {
                PlaySound(SoundsEnum.red_high, 100);
                if (CurrentSound != SoundsEnum.red_high)
                    StopSound();
                CurrentSound = SoundsEnum.red_high;
            }            
            // play yellow_high
            else if (gaugeData >= BreathGaugeValues.Green3Top)
            {
                PlaySound(SoundsEnum.yellow_high, 100);
                if (CurrentSound != SoundsEnum.yellow_high)
                    StopSound();
                CurrentSound = SoundsEnum.yellow_high;
            }
            // play green_high
            else if (gaugeData >= BreathGaugeValues.Green2Top)
            {
                PlaySound(SoundsEnum.green_high, 100);
                if (CurrentSound != SoundsEnum.green_high)
                    StopSound();
                CurrentSound = SoundsEnum.green_high;
            }
            // play green_mid
            else
            {
                PlaySound(SoundsEnum.green_mid, 100);
                if (CurrentSound != SoundsEnum.green_mid)
                    StopSound();
                CurrentSound = SoundsEnum.green_mid;
            }
        }

        private static void StopSound()
        {
            switch (CurrentSound)
            {
                case SoundsEnum.none:
                    break;
                case SoundsEnum.red_low:
                    Stop(red_low);
                    break;
                case SoundsEnum.red_high:
                    Stop(red_high);
                    break;
                case SoundsEnum.yellow_low:
                    Stop(yellow_low);
                    break;
                case SoundsEnum.yellow_high:
                    Stop(yellow_high);
                    break;
                case SoundsEnum.green_low:
                    Stop(green_low);
                    break;
                case SoundsEnum.green_mid:
                    Stop(green_mid);
                    break;
                case SoundsEnum.green_high:
                    Stop(green_high);
                    break;
                case SoundsEnum.test_failure:
                    break;
                case SoundsEnum.test_success:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
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
            switch (sound)
            {
                case SoundsEnum.red_low:
                    if (!red_low.IsPlaying)
                    {
                        StopAll();
                        red_low.Volume = volume;
                        red_low.Play();
                    }
                    break;
                case SoundsEnum.red_high:
                    if (!red_high.IsPlaying)
                    {
                        StopAll();
                        red_high.Volume = volume;
                        red_high.Play();
                    }
                    break;
                case SoundsEnum.yellow_low:
                    if (!yellow_low.IsPlaying)
                    {
                        StopAll();
                        yellow_low.Volume = volume;
                        yellow_low.Play();
                    }
                    break;
                case SoundsEnum.yellow_high:
                    if (!yellow_high.IsPlaying)
                    {
                        StopAll();
                        yellow_high.Volume = volume;
                        yellow_high.Play();
                    }
                    break;
                case SoundsEnum.green_low:
                    if (!green_low.IsPlaying)
                    {
                        StopAll();
                        green_low.Volume = volume;
                        green_low.Play();
                    }
                    break;
                case SoundsEnum.green_mid:
                    if (!green_mid.IsPlaying)
                    {
                        StopAll();
                        green_mid.Volume = volume;
                        green_mid.Play();
                    }
                    break;
                case SoundsEnum.green_high:
                    if (!green_high.IsPlaying)
                    {
                        StopAll();
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
        public static void Stop(ISimpleAudioPlayer isap, bool dispose = false)
        {
            if (isap != null)
            {
                if (isap.IsPlaying)
                    isap.Stop();

                if (dispose == true)
                {
                    isap.Dispose();
                    isap = null;
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public static void StopAll(bool dispose = false)
        {
            try
            {
                Stop(red_low, dispose);
                Stop(red_high, dispose);
                Stop(yellow_low, dispose);
                Stop(yellow_high, dispose);
                Stop(green_low, dispose);
                Stop(green_mid, dispose);
                Stop(green_high, dispose);
            }
            catch (Exception ex)
            {
                Console.Write(ex);
            }
        }

        public static void Close()
        {
            StopAll(true);
        }
    }
}
