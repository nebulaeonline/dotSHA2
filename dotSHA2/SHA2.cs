using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static nebulae.dotSHA2.SHA2Interop;

namespace nebulae.dotSHA2;

public enum SHA2Algorithm
{
    SHA2_256 = 256,
    SHA2_384 = 384,
    SHA2_512 = 512
}

public sealed class SHA2 : IDisposable
{
    private readonly SHA2Algorithm _variant;
    private SHA256_CTX _ctx256;
    private SHA512_CTX _ctx512;
    private readonly int _digestLength;
    private bool _finalized;

    public SHA2(SHA2Algorithm variant)
    {
        _variant = variant;
        _digestLength = variant switch
        {
            SHA2Algorithm.SHA2_256 => 32,
            SHA2Algorithm.SHA2_384 => 48,
            SHA2Algorithm.SHA2_512 => 64,
            _ => throw new ArgumentOutOfRangeException(nameof(variant))
        };

        Reset();
    }

    public void Update(ReadOnlySpan<byte> input)
    {
        var buffer = input.ToArray(); // Interop-safe
        switch (_variant)
        {
            case SHA2Algorithm.SHA2_256:
                SHA2Interop.sha256_update(ref _ctx256, buffer, (nuint)buffer.Length);
                break;
            case SHA2Algorithm.SHA2_384:
                SHA2Interop.sha384_update(ref _ctx512, buffer, (nuint)buffer.Length);
                break;
            case SHA2Algorithm.SHA2_512:
                SHA2Interop.sha512_update(ref _ctx512, buffer, (nuint)buffer.Length);
                break;
        }
    }

    public byte[] FinalizeHash()
    {
        var output = new byte[_digestLength];
        switch (_variant)
        {
            case SHA2Algorithm.SHA2_256:
                SHA2Interop.sha256_final(ref _ctx256, output);
                break;
            case SHA2Algorithm.SHA2_384:
                SHA2Interop.sha384_final(ref _ctx512, output);
                break;
            case SHA2Algorithm.SHA2_512:
                SHA2Interop.sha512_final(ref _ctx512, output);
                break;
        }

        _finalized = true;
        return output;
    }

    public void Reset()
    {
        _finalized = false;
        switch (_variant)
        {
            case SHA2Algorithm.SHA2_256:
                SHA2Interop.sha256_init(out _ctx256);
                break;
            case SHA2Algorithm.SHA2_384:
                SHA2Interop.sha384_init(out _ctx512);
                break;
            case SHA2Algorithm.SHA2_512:
                SHA2Interop.sha512_init(out _ctx512);
                break;
        }
    }

    public void Dispose() => _finalized = true;

    public static void ComputeSHA256(ReadOnlySpan<byte> input, Span<byte> output)
    {
        byte[] inputBytes = input.ToArray();
        byte[] result = new byte[32];
        if (SHA2Interop.sha256_hash(inputBytes, (nuint)inputBytes.Length, result) != 1)
            throw new InvalidOperationException("sha256_hash failed");
        result.CopyTo(output);
    }

    public static void ComputeSHA384(ReadOnlySpan<byte> input, Span<byte> output)
    {
        byte[] inputBytes = input.ToArray();
        byte[] result = new byte[48];
        if (SHA2Interop.sha384_hash(inputBytes, (nuint)inputBytes.Length, result) != 1)
            throw new InvalidOperationException("sha384_hash failed");
        result.CopyTo(output);
    }

    public static void ComputeSHA512(ReadOnlySpan<byte> input, Span<byte> output)
    {
        byte[] inputBytes = input.ToArray();
        byte[] result = new byte[64];
        if (SHA2Interop.sha512_hash(inputBytes, (nuint)inputBytes.Length, result) != 1)
            throw new InvalidOperationException("sha512_hash failed");
        result.CopyTo(output);
    }
}

