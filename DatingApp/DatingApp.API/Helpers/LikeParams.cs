namespace DatingApp.API.Helpers
{
    public class LikeParams : PaginationParams
    {
        public Guid UserId { get; set; }
        public string Predicate { get; set; }
    }
}
