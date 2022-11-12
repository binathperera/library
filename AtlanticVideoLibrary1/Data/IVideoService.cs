namespace AtlanticVideoLibrary1.Data
{
    public interface IVideoService
    {

        List<Video> GetVideos();
        List<Video> Search(String s);
        Video GetVideo(String id);
        bool Update(Video m);
        bool Add(Video m);
        bool Delete(String id);
        String GenerateId();
    }
}
