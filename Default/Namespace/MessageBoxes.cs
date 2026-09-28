using System;

namespace Default.Namespace;

public static class MessageBoxes
{
    public static void ShowNotSignedToXBoxError(AsyncCallback callback)
    {
        ShowOKWindow();
    }

    public static void ShowInternetError(AsyncCallback callback)
    {
        ShowOKWindow();
    }

    private static void ShowOKWindow()
    {
    }

    public static void ShowInternetError()
    {
        ShowInternetError(null);
    }
}
