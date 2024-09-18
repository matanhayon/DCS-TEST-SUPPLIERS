namespace DCS_TEST_SUPPLIERS.Models
{
    public class Hotel : Supplier
    {
        public string ChainName { get; set; }

        public Hotel()
        {
            CreateDate = DateTime.Now;
        }
    }

}
