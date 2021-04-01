using System.Linq;
using System.Diagnostics;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Jobs;

namespace EncryptedChatApp.Console
{
    public enum Padding
    {
        NoPadding,
        PKCS7
    }
}
