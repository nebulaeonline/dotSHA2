# dotSHA2

dotSHA2 is a wrapper around Google's BoringSSL optimized SHA-2 C++ implementation.

It provides both one-shot and incremental hashing (for streaming data).

Span-based, cross-platform, and verified against the NIST test vectors for each hash length (256/384/512) 100% (643/643) passing on all platforms.

In benchmarking, this library is 10-15% faster than Microsoft's System.Security.Cryptography SHA2 implementation, but supports more ergonomic stream hashing.

Tests and benchmarks are included in the Gihub repository.

[![NuGet](https://img.shields.io/nuget/v/nebulae.dotSHA2.svg)](https://www.nuget.org/packages/nebulae.dotSHA2)

## Features

- **Cross-platform**: Works on Windows, Linux, and macOS (x64 & Apple Silicon).
- **High performance**: Optimized for speed, leveraging native SIMD-enabled code.
- **Easy to use**: Simple API for generating keys and signing/verification.
- **Secure**: Uses Google's BoringSSL implementation, which is widely trusted in the industry.
- **Minimal dependencies**: No external dependencies required (all are included), making it lightweight and easy to integrate.

---

## Requirements

- .NET 8.0 or later
- Windows x64, Linux x64, or macOS (x64 & Apple Silicon)

---

## Usage

One-shot hashing example:

```csharp

using nebulae.dotSHA2;
using System;

ReadOnlySpan<byte> input = "hello world"u8;
Span<byte> hash = stackalloc byte[32]; // SHA-256 = 32 bytes

SHA2.ComputeSHA256(input, hash);

Console.WriteLine(Convert.ToHexString(hash));
// b94d27b9934d3e08a52e52d7da7dabfac484efe37a5380ee9088f7ace2efcde9

```

Stream hashing example:

```csharp

using nebulae.dotSHA2;
using System;
using System.Text;

ReadOnlySpan<byte> part1 = Encoding.UTF8.GetBytes("The quick brown ");
ReadOnlySpan<byte> part2 = Encoding.UTF8.GetBytes("fox jumps over the lazy dog");

using var sha2 = new SHA2(SHA2Algorithm.SHA2_512);
sha2.Update(part1);
sha2.Update(part2);
byte[] hash = sha2.FinalizeHash();

Console.WriteLine(Convert.ToHexString(hash));
// 07e547d9586f6a73f73fbac0435ed76951218fb7d0c8d788a309d785436bbb642e93a252a954f23912547d1e8a3b5ed6e1bfd7097821233fa0538f3db854fee6

```

---

## Installation

You can install the package via NuGet:

```bash

$ dotnet add package nebulae.dotSHA2

```

Or via git:

```bash

$ git clone https://github.com/nebulaeonline/dotSHA2.git
$ cd dotSHA2
$ dotnet build

```

---

## License

MIT

## Roadmap

Unless there are vulnerabilities found, there are no plans to add any new features.