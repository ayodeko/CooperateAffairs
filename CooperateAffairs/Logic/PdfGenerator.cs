using SelectPdf;

namespace CooperateAffairs.Logic
{
	public interface IPdf
	{
		public void GetPdf();
	}
	public class PdfGenerator : IPdf
	{
		string htmlString = @"<html>
 <body>
  Hello World from selectpdf.com.
 </body>
</html>
";
		public void GetPdf()
		{
			var converter = new HtmlToPdf();
			var doc = converter.ConvertHtmlString(htmlString);
			doc.Save(@"C:\Appzone\Sample.pdf");
		}
	}
}
