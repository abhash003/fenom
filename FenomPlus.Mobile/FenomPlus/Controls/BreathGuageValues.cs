using System;

namespace FenomPlus.Controls
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
    // ------------------------------------------------------------------------------------------------------------------------
    // Breath Flow(lpm)     On the Gauge(degree)    Colour on the UI        Comments	
    // 0 to 1	            0 to 22.5	            White(Blank)
    // 1 to 2.3	            22.5 to 90	            Red zone                16 parts of 4.21875 degree each Far Below Range<2.4
    // 2.3 to 2.4	        90 to 112.5	            Red zone                1 part of 22.5 degree Below Range (<2.7)
    // 2.4 to 2.7	        112.5 to 135.0	        Yellow Zone             1 part of 22.5 degree within Range(low)
    // 2.7 to 3.3	        135.0 to 225.0	        Green Zone              4 parts of 22.5 degree each within range(Optimal)
    // 3.3 to 3.6	        225 to 247.5	        Yellow Zone             1 part of 22.5 degree within range(high)
    // 3.6 to 3.7	        247.5 to 270	        Red zone                1 part of 22.5 degree Above Range(>3.3)
    // 3.7 to 5	            270 to 337.5	        Red zone                16 parts of 4.21875 degree each Far Above Range(>3.6)
    // 5 to 6	            337.5 to 360	        White(Blank)
    // ------------------------------------------------------------------------------------------------------------------------

    public static class BreathGuageValues
    {
        public static float White1      = 0.0f;
        public static float White1Top   = 1.0f;
        public static float Red1        = White1Top;
        public static float Red1Top     = 2.3f;
        public static float Red2        = Red1Top;
        public static float Red2Top     = 2.4f;
        public static float Yellow1     = Red2Top;
        public static float Yellow1Top  = 2.7f;
        public static float Green1      = Yellow1Top;
        public static float Green1Top   = 2.8f;
        public static float Green2      = Green1Top;
        public static float Green2Top   = 3.2f;
        public static float Green3      = Green2Top;
        public static float Green3Top   = 3.3f;
        public static float Yellow2     = Green3Top;
        public static float Yellow2Top  = 3.6f;
        public static float Red3        = Yellow2Top;
        public static float Red3Top     = 3.7f;
        public static float Red4        = Red3Top;
        public static float Red4Top     = 5.0f;
        public static float White2      = Red4Top;
        public static float White2Top   = 6.0f;
    }
}
