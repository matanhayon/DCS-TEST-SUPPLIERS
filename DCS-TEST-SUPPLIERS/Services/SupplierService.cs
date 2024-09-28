using DCS_TEST_SUPPLIERS.Models;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DCS_TEST_SUPPLIERS.Services
{
    public class SupplierService
    {
        private readonly ILogger<SupplierService> _logger;

        public SupplierService(ILogger<SupplierService> logger)
        {
            _logger = logger;
        }

        public async Task SendSuppliersToWebServiceAsync(string candidateFullName, List<SupplierDetails> supplierDetailsList)
        {
            var xmlContent = ConvertToXmlContent(candidateFullName, supplierDetailsList);
            _logger.LogInformation("Generated XML content: {xmlContent}", xmlContent);

            var url = "http://web27.agency2000.co.il/Test/TestService.asmx";
            await PostXmlDataAsync(url, xmlContent);
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
                    var content = new StringContent(xmlContent, Encoding.UTF8, "application/soap+xml");
                    System.Console.WriteLine("Post Content: " + content);
                    var response = await httpClient.PostAsync(url, content);
                    System.Console.WriteLine("Response Content: " + response);

                    if (response.IsSuccessStatusCode)
                    {
                        _logger.LogInformation("Data successfully sent. Status code: {statusCode}", response.StatusCode);
                    }
                    else
                    {
                        _logger.LogError("Error sending data. Status code: {statusCode}", response.StatusCode);
                        var errorResponse = await response.Content.ReadAsStringAsync();
                        _logger.LogError("Error details: {errorResponse}", errorResponse);
                    }

                    response.EnsureSuccessStatusCode();
                }
                catch (HttpRequestException ex)
                {
                    _logger.LogError("Error occurred while sending data: {message}", ex.Message);
                    throw new ApplicationException("There was an error sending the supplier details. Please try again later.");
                }
            }
        }
    }
}
