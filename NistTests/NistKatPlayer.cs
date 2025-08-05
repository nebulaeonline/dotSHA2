using System.Globalization;

using nebulae.dotSHA2;

namespace NistTests;

internal class NistKatPlayer
{
    public static void RunSha3Kat(string path)
    {
        using var reader = new StreamReader(path);
        string? line;
        SHA2Algorithm? currentAlgorithm = null;

        int testCount = 0;
        int failCount = 0;
        int bitLength = -1;

        while ((line = reader.ReadLine()) != null)
        {
            line = line.Trim();

            if (line.StartsWith("Len ="))
            {
                bitLength = int.Parse(line.Substring(5).Trim());
            }

            if (line.StartsWith("[L ="))
            {
                if (line.Contains("32")) currentAlgorithm = SHA2Algorithm.SHA2_256;
                else if (line.Contains("48")) currentAlgorithm = SHA2Algorithm.SHA2_384;
                else if (line.Contains("64")) currentAlgorithm = SHA2Algorithm.SHA2_512;
                else throw new Exception($"Unknown digest size in line: {line}");
            }

            if (line.StartsWith("Msg ="))
            {
                if (bitLength < 0)
                    throw new Exception("No valid Len = line before Msg");

                string msgHex;

                if (bitLength == 0)
                    msgHex = "";
                else
                    msgHex = line.Substring(5).Trim();
                
                byte[] msg = HexToBytes(msgHex);

                // Truncate to bitLength
                int byteLen = (bitLength + 7) / 8;
                if (msg.Length > byteLen)
                    Array.Resize(ref msg, byteLen);

                int bitsExtra = bitLength % 8;
                if (bitsExtra > 0 && msg.Length > 0)
                {
                    int mask = 0xFF << (8 - bitsExtra);
                    msg[^1] &= (byte)mask;
                }

                string? mdLine;
                do { mdLine = reader.ReadLine(); } while (mdLine != null && !mdLine.TrimStart().StartsWith("MD ="));
                if (mdLine == null) throw new Exception("Expected MD = line");

                var expectedHex = mdLine.Substring(5).Trim();
                byte[] expected = HexToBytes(expectedHex);

                if (currentAlgorithm == null) throw new Exception("Digest length (L =) not declared before test block");

                using var sha2 = new SHA2(currentAlgorithm.Value);
                sha2.Update(msg);
                byte[] actual = sha2.FinalizeHash();
                byte[] actual2; 

                switch (currentAlgorithm.Value)
                {
                    case SHA2Algorithm.SHA2_256:
                        actual2 = new byte[32];
                        SHA2.ComputeSHA256(msg, actual2.AsSpan<byte>());
                        break;
                    case SHA2Algorithm.SHA2_384:
                        actual2 = new byte[48];
                        SHA2.ComputeSHA384(msg, actual2.AsSpan<byte>());
                        break;
                    case SHA2Algorithm.SHA2_512:
                        actual2 = new byte[64];
                        SHA2.ComputeSHA512(msg, actual2.AsSpan<byte>());
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(currentAlgorithm), "Unknown SHA2 algorithm");
                }

                testCount += 2;
                if (!ByteArraysEqual(actual, expected))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"FAIL ({currentAlgorithm}) - Test #{testCount}");
                    Console.WriteLine($"Len = {bitLength}");
                    Console.WriteLine($"Input = {BitConverter.ToString(msg)}");
                    Console.WriteLine($"Expected = {BitConverter.ToString(expected)}");
                    Console.WriteLine($"Actual   = {BitConverter.ToString(actual)}");
                    Console.ResetColor();
                    failCount++;
                }

                if (!ByteArraysEqual(actual2, expected))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"FAIL ({currentAlgorithm}) - Test #{testCount}");
                    Console.WriteLine($"Len = {bitLength}");
                    Console.WriteLine($"Input = {BitConverter.ToString(msg)}");
                    Console.WriteLine($"Expected = {BitConverter.ToString(expected)}");
                    Console.WriteLine($"Actual   = {BitConverter.ToString(actual2)}");
                    Console.ResetColor();
                    failCount++;
                }

                // reset for next round
                bitLength = -1;
            }
        }

        Console.WriteLine($"Completed {testCount} test vectors ({failCount} failed)");
    }

    private static bool ByteArraysEqual(ReadOnlySpan<byte> a, ReadOnlySpan<byte> b)
    {
        return a.Length == b.Length && a.SequenceEqual(b);
    }

    private static byte[] HexToBytes(string hex)
    {
        if (hex.Length % 2 != 0)
            hex = hex + "0"; // pad if needed (shouldn't happen in KATs)

        byte[] result = new byte[hex.Length / 2];
        for (int i = 0; i < result.Length; i++)
            result[i] = byte.Parse(hex.Substring(i * 2, 2), NumberStyles.HexNumber);
        return result;
    }
}
