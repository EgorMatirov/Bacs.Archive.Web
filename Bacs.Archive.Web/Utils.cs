using System;
using System.Collections.Generic;
using System.IO;

namespace Bacs.Archive.Web
{
    internal static class Utils
    {
        public static T ParseEnum<T>(string value)
        {
            return (T) Enum.Parse(typeof(T), value);
        }

        public static IEnumerable<byte> AsEnumerable(this Stream stream)
        {
            if (stream == null) yield break;
            for (var i = stream.ReadByte(); i != -1; i = stream.ReadByte())
                yield return (byte)i;
        }
    }
}