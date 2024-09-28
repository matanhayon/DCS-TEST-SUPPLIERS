using DCS_TEST_SUPPLIERS.Models;
using DCS_TEST_SUPPLIERS.Data; 
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; 
using System.Diagnostics;
using System.Xml.Linq;
using System.Text;
using System.Xml.Serialization;


namespace DCS_TEST_SUPPLIERS.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
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
                await SendSuppliersToWebService("Matan Hayon");
                TempData["Message"] = "Supplier details have been sent successfully!";
            }
            catch (ApplicationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction("AllSuppliers");
        }


        public async Task SendSuppliersToWebService(string candidateFullName)
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

            var xmlContent = ConvertToXmlContent(candidateFullName, supplierDetailsList);
            Console.WriteLine(xmlContent);
            await PostXmlDataAsync("http://web27.agency2000.co.il/Test/TestService.asmx", xmlContent);
        }


        private string ConvertToXmlContent(string candidateFullName, List<SupplierDetails> supplierDetailsList)
        {
            var sb = new StringBuilder();

            sb.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            sb.Append("<soap12:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" ");
            sb.Append("xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" ");
            sb.Append("xmlns:soap12=\"http://www.w3.org/2003/05/soap-envelope\">");
            sb.Append("<soap12:Body>");

            sb.Append("<GetSuppliersDetails xmlns=\"http://tempuri.org/\">");
            sb.AppendFormat("<CandidateFullName>{0}</CandidateFullName>", candidateFullName);

            foreach (var sd in supplierDetailsList)
            {
                sb.Append("<SupplierDetails>");
                sb.AppendFormat("<Id>{0}</Id>", sd.Id);
                sb.AppendFormat("<Name>{0}</Name>", sd.Name);
                sb.AppendFormat("<ManagerName>{0}</ManagerName>", sd.ManagerName);
                sb.AppendFormat("<ManagerPhoneNumber>{0}</ManagerPhoneNumber>", sd.ManagerPhoneNumber);
                sb.AppendFormat("<CreateDate>{0}</CreateDate>", sd.CreateDate);
                sb.AppendFormat("<SupplierType>{0}</SupplierType>", sd.SupplierType);
                sb.AppendFormat("<ExtraDetails>{0}</ExtraDetails>", sd.ExtraDetails);
                sb.Append("</SupplierDetails>");
            }

            sb.Append("</GetSuppliersDetails>");
            sb.Append("</soap12:Body>");
            sb.Append("</soap12:Envelope>");

            return sb.ToString();
        }

        private async Task PostXmlDataAsync(string url, string xmlContent)
        {
            using (var httpClient = new HttpClient())
            {
                try
                {
                    var content = new StringContent(xmlContent, Encoding.UTF8, "application/soap+xml");  // Use 'application/soap+xml' for SOAP 1.2
                    Console.WriteLine("XML content: " + xmlContent);

                    var response = await httpClient.PostAsync(url, content);

                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Success: " + response.StatusCode);
                    }
                    else
                    {
                        Console.WriteLine("Error: " + response.StatusCode);
                        var errorResponse = await response.Content.ReadAsStringAsync();
                        Console.WriteLine("Error details: " + errorResponse);
                    }

                    response.EnsureSuccessStatusCode();
                }
                catch (HttpRequestException ex)
                {
                    _logger.LogError($"Error occurred while sending data: {ex.Message}");

                    throw new ApplicationException("There was an error sending the supplier details. Please try again later.");
                }
            }
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
