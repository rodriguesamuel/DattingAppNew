namespace API.DTOs
{
    public class PhotoForApprovalDto
    {
        public int PhotoId { get; set; }
        public string PhotoUrl { get; set; }
        public string Username { get; set; }
        public bool IsApproved { get; set; }
    }
}