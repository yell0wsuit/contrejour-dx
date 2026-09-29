using System;

namespace Mokus2D.Util
{
    public static class ExceptionUtil
    {
        public static void Throw(Exception exception)
        {
            if (!Mokus2DGame.Instance.IsExiting)
            {
                throw exception;
            }
        }
    }
}
