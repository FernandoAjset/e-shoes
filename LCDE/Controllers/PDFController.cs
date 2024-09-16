using LCDE.Servicios;
using Microsoft.AspNetCore.Mvc;

namespace LCDE.Controllers
{
    public class PDFController : Controller
    {
        private readonly IFileRepository fileRepository;

        public PDFController(IFileRepository fileRepository)
        {
            this.fileRepository = fileRepository;
        }
        public async Task<IActionResult> Show(string pdfFilePath)
        {
            var attachment = await fileRepository.DownloadFileAsFormFileAsync(pdfFilePath, "venta.pdf");
            if (attachment == null)
            {
                return NotFound();
            }
            return File(attachment.OpenReadStream(), "application/pdf");
        }

        public IActionResult ShowPDF(string filePath)
        {
            return RedirectToAction("Show", "PDF", new { pdfFilePath = filePath });
        }

    }
}