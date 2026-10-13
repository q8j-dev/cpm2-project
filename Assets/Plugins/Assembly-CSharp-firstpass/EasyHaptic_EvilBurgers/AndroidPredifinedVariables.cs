namespace EasyHaptic_EvilBurgers
{
	public sealed class AndroidPredifinedVariables
	{
		public static long lightImpactDuration = 40L;

		public static long mediumImpactDuration = 75L;

		public static long heavyImpactDuration = 120L;

		public static int lightImpactAmplitude = 40;

		public static int mediumImpactAmplitude = 120;

		public static int heavyImpactAmplitude = 255;

		public static long[] successPatternDuration = new long[4] { 30L, 140L, 30L, 140L };

		public static int[] successPatternAmplitude = new int[4] { 20, 75, 20, 75 };

		public static long[] warningPatternDuration = new long[6] { 50L, 100L, 50L, 100L, 50L, 100L };

		public static int[] warningPatternAmplitude = new int[6] { 60, 200, 60, 200, 60, 200 };

		public static long[] failurePatternDuration = new long[4] { 50L, 500L, 50L, 500L };

		public static int[] failurePatternAmplitude = new int[4] { 50, 150, 50, 150 };
	}
}
