namespace TrackIt.WebAPI.Model
{
    public class AddRatingAndCommentRequest
    {
        public Guid ClientId { get; set; }
        public Guid PackageId { get; set; }
        public int RatingNumber { get; set; }
        public string Comment { get; set; }
    }
}
