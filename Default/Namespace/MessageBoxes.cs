using System;

namespace Default.Namespace;

public static class MessageBoxes
{
    public static void ShowNotSignedToXBoxError(AsyncCallback callback)
    {
        ShowOKWindow(callback, "ACCOUNT_ERROR", "NOT_SIGNED_TO_XBOX");
    }

    public static void ShowInternetError(AsyncCallback callback)
    {
        ShowOKWindow(callback, " ", "NO_INTERNET");
    }

    private static void ShowOKWindow(AsyncCallback callback, string title, string message)
    {
    }

    public static void ShowInternetError()
    {
        ShowInternetError(null);
    }
}
