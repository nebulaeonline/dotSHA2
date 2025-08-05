using System.Runtime.InteropServices;

namespace nebulae.dotSHA2;

public static class SHA2Interop
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct SHA256_CTX
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public uint[] h;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public uint[] Nl_Nh;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public uint[] data;
        public uint num;
        public uint md_len;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct SHA512_CTX
    {
        public fixed ulong h[8];
        public ulong Nl;
        public ulong Nh;
        public fixed byte data[128];
        public uint num;
        public uint md_len;
    }

    static SHA2Interop()
    {
        // Ensure the SHA2 library is loaded
        SHA2Library.Init();
    }

    [DllImport("sha2", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int sha256_init(out SHA256_CTX ctx);

    [DllImport("sha2", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int sha256_update(ref SHA256_CTX ctx, byte[] input, UIntPtr len);

    [DllImport("sha2", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int sha256_final(ref SHA256_CTX ctx, byte[] output);

    [DllImport("sha2", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int sha256_hash(byte[] input, UIntPtr len, byte[] output);

    [DllImport("sha2", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int sha384_init(out SHA512_CTX ctx);

    [DllImport("sha2", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int sha384_update(ref SHA512_CTX ctx, byte[] input, UIntPtr len);

    [DllImport("sha2", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int sha384_final(ref SHA512_CTX ctx, byte[] output);

    [DllImport("sha2", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int sha384_hash(byte[] input, UIntPtr len, byte[] output);

    [DllImport("sha2", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int sha512_init(out SHA512_CTX ctx);

    [DllImport("sha2", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int sha512_update(ref SHA512_CTX ctx, byte[] input, UIntPtr len);

    [DllImport("sha2", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int sha512_final(ref SHA512_CTX ctx, byte[] output);

    [DllImport("sha2", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int sha512_hash(byte[] input, UIntPtr len, byte[] output);
}
