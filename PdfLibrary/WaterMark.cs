using iText.IO.Font;
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Extgstate;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;

namespace PdfLibrary
{
    public class WaterMark
    {
        /// <summary>
        /// Adds a WaterMark in text in a Pdf Document
        /// </summary>
        /// <param name="srcPath">Orig file path</param>
        /// <param name="destPath">Destination file path</param>
        /// <param name="text">Text of the watermark</param>
        /// <param name="angle">Angle of the rotation in degrees</param>
        /// <param name="opacity">Opacity of the text</param>
        public static void AddWaterMark(String srcPath, String destPath, String text, double angle, float opacity)
        {
            PdfDocument pdfDoc = new(new PdfReader(srcPath), new PdfWriter(destPath));
            Document doc = new(pdfDoc);
            PdfFont font = PdfFontFactory.CreateFont(FontProgramFactory.CreateFont(StandardFonts.HELVETICA));
            Paragraph paragraph = new Paragraph(text).SetFont(font).SetFontSize(30);

            PdfExtGState gs1 = new PdfExtGState().SetFillOpacity(opacity);

            for (int i = 1; i <= pdfDoc.GetNumberOfPages(); i++)
            {
                PdfPage pdfPage = pdfDoc.GetPage(i);
                Rectangle pageSize = pdfPage.GetPageSize();
                float x = (pageSize.GetLeft() + pageSize.GetRight()) / 2;
                float y = (pageSize.GetTop() + pageSize.GetBottom()) / 2;
                PdfCanvas over = new(pdfPage);
                over.SaveState();
                over.SetExtGState(gs1);
                doc.ShowTextAligned(paragraph, x, y, i, TextAlignment.CENTER, VerticalAlignment.TOP, (float)angle);
                over.RestoreState();
            }

            doc.Close();
        }

    }
}