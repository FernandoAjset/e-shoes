using Microsoft.AspNetCore.Mvc;

namespace LCDE.Controllers
{
    public class PDFController : Controller
    {
        public IActionResult Show(string pdfFilePath)
        {
            var fileBytes = System.IO.File.ReadAllBytes(pdfFilePath);
            return File(fileBytes, "application/pdf");
        }

        public IActionResult ShowPDF(string filePath)
        {
            return RedirectToAction("Show", "PDF", new { pdfFilePath = filePath });
        }

    }
}
