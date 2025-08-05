using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using nebulae.dotSHA2;
using System.Security.Cryptography;

[MemoryDiagnoser]
public class Sha2Benchmarks
{
    private byte[] input;

    [GlobalSetup]
    public void Setup()
    {
        input = new byte[1024];
        Random.Shared.NextBytes(input);
    }

    [Benchmark(Baseline = true)]
    public byte[] DotNet_SHA2_256()
    {
        return SHA256.HashData(input);
    }

    [Benchmark]
    public byte[] DotSHA2_256()
    {
        byte[] hash = new byte[32];
        SHA2.ComputeSHA256(input, hash);
        return hash;
    }

    [Benchmark]
    public byte[] DotNet_SHA2_384()
    {
        return SHA384.HashData(input);
    }

    [Benchmark]
    public byte[] DotSHA2_384()
    {
        byte[] hash = new byte[48];
        SHA2.ComputeSHA384(input, hash);
        return hash;
    }

    [Benchmark]
    public byte[] DotNet_SHA2_512()
    {
        return SHA512.HashData(input);
    }

    [Benchmark]
    public byte[] DotSHA2_512()
    {
        byte[] hash = new byte[64];
        SHA2.ComputeSHA512(input, hash);
        return hash;
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        BenchmarkRunner.Run<Sha2Benchmarks>();
    }
}
