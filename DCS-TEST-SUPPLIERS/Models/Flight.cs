using System;
using System.ComponentModel.DataAnnotations;

namespace DCS_TEST_SUPPLIERS.Models
{
    public class Flight : Supplier
    {
        [Required(ErrorMessage = "Carrier name is required.")]
        [StringLength(100, ErrorMessage = "Carrier name cannot exceed 100 characters.")]
        public string CarrierName { get; set; }
    }
}