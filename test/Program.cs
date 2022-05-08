using PdfLibrary;

namespace Test
{
    class Program
    {
        static void Main()
        {
            String orig = Path.Combine("C:\\Users\\perez\\Downloads", "SolicitudBecaDistanciaLucia.pdf");
            String dest = Path.Combine("C:\\Users\\perez\\Documents", "Prueba.pdf");
            String file2 = Path.Combine("C:\\Users\\perez\\Downloads", "TestPdf.pdf");

            List<String> list = new()
            {
                file2
            };

            Splitter.SplitPdfByPage(orig, dest, 1, 2);
            Merger.Merge(orig, dest, list);
            
        }
    }
}