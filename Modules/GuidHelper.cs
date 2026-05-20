using System;
using System.Security.Cryptography;
using System.Text;

namespace DbsContentApi;

internal static class GuidHelper
{
    public static Guid ToDeterministicGuid(string input)
    {
        using MD5 md5 = MD5.Create();
        byte[] hash = md5.ComputeHash(Encoding.Default.GetBytes(input));
        return new Guid(hash);
    }
}
