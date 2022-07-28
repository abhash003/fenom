using System;
using Plugin.SimpleAudioPlayer;

namespace FenomPlus.Helpers
{
    // ------------------------------------------------------------------------------------------------------------------------
    // Breath Flow(lpm)     On the Gauge(degree)    Colour on the UI        Comments	
    // 0 to 1	            0 to 22.5	            White(Blank)
    // 1 to 2.6	            22.5 to 90	            Red zone                16 parts of 4.21875 degree each Far Below Range<2.4
    // 2.6 to 2.7	        90 to 112.5	            Red zone                1 part of 22.5 degree Below Range (<2.7)
    // 2.7 to 2.8	        112.5 to 135.0	        Yellow Zone             1 part of 22.5 degree within Range(low)
    // 2.8 to 3.2	        135.0 to 225.0	        Green Zone              4 parts of 22.5 degree each within range(Optimal)
    // 3.2 to 3.3	        225 to 247.5	        Yellow Zone             1 part of 22.5 degree within range(high)
    // 3.3 to 3.4	        247.5 to 270	        Red zone                1 part of 22.5 degree Above Range(>3.3)
    // 3.4 to 5	            270 to 337.5	        Red zone                16 parts of 4.21875 degree each Far Above Range(>3.6)
    // 5 to 6	            337.5 to 360	        White(Blank)
    // ------------------------------------------------------------------------------------------------------------------------

    public class PlaySounds
    {
        private static ISimpleAudioPlayer high_0_10sec;
        private static ISimpleAudioPlayer low_0_10sec;
        private static ISimpleAudioPlayer mid_high_05sec;
        private static ISimpleAudioPlayer mid_mid_05sec;
        private static ISimpleAudioPlayer mid_low_05sec;
        private static ISimpleAudioPlayer test_failure;
        private static ISimpleAudioPlayer test_success;

        /// <summary>
        /// 
        /// </summary>
        public static void InitSound()
        {
            if (high_0_10sec == null)
            {
                high_0_10sec = CrossSimpleAudioPlayer.CreateSimpleAudioPlayer();
                high_0_10sec.Load("high_0_10sec.wav");
                low_0_10sec = CrossSimpleAudioPlayer.CreateSimpleAudioPlayer();
                low_0_10sec.Load("low_0_10sec.wav");
                mid_high_05sec = CrossSimpleAudioPlayer.CreateSimpleAudioPlayer();
                mid_high_05sec.Load("mid_high_05sec.wav");
                mid_low_05sec = CrossSimpleAudioPlayer.CreateSimpleAudioPlayer();
                mid_low_05sec.Load("mid_low_05sec.wav");
                mid_mid_05sec = CrossSimpleAudioPlayer.CreateSimpleAudioPlayer();
                mid_mid_05sec.Load("mid_mid_05sec.wav");
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
                if (!low_0_10sec.IsPlaying)
                {
                    StopMid();
                    StopHigh();
                    low_0_10sec.Loop = true;
                    low_0_10sec.Volume = 100;
                    low_0_10sec.Play();
                }
            }
            // play high
            else if (guageData > 3.2)
            {
                if (!high_0_10sec.IsPlaying)
                {
                    StopMid();
                    StopLow();
                    high_0_10sec.Volume = 100;
                    high_0_10sec.Loop = true;
                    high_0_10sec.Play();
                }
            }
            // play mid
            else
            {
                if (!mid_low_05sec.IsPlaying)
                {
                    StopHigh();
                    StopLow();
                    mid_low_05sec.Loop = true;
                    mid_low_05sec.Volume = 100;
                    mid_low_05sec.Play();
                }
            }
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
                    high_0_10sec.Volume = volume;
                    high_0_10sec.Play();
                    break;
                case SoundsEnum.low_0_10sec:
                    StopMid();
                    StopHigh();
                    low_0_10sec.Volume = volume;
                    low_0_10sec.Play();
                    break;
                case SoundsEnum.mid_high_05sec:
                    StopHigh();
                    StopLow();
                    mid_high_05sec.Volume = volume;
                    mid_high_05sec.Play();
                    break;
                case SoundsEnum.mid_low_05sec:
                    StopHigh();
                    StopLow();
                    mid_low_05sec.Volume = volume;
                    mid_low_05sec.Play();
                    break;
                case SoundsEnum.mid_mid_05sec:
                    StopHigh();
                    StopLow();
                    mid_mid_05sec.Volume = volume;
                    mid_mid_05sec.Play();
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
                if (high_0_10sec != null)
                {
                    high_0_10sec.Stop();
                    low_0_10sec.Stop();
                    mid_high_05sec.Stop();
                    mid_low_05sec.Stop();
                    mid_mid_05sec.Stop();
                }
            }
            catch(Exception ex)
            {
                Console.Write(ex);
            }
        }

        public static void StopLow()
        {
            if(low_0_10sec != null)
            low_0_10sec.Stop();
        }

        public static void StopHigh()
        {
            if(high_0_10sec != null)
            high_0_10sec.Stop();
        }

        public static void StopMid()
        {
            if (mid_high_05sec != null)
            {
                mid_high_05sec.Stop();
                mid_low_05sec.Stop();
                mid_mid_05sec.Stop();
            }
        }
    }
}
