namespace AtlanticVideoLibrary1.Data
{
    public interface ILendingService
    {
        List<Lending> GetLendings();
        LendingDetails GetLendingDetails(String id);
        List<Lending> Search(String s);
        Lending GetLending(String id);
        bool Update(Lending l);
        bool Add(Lending l);
        bool Delete(String id);

        bool AddVideo(String lendingId, String videoId);
        bool DeleteVideo(String lendingId, String videoId);    
        Video GetVideo(String id);
        bool IsMemberExist(String id);

        int SetReturnStatus(String lendingId, String videoId, bool stat);
        /*Return value definition
         0 = Successfully updated return status
         1 = Failed to update return status due to system error
         2 = Failed to update return status as Video was taken by someone else
         */


        bool GetReturnStatus(String lendingId,String videoId);
    }
}
