using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DanyaHub.Data;
using DanyaHub.Models;

namespace DanyaHub.Controllers
{
    public class FilesController : Controller
    {
        private readonly Context _context;

        public FilesController(Context context)
        {
            _context = context;
        }

        [Authorize(Roles = "User, Admin")]
        public async Task<IActionResult> Index()
        {
            var files = await _context.Files.ToListAsync();
            return View(files);
        }

        [Authorize(Roles = "User, Admin")]
        public async Task<IActionResult> Download(int id)
        {
            var file = await _context.Files.FindAsync(id);
            if (file == null)
            {
                return NotFound();
            }

            if (!User.IsInRole("Admin") && !file.FileName.EndsWith(".doc") && !file.FileName.EndsWith(".docx"))
            {
                return Forbid();
            }

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", file.FilePath);
            return PhysicalFile(filePath, "application/octet-stream", file.FileName);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var file = await _context.Files.FindAsync(id);
            if (file == null)
            {
                return NotFound();
            }

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", file.FilePath);
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            _context.Files.Remove(file);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                ModelState.AddModelError("file", "Не выбран файл для загрузки.");
                return View();
            }

            var uploadsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
            if (!Directory.Exists(uploadsDirectory))
            {
                Directory.CreateDirectory(uploadsDirectory);
            }

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadsDirectory, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var fileModel = new FileModel
            {
                FileName = file.FileName,
                FilePath = fileName 
            };

            _context.Files.Add(fileModel);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
