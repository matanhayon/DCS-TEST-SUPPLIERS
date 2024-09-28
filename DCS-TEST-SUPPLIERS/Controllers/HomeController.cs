using DCS_TEST_SUPPLIERS.Models;
using DCS_TEST_SUPPLIERS.Data;
using DCS_TEST_SUPPLIERS.Services;  // Import the SupplierService
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace DCS_TEST_SUPPLIERS.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly SupplierService _supplierService;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, SupplierService supplierService)
        {
            _logger = logger;
            _context = context;
            _supplierService = supplierService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public async Task<IActionResult> AllSuppliers()
        {
            var allSuppliers = await GetSuppliersAsync();
            return View(allSuppliers);
        }

        public async Task<IActionResult> SendAllSuppliers()
        {
            try
            {
                var suppliers = await GetSuppliersAsync();

                var supplierDetailsList = suppliers.Select(s => new SupplierDetails
                {
                    Id = s.Id,
                    Name = s.Name,
                    ManagerName = s.ManagerName,
                    ManagerPhoneNumber = s.ManagerPhoneNumber,
                    CreateDate = s.CreateDate.ToString("yyyy-MM-dd"),
                    SupplierType = s.SupplierType,
                    ExtraDetails = s is Hotel hotel ? $"Chain Name: {hotel.ChainName}" :
                                  s is Flight flight ? $"Carrier Name: {flight.CarrierName}" :
                                  s is Attraction attraction ? $"Max Tickets Allowed: {attraction.MaxTicketsAllowed}" : ""
                }).ToList();

                await _supplierService.SendSuppliersToWebServiceAsync("Matan Hayon", supplierDetailsList);

                TempData["Message"] = "Supplier details have been sent successfully!";
            }
            catch (ApplicationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction("AllSuppliers");
        }

        private async Task<List<Supplier>> GetSuppliersAsync()
        {
            var hotelSuppliers = await _context.Hotel.ToListAsync();
            var flightSuppliers = await _context.Flight.ToListAsync();
            var attractionSuppliers = await _context.Attraction.ToListAsync();

            var allSuppliers = hotelSuppliers.Cast<Supplier>()
                .Concat(flightSuppliers.Cast<Supplier>())
                .Concat(attractionSuppliers.Cast<Supplier>())
                .ToList();

            return allSuppliers;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
