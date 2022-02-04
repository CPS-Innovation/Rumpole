using System;
using Aspose.Words;

namespace aspose_word_to_pdf_spike
{
    class Program
    {
        static void Main(string[] args)
        {
            Document doc = new Document("MCLOVE MG3.docx");

            // Save the document in PDF format.
            doc.Save("MCLOVE MG3.pdf");

            Console.WriteLine("Hello World!");
        }
    }
}
