using System;
using System.ComponentModel.DataAnnotations;

namespace DCS_TEST_SUPPLIERS.Models
{
    public class Supplier
    {
        [Required(ErrorMessage = "Supplier ID is required.")]
        [StringLength(50, ErrorMessage = "Supplier ID cannot exceed 50 characters.")]
        public string Id { get; set; }

        [Required(ErrorMessage = "Supplier name is required.")]
        [StringLength(100, ErrorMessage = "Supplier name cannot exceed 100 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Manager name is required.")]
        [StringLength(100, ErrorMessage = "Manager name cannot exceed 100 characters.")]
        public string ManagerName { get; set; }

        [Required(ErrorMessage = "Manager phone number is required.")]
        [RegularExpression(@"^(\+?\d{1,4}[\s.-]?)?(\(?\d{3}\)?[\s.-]?)?[\d\s.-]{7,10}$",
            ErrorMessage = "Invalid phone number format. Only up to 11 digits are allowed.")]
        [DataType(DataType.PhoneNumber, ErrorMessage = "Invalid phone number.")]
        public string ManagerPhoneNumber { get; set; }

        [Required(ErrorMessage = "Creation date is required.")]
        [DataType(DataType.Date, ErrorMessage = "Invalid date format.")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime CreateDate { get; set; }

        [Required(ErrorMessage = "Supplier type is required.")]
        [StringLength(50, ErrorMessage = "Supplier type cannot exceed 50 characters.")]
        public string SupplierType { get; set; }
    }
}