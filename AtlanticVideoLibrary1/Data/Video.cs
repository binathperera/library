namespace AtlanticVideoLibrary1.Data
{
    public class Video
    {
        public String id { get; set; } = "";
        public String name { get; set; } = "";
        public String dateOfCreation { get; set; } = "";
        public String author { get; set; } = "";
        public Guid? lendingId { get; set; }
        public bool isReturned { get; set; }
    }
}
