namespace Mokus2D.Util.Data;

public class CryptUtils
{
    private static readonly byte[] ENCRYPTION_KEY =
    [
        13, 22, 36, 4, 5, 6, 7, 8, 9, 10,
        11, 12, 13, 144, 15, 216, 1, 18, 19, 20,
        21, 22, 23, 24
    ];

    private static readonly byte[] ENCRYPTION_VECTOR = [87, 17, 59, 30, 44, 31, 73, 1];

    public static byte[] RunProtector(byte[] input, bool encrypt)
    {
        return input;
    }
}
