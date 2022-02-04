using System;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Extgstate;
using iText.Layout;

namespace draw_in_pdf_spike
{



    public static class HighlightedWord
    {
        public static float[] Points = new float[8] {
            0.4787F, 5.5069F, 3.1454F, 5.5069F, 3.1454F, 5.6379F, 0.4787F, 5.6379F
            //0.4787F, 6.0799F, 2.7019F, 6.0799F, 2.7019F, 6.2108F, 0.4787F, 6.2108F
        };

        public static float PageHeight = 11.6944F;

        public static float PageWidth = 8.2639F;

        public static float[] GetArgs(float pageHeight)
        {
            var factor = pageHeight / PageHeight;

            float x = Points[0],
                y = Points[1],
                width = Points[2] - Points[0],
                height = Points[5] - Points[1];

            return new float[4] {
                x * factor,
                y * factor,
                width * factor,
                height * factor
            };
        }

    }

    class Program
    {
        static void Main(string[] args)
        {
            Draw();
            Console.WriteLine("Hello World!");
        }

        public static void Draw()
        {
            var writer = new PdfWriter("101-after.pdf");
            var pdf = new PdfDocument(new PdfReader("101.pdf"), writer);

            var page = pdf.GetPage(1);

            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SaveState();
            canvas.SetFillColor(iText.Kernel.Colors.ColorConstants.RED);
            canvas.SetExtGState(new PdfExtGState().SetFillOpacity(0.2f));

            var pageHeight = page.GetPageSize().GetHeight();
            var pageWidth = page.GetPageSize().GetWidth();

            var args = HighlightedWord.GetArgs(pageHeight);

            canvas.Rectangle(args[0], pageHeight - args[1] - args[3], args[2], args[3]);

            canvas.Fill();
            canvas.RestoreState();

            pdf.Close();
        }
    }
}
