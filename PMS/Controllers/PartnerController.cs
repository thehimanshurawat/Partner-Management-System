using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PMS.Areas.Identity.Data;
using PMS.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

[Authorize(Roles = "Partner")]
public class PartnerController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public PartnerController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // GET: Partner/Dashboard (To List Created Offers)
    public async Task<IActionResult> Dashboard()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }

        var offers = await _context.Offers
            .Where(o => o.PartnerEmail == user.Email)
            .ToListAsync();

        return View(offers);
    }

    // GET: Partner/CreateOffer
    public IActionResult CreateOffer()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateOffer(Offer offer)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            // ✅ Assign PartnerId, Email, Default Status
            offer.PartnerEmail = user.Email;
            offer.PartnerId = user.Id;
            offer.Status = OfferStatus.Pending; // Default to pending approval

            _context.Add(offer);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Offer Created Successfully. Await Admin Approval.";
            return RedirectToAction("Dashboard");
        }



        // ❌ Debug validation errors
        foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
        {
            Console.WriteLine(error.ErrorMessage);
        }

        TempData["ErrorMessage"] = "Invalid input. Please check your entries.";
        return View(offer);
    }
    [HttpGet]
    public IActionResult EditOffer(int id)
    {
        var offer = _context.Offers.Find(id); // Fetch offer from DB
        if (offer == null)
        {
            return NotFound(); // Return 404 if offer not found
        }
        return View(offer); // Return the EditOffer.cshtml view
    }
    [HttpPost]
    public IActionResult EditOffer(Offer model)
    {
        var offer = _context.Offers.Find(model.OfferId);
        if (offer == null)
        {
            return NotFound();
        }

        offer.OfferName = model.OfferName;
        offer.Description = model.Description;
        offer.StartDate = model.StartDate;
        offer.EndDate = model.EndDate;
        offer.Cost = model.Cost;
        offer.Status = OfferStatus.Pending; // Reset to pending after edit

        _context.SaveChanges();

        return RedirectToAction("Dashboard");
    }
    [HttpGet]
    public IActionResult GetOfferCounts()
    {
        var userId = User.Identity.Name;
        var all = _context.Offers.Count(o => o.PartnerId == userId);
        var approved = _context.Offers.Count(o => o.Status == OfferStatus.Approved && o.PartnerId == userId);
        var pending = _context.Offers.Count(o => o.Status == OfferStatus.Pending && o.PartnerId == userId);
        var rejected = _context.Offers.Count(o => o.Status == OfferStatus.Rejected && o.PartnerId == userId);

        return Json(new { all, approved, pending, rejected });
    }
}
