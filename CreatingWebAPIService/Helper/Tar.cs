using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace CreatingWebAPIService.Helper
{
    public class Tar
    {
        public static List<TarEntry> ExtractTarGzEntries(Stream stream)
        {
            var gzip = new GZipStream(stream, CompressionMode.Decompress);
            var mTar = new MemoryStream();
            gzip.CopyTo(mTar);
            mTar.Seek(0, SeekOrigin.Begin);
            return ExtractTarEntries(mTar);
        }

        private static List<TarEntry> ExtractTarEntries(Stream stream)
        {
            var lTarEntries = new List<TarEntry>();
            var buffer = new byte[100];
            while (true)
            {
                stream.Read(buffer, 0, 100);
                var name = Encoding.ASCII.GetString(buffer).Trim('\0');
                if (String.IsNullOrWhiteSpace(name))
                    break;
                stream.Seek(24, SeekOrigin.Current);
                stream.Read(buffer, 0, 12);
                var size = Convert.ToInt64(Encoding.ASCII.GetString(buffer, 0, 12).Trim('\0'), 8);
                stream.Seek(376L, SeekOrigin.Current);
                var buf = new byte[size];
                stream.Read(buf, 0, buf.Length);
                lTarEntries.Add(new TarEntry
                {
                    Contents = buf,
                    FileName = name
                });

                var pos = stream.Position;

                var offset = 512 - (pos % 512);
                if (offset == 512)
                    offset = 0;

                stream.Seek(offset, SeekOrigin.Current);
            }
            return lTarEntries;
        }

        public class TarEntry
        {
            public string FileName { get; set; }
            public byte[] Contents { get; set; }
        }
    }
}