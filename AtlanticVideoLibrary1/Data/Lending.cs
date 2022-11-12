namespace AtlanticVideoLibrary1.Data
{
    public class Lending
    {
        public String id { get; set; } = "";
        public String memberId { get; set; } = "";
        public String borrowedDate { get; set; } = "";
        public String returnDate { get; set; } = "";     
        public LendingDetails details { get; set; }  = new LendingDetails();
    }
}
