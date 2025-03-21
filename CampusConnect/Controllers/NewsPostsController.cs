using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CampusConnect.Models;
using CampusConnect.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CampusConnect.Controllers;

[Authorize]
public class NewsPostsController : Controller
{
    private readonly ApplicationDbContext _context;

    public NewsPostsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: NewsPosts
    public async Task<IActionResult> Index()
    {
        var newsPosts = await _context.NewsPosts
            .Include(n => n.User)
            .OrderByDescending(n => n.PublishDate)
            .ToListAsync();
        return View(newsPosts);
    }

    // GET: NewsPosts/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var newsPost = await _context.NewsPosts
            .Include(n => n.User)
            .FirstOrDefaultAsync(m => m.Id == id);
            
        if (newsPost == null)
        {
            return NotFound();
        }

        return View(newsPost);
    }

    // GET: NewsPosts/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: NewsPosts/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Title,Content")] NewsPost newsPost)
    {
        if (ModelState.IsValid)
        {
            newsPost.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            newsPost.PublishDate = DateTime.UtcNow;
            _context.Add(newsPost);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(newsPost);
    }

    // GET: NewsPosts/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var newsPost = await _context.NewsPosts.FindAsync(id);
        if (newsPost == null)
        {
            return NotFound();
        }

        // Check if the current user is the owner
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (newsPost.UserId != userId)
        {
            return Forbid();
        }

        return View(newsPost);
    }

    // POST: NewsPosts/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Content")] NewsPost newsPost)
    {
        if (id != newsPost.Id)
        {
            return NotFound();
        }

        var existingPost = await _context.NewsPosts.FindAsync(id);
        if (existingPost == null)
        {
            return NotFound();
        }

        // Check if the current user is the owner
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (existingPost.UserId != userId)
        {
            return Forbid();
        }

        if (ModelState.IsValid)
        {
            try
            {
                existingPost.Title = newsPost.Title;
                existingPost.Content = newsPost.Content;
                
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NewsPostExists(newsPost.Id))
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
        return View(newsPost);
    }

    // GET: NewsPosts/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var newsPost = await _context.NewsPosts
            .Include(n => n.User)
            .FirstOrDefaultAsync(m => m.Id == id);
            
        if (newsPost == null)
        {
            return NotFound();
        }

        // Check if the current user is the owner
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (newsPost.UserId != userId)
        {
            return Forbid();
        }

        return View(newsPost);
    }

    // POST: NewsPosts/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var newsPost = await _context.NewsPosts.FindAsync(id);
        if (newsPost == null)
        {
            return NotFound();
        }

        // Check if the current user is the owner
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (newsPost.UserId != userId)
        {
            return Forbid();
        }

        _context.NewsPosts.Remove(newsPost);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool NewsPostExists(int id)
    {
        return _context.NewsPosts.Any(e => e.Id == id);
    }
}