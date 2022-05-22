namespace POS_Api.Model
{
    public class SectionModel
    {
        public string UId { get; set; }
        public string Description { get; set; }
        public string SecondDescription { get; set; }
        public bool ApplyToUI { get; set; }
        public string AddedDateTime { get; set; }
        public string UpdatedDateTime { get; set; }
        public string AddedBy { get; set; }
        public string UpdatedBy { get; set; }
        public string LocationUId { get; set; }

        public bool IsError { get; set; }
        public string Error { get; set; }
    }
}
