using iText.Kernel.Pdf;
using iText.Kernel.Utils;

namespace PdfLibrary
{
    public class Merger
    {
        public static void Merge(String orig, String dest, List<String> files)
        {
            PdfDocument pdfDocument = new(new PdfReader(orig), new PdfWriter(dest)); // open the reader and the writer to the same doc
            PdfMerger merger = new(pdfDocument); // creating a merger

            // merging every file in the list with the original file
            foreach (String file in files)
            {
                PdfDocument doc = new(new PdfReader(file));
                merger.Merge(doc, 1, doc.GetNumberOfPages());
                doc.Close();
            }

            // closing the original file
            pdfDocument.Close();
        }
    }
}
