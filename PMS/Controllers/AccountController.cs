using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PMS.Areas.Identity.Data;
using System.Threading.Tasks;

namespace PMS.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }



        [Authorize]
        public async Task<IActionResult> DocumentVerification()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            // ✅ If verified, go to Partner Dashboard
            if (user.IsVerified)
                return RedirectToAction("Dashboard", "Partner");

            // ✅ If documents are submitted (even if pending or rejected), show status page
            if (!string.IsNullOrEmpty(user.PAN) && !string.IsNullOrEmpty(user.Aadhar))
                return RedirectToAction("WaitingForApproval");

            return View(user); // If not submitted yet, show document upload page
        }


        [HttpPost]
        [Authorize]
        public async Task<IActionResult> DocumentVerification(ApplicationUser model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            // Save document details for admin approval
            user.PAN = model.PAN;
            user.Aadhar = model.Aadhar;
            user.IsVerified = false; // Default: Wait for admin approval
            user.RejectionMessage = null; // Reset rejection message on resubmission

            await _userManager.UpdateAsync(user);

            TempData["Message"] = "Documents submitted for verification. Please wait for admin approval.";
            return RedirectToAction("WaitingForApproval");
        }


        public async Task<IActionResult> WaitingForApproval()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            // ✅ If verified, go to Dashboard
            if (user.IsVerified)
                return RedirectToAction("Dashboard", "Partner");

            // ✅ Show rejection or pending status in the same view
            ViewBag.RejectionMessage = user.RejectionMessage;
            return View(user);
        }
        public IActionResult RejectedApplications()
        {
            var rejectedUsers = _userManager.Users.Where(u => !u.IsVerified && !string.IsNullOrEmpty(u.RejectionMessage)).ToList();
            return View(rejectedUsers);
        }
        [HttpPost]
        public async Task<IActionResult> RestoreVerification(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                user.IsVerified = false; // Back to pending
                user.RejectionMessage = null; // Clear rejection message
                await _userManager.UpdateAsync(user);
            }

            return RedirectToAction("RejectedApplications");
        }
        [HttpGet]
        public IActionResult GetRejectedApplicationsCount()
        {
            var rejectedApplications = _userManager.Users.Count(u => !u.IsVerified && !string.IsNullOrEmpty(u.RejectionMessage));
            return Json(new { rejectedApplications });
        }
        [Authorize]
        public async Task<IActionResult> ResubmitDocuments()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            return View(user); // Show resubmission form
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ResubmitDocuments(ApplicationUser model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            // ✅ Allow resubmission of documents
            user.PAN = model.PAN;
            user.Aadhar = model.Aadhar;
            user.IsVerified = false; // Set back to pending
            user.RejectionMessage = null; // Clear rejection message

            await _userManager.UpdateAsync(user);

            TempData["Message"] = "Your documents have been resubmitted for verification.";
            return RedirectToAction("WaitingForApproval");
        }
    }
}