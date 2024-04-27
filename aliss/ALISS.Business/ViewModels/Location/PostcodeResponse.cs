namespace ALISS.Business.ViewModels.Location
{
    public class PostcodeResponse
    {
        public int status { get; set; }
        public string error { get; set; }
        public PostcodeResult result { get; set; }
    }

    public class PostcodeResult
    {
        public string postcode { get; set; }
        public double longitude { get; set; }
        public double latitude { get; set; }
    }
}
