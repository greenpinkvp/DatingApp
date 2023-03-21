namespace DatingApp.API.DTOs
{
    public class PhotoForApprovalDto
    {
        public Guid Id { get; set; }
        public string Url { get; set; }
        public string UserName { get; set; }
        public bool IsApproved { get; set; }
    }
}
