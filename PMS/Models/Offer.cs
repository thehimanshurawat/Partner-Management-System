using PMS.Areas.Identity.Data;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PMS.Models
{
    public enum OfferStatus
    {
        Pending,
        Approved,
        Rejected
    }

    public class Offer
    {
        [Key]
        public int OfferId { get; set; }

        [Required]
        [StringLength(100)]
        public string? OfferName { get; set; }

        [Required]
        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [Required]
        public string? PartnerEmail { get; set; }

        public string? PartnerId { get; set; }

        [ForeignKey("PartnerId")]
        public ApplicationUser? Partner { get; set; }

        public OfferStatus Status { get; set; } = OfferStatus.Pending; // Default to Pending

        public decimal Cost { get; set; } // New column for cost

        public string? Message { get; set; } // Reason for rejection (Optional)
    }

}
