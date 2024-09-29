using System;
using System.ComponentModel.DataAnnotations;

namespace DCS_TEST_SUPPLIERS.Models
{
    public class Hotel : Supplier
    {
        [Required(ErrorMessage = "Chain name is required.")]
        [StringLength(100, ErrorMessage = "Chain name cannot exceed 100 characters.")]
        public string ChainName { get; set; }
    }
}