using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CampusConnect.Models;
using CampusConnect.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CampusConnect.Controllers;

[Authorize]
public class ResourcesController : Controller
{
    private readonly ApplicationDbContext _context;

    public ResourcesController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Resources
    public async Task<IActionResult> Index()
    {
        var resources = await _context.Resources
            .Include(r => r.User)
            .OrderByDescending(r => r.UploadDate)
            .ToListAsync();
        return View(resources);
    }

    // GET: Resources/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var resource = await _context.Resources
            .Include(r => r.User)
            .FirstOrDefaultAsync(m => m.Id == id);
            
        if (resource == null)
        {
            return NotFound();
        }

        return View(resource);
    }

    // GET: Resources/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Resources/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Title,Description,FileUrl,FileType")] Resource resource)
    {
        if (ModelState.IsValid)
        {
            resource.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            resource.UploadDate = DateTime.UtcNow;
            _context.Add(resource);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(resource);
    }

    // GET: Resources/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var resource = await _context.Resources.FindAsync(id);
        if (resource == null)
        {
            return NotFound();
        }

        // Check if the current user is the owner
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (resource.UserId != userId)
        {
            return Forbid();
        }

        return View(resource);
    }

    // POST: Resources/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,FileUrl,FileType")] Resource resource)
    {
        if (id != resource.Id)
        {
            return NotFound();
        }

        var existingResource = await _context.Resources.FindAsync(id);
        if (existingResource == null)
        {
            return NotFound();
        }

        // Check if the current user is the owner
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (existingResource.UserId != userId)
        {
            return Forbid();
        }

        if (ModelState.IsValid)
        {
            try
            {
                existingResource.Title = resource.Title;
                existingResource.Description = resource.Description;
                existingResource.FileUrl = resource.FileUrl;
                existingResource.FileType = resource.FileType;
                
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ResourceExists(resource.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        return View(resource);
    }

    // GET: Resources/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var resource = await _context.Resources
            .Include(r => r.User)
            .FirstOrDefaultAsync(m => m.Id == id);
            
        if (resource == null)
        {
            return NotFound();
        }

        // Check if the current user is the owner
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (resource.UserId != userId)
        {
            return Forbid();
        }

        return View(resource);
    }

    // POST: Resources/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var resource = await _context.Resources.FindAsync(id);
        if (resource == null)
        {
            return NotFound();
        }

        // Check if the current user is the owner
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (resource.UserId != userId)
        {
            return Forbid();
        }

        _context.Resources.Remove(resource);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool ResourceExists(int id)
    {
        return _context.Resources.Any(e => e.Id == id);
    }
}