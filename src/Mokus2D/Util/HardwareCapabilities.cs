using System;

namespace Mokus2D.Util
{
    public static class HardwareCapabilities
    {
        private static bool isLowMemory;

        private static bool lowMemoryChecked;

        public static long TotalMemory => throw new NotImplementedException();

        public static bool IsLowMemoryDevice
        {
            get
            {
                if (!lowMemoryChecked)
                {
                    try
                    {
                        long applicationMemoryLimit = ApplicationMemoryLimit;
                        isLowMemory = applicationMemoryLimit < 94371840;
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        isLowMemory = false;
                    }
                    lowMemoryChecked = true;
                }
                return isLowMemory;
            }
        }

        private static long ApplicationMemoryLimit => 268435456L;
    }
}
