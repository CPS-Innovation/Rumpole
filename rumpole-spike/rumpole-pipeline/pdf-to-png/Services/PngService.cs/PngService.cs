using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Docnet.Core;
using Docnet.Core.Converters;
using Docnet.Core.Models;

namespace Services.PngService
{
    public class PngService
    {
        private readonly IDocLib _docnet;
        public PngService(DocNetInstance docnetInstance)
        {
            _docnet = docnetInstance.GetDocNet();
        }

        public List<Stream> GetPngStreams(MemoryStream stream)
        {
            using var docReader = _docnet.GetDocReader(
                    stream.ToArray(),
                    new PageDimensions(1080, 1920));

            var pageCount = docReader.GetPageCount();

            var pngStreams = new List<Stream>();
            for (int i = 0; i < pageCount; i++)
            {
                using var pageReader = docReader.GetPageReader(i);

                var rawBytes = pageReader.GetImage();
                var remover = new NaiveTransparencyRemover();
                remover.Convert(rawBytes);

                var width = pageReader.GetPageWidth();
                var height = pageReader.GetPageHeight();

                using var bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);

                AddBytes(bmp, rawBytes);

                var pngStream = new MemoryStream();

                bmp.Save(pngStream, ImageFormat.Png);
                pngStream.Seek(0, SeekOrigin.Begin);

                pngStreams.Add(pngStream);
            }

            return pngStreams;
        }

        private void AddBytes(Bitmap bmp, byte[] rawBytes)
        {
            var rect = new Rectangle(0, 0, bmp.Width, bmp.Height);

            var bmpData = bmp.LockBits(rect, ImageLockMode.WriteOnly, bmp.PixelFormat);
            var pNative = bmpData.Scan0;

            Marshal.Copy(rawBytes, 0, pNative, rawBytes.Length);
            bmp.UnlockBits(bmpData);
        }
    }

}