using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyProject.DAL;
using MyProject.Models;
using MyProject.Utilities.Enums;
using MyProject.Utilities.Extensions;
using System.Drawing;

namespace MyProject.Areas.Admin.Controllers
{
    
        [Area("Admin")]
        public class ProductController : Controller
        {

            private readonly AppDbContext _context;
            private readonly IWebHostEnvironment _env;
        public ProductController(AppDbContext context, IWebHostEnvironment env)
            {
               _context = context;
                _env = env;
        }
            public async Task<IActionResult> Index()
            {
                List<Product> products = await _context.Products.ToListAsync();
                return View(products);
            }
            [HttpGet]
            public IActionResult Create()
            {
                return View();
            }
            [HttpPost]
            public async Task<IActionResult> Create(Product product)
            {
            if (!ModelState.IsValid)
            {
                return View();
            }
            if (!product.Photo.ValidateType("image/"))
            {
                ModelState.AddModelError("Photo", "File type must be image");
                return View();
            }
            if (product.Photo.ValidateSize(2, FileSize.MB))
            {
                ModelState.AddModelError("Photo", "Image size must be max 2MB");
                return View();
            }

            product.Image = await product.Photo.CreateFile(_env.WebRootPath, "assets", "images");
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");

            }
            public async Task<IActionResult> Update(int? id)
            {
                if (id == null || id < 1)
                {
                    return BadRequest();
                }
                Product product = await _context.Products.FirstOrDefaultAsync(c => c.Id == id);
                if (product is null)
                {
                    return NotFound();
                }
                return View(product);
            }
            [HttpPost]
            public async Task<IActionResult> Update(int? id, Product product)
            {
                if (id == null || id < 1)
                {
                    return BadRequest();
                }
                Product dbProduct = await _context.Products.FirstOrDefaultAsync(c => c.Id == id);
                if (dbProduct is null)
                {
                    return NotFound();
                }
                if (!ModelState.IsValid)
                {
                    return View(dbProduct);
                }
                bool result = await _context.Products.AnyAsync(c => c.Title.ToLower() == product.Title.ToLower() && c.Id != id);
                if (result)
                {
                    ModelState.AddModelError("Title", "This product already exists");
                    return View(dbProduct);
                }
                dbProduct.Title = product.Title;
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            public async Task<IActionResult> Delete(int? id)
            {
                if (id == null || id < 1)
                {
                    return BadRequest();
                }
                Product product = await _context.Products.FirstOrDefaultAsync(c => c.Id == id);
                if (product is null)
                {
                    return NotFound();
                }
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            public async Task<IActionResult> Detail(int? id)
            {
                if (id == null || id < 1)
                {
                    return BadRequest();
                }
                Product product = await _context.Products.FirstOrDefaultAsync(c => c.Id == id);
                if (product is null)
                {
                    return NotFound();
                }
                return View(product);
            }
        }
    }