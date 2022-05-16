using iText.Kernel.Pdf;
using iText.Kernel.Utils;

namespace PdfLibrary
{
    public static class Splitter
    {
        /// <summary>
        /// Split the file <paramref name="orig"/> into <paramref name="dest"/> with a range of pages definite by <paramref name="firstPage"/> and <paramref name="lastPage"/>
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="dest"></param>
        /// <param name="firstPage"></param>
        /// <param name="lastPage"></param>
        /// <returns></returns>
        public static bool SplitPdfByPage(String orig, String dest, int firstPage, int lastPage)
        {
            String destFile = (firstPage == 1) ? "tmp1.pdf" : "tmp2.pdf"; // setting the temp file's name
            if (!File.Exists(orig)) return false; // checking if the file exists

            PdfDocument document = new(new PdfReader(orig)); // open and parsing the file
            String pathTemp = Path.Combine(Path.GetTempPath(), "tmp{0}.pdf"); // seting temp directory to the temp system directory

            List<int> pages = new() { firstPage, (lastPage + 1) }; // create the list with the pages required

            // splitting the file and getting every part of document in a list
            IList<PdfDocument> pdfDocuments = new CustomPdfSplitter(document, pathTemp).SplitByPageNumbers(pages);
            foreach (PdfDocument pdfDocument in pdfDocuments) pdfDocument.Close(); // closing all the documents created before
            document.Close(); // close the original document

            FileInfo result = new(Path.Combine(Path.GetTempPath(), destFile)); // obtain the right file
            result.CopyTo(dest, true); // copy the rigth file to the destination
            String[] tmpFiles = Directory.GetFiles(Path.GetTempPath(), "tmp*.pdf"); // obtain temp files
            foreach (String file in tmpFiles) File.Delete(file); // delete temp files
            return true; //return true if all it's okay
        }

        // overrride part of the original class of the api iText 7
        private class CustomPdfSplitter : PdfSplitter
        {

            private readonly String dest;
            private int partNumber = 1;

            public CustomPdfSplitter(PdfDocument pdfDocument, String dest) : base(pdfDocument)
            {
                this.dest = dest;
            }

            protected override PdfWriter GetNextPdfWriter(PageRange documentPageRange)
            {
                return new PdfWriter(String.Format(dest, partNumber++));
            }
        }
    }
}
