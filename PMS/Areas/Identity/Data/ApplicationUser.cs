using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;

namespace PMS.Areas.Identity.Data;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }

    [Required] // Ensure it is required in the database
    public string PAN { get; set; } = string.Empty;

    [Required] // Ensure it is required in the database
    public string Aadhar { get; set; } = string.Empty;

    public bool IsVerified { get; set; } = false; // Admin approval status

    public bool IsAdmin { get; set; } = false; // Single admin flag
    public string? RejectionMessage { get; set; } // Stores rejection reason
     // ✅ Navigation property: A partner can create multiple offers

}
