using System.Xml.Serialization;

namespace DCS_TEST_SUPPLIERS.Models
{
    public class SupplierDetails
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ManagerName { get; set; }
        public string ManagerPhoneNumber { get; set; }
        public string CreateDate { get; set; }
        public string SupplierType { get; set; }
        public string ExtraDetails { get; set; }
    }
}
