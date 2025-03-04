using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PMS.Areas.Identity.Data;
using PMS.Models;
using System.Linq;
using System.Threading.Tasks;

namespace PMS.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController(UserManager<ApplicationUser> userManager, ApplicationDbContext context) : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly ApplicationDbContext _context = context;

        // ✅ Admin Dashboard Route
        public IActionResult Dashboard()
        {
            return View();
        }

        // ✅ Display All Registered Partners
        public IActionResult PartnersList()
        {
            var partners = _userManager.Users.Where(u => u.IsVerified).ToList();
            return View(partners);
        }

        // ✅ Remove a Partner (Move to Pending Verification)
        [HttpPost]
        public async Task<IActionResult> RemovePartner(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                user.IsVerified = false;
                user.RejectionMessage = null; // Clear any previous rejection
                await _userManager.UpdateAsync(user);
            }
            return RedirectToAction("PartnersList");
        }

        // ✅ Display Pending Verifications List
        public IActionResult PendingVerifications()
        {
            var users = _userManager.Users.Where(u => !u.IsVerified && string.IsNullOrEmpty(u.RejectionMessage)).ToList();
            return View(users);
        }

        // ✅ Approve User Verification & Update Count
        [HttpPost]
        public async Task<IActionResult> ApproveVerification(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                user.IsVerified = true;
                await _userManager.UpdateAsync(user);
            }
            return RedirectToAction("PendingVerifications");
        }

        [HttpPost]
        public async Task<IActionResult> RejectVerification(string userId, string rejectionMessage)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                user.IsVerified = false;
                user.RejectionMessage = rejectionMessage; // Store rejection reason
                await _userManager.UpdateAsync(user);
            }
            return RedirectToAction("PendingVerifications");
        }

        // ✅ Display Rejected Applications
        public IActionResult RejectedApplications()
        {
            var rejectedUsers = _userManager.Users
                                .Where(u => !u.IsVerified && !string.IsNullOrEmpty(u.RejectionMessage))
                                .ToList();
            return View(rejectedUsers);
        }

        [HttpPost]
        public async Task<IActionResult> MoveToPendingVerification(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                user.IsVerified = false;
                user.RejectionMessage = null; // Clear rejection message
                await _userManager.UpdateAsync(user);
            }
            return RedirectToAction("RejectedApplications");
        }

        // ✅ API Endpoint to Get Real-Time Counts (Total Partners & Pending Approvals)
        [HttpGet]
        public IActionResult GetPendingVerificationsCount()
        {
            var totalPartners = _userManager.Users.Count(u => u.IsVerified);
            var pendingApprovals = _userManager.Users.Count(u => !u.IsVerified && string.IsNullOrEmpty(u.RejectionMessage));
            var rejectedApplications = _userManager.Users.Count(u => !u.IsVerified && !string.IsNullOrEmpty(u.RejectionMessage));

            return Json(new { totalPartners, pendingApprovals, rejectedApplications });
        }

        // ✅ Display All Offers (Admin View)
        public async Task<IActionResult> OffersList()
        {
            var offers = await _context.Offers.Include(o => o.Partner).ToListAsync();
            return View(offers);

        }

        // ✅ Approve an Offer
        [HttpPost]
        public async Task<IActionResult> ApproveOffer(int offerId)
        {
            var offer = await _context.Offers.FindAsync(offerId);
            if (offer != null)
            {
                offer.Status = OfferStatus.Approved;
                offer.Message = null; // Clear rejection message if approved
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("OffersList");
        }

        // Reject an Offer
        [HttpPost]
        public async Task<IActionResult> RejectOffer(int offerId, string message)
        {
            var offer = await _context.Offers.FindAsync(offerId);
            if (offer != null)
            {
                offer.Status = OfferStatus.Rejected;
                offer.Message = message; // Save rejection reason
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("OffersList");
        }

        // ✅ Get Offer Counts for Dashboard
        [HttpGet]
        public IActionResult GetOfferCounts()
        {
            var allOffers = _context.Offers.Count();
            var pendingOffers = _context.Offers.Count(o => o.Status.ToString() == "Pending");
            var approvedOffers = _context.Offers.Count(o => o.Status.ToString() == "Approved");
            var rejectedOffers = _context.Offers.Count(o => o.Status.ToString() == "Rejected");

            return Json(new
            {
                all = allOffers,
                pending = pendingOffers,
                approved = approvedOffers,
                rejected = rejectedOffers
            });
        }

        // ✅ Get Offers by Status (for real-time table updates)
        [HttpGet]
        public async Task<IActionResult> GetOffersByStatus(string status)
        {
            var query = _context.Offers.AsQueryable();

            if (status != "All")
            {
                if (Enum.TryParse(status, out OfferStatus offerStatus))
                    query = query.Where(o => o.Status == offerStatus);
            }

            var offers = await query
                .Select(o => new
                {
                    o.OfferId,
                    o.OfferName,
                    o.Description,
                    o.StartDate,
                    o.EndDate,
                    o.Cost,
                    o.Status,
                    o.PartnerEmail
                })
                .ToListAsync();

            return Json(offers);
        }
        public IActionResult OffersList1()
        {
            var offers = _context.Offers.ToList();

            // Debugging: Log OfferNames
            foreach (var offer in offers)
            {
                Console.WriteLine($"Offer ID: {offer.OfferId}, Name: {offer.OfferName}");
            }

            return View(offers);
        }

    }
}
