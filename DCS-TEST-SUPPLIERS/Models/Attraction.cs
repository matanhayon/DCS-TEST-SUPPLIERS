using System;
using System.ComponentModel.DataAnnotations;

namespace DCS_TEST_SUPPLIERS.Models
{
    public class Attraction : Supplier
    {
        [Required(ErrorMessage = "Maximum number of tickets allowed is required.")]
        [Range(1, 100000, ErrorMessage = "Max tickets allowed must be between 1 and 100,000.")]
        public int MaxTicketsAllowed { get; set; }
    }
}