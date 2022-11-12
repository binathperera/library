using AtlanticVideoLibrary1.Data;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Data;

namespace AtlanticVideoLibrary1.Pages
{
    public partial class Lendings
    {
        private List<AtlanticVideoLibrary1.Data.Lending> lendings;
        private MudBlazor.Severity level { get; set; } = MudBlazor.Severity.Normal;
        private string message { get; set; } = "";
        private MudBlazor.Severity videoLevel { get; set; } = MudBlazor.Severity.Normal;
        private string videoMessage { get; set; } = "";
        private AtlanticVideoLibrary1.Data.Lending lending = new AtlanticVideoLibrary1.Data.Lending();
        private String lendingSearchValue = "";
        private String videoSearchValue = "";
        private Data.Video video;
        private decimal memberId=0;
        private DateTime? borrowedDate = DateTime.Today;
        private DateTime? returnDate = DateTime.Today;
        private List<Data.Video> videos= new List<Data.Video>();


      /*  #region Injection ReadOnlys
       private readonly ILendingService  _lendingService;
        private readonly IVideoService _videoService;
        private readonly NavigationManager _navigationManager;
        #endregion*/
        /*public Lendings(ILendingService lendingService, IVideoService videoService, NavigationManager navigationManager)
        {
            _lendingService= lendingService;
            _videoService= videoService;
            _navigationManager= navigationManager;
        }*/
        protected override void OnInitialized()
        {
            lendings = _lendingService.GetLendings();

        }

        public void SearchVideo()
        {
            foreach(Data.Video v in videos)
            {
                if (v.id == videoSearchValue) {
                    videoLevel = MudBlazor.Severity.Info;
                    videoMessage = "Video already added to table";
                    return;
                }
            }
            video = _lendingService.GetVideo(videoSearchValue);
            if (video == null)
            {
                videoLevel = MudBlazor.Severity.Error;
                videoMessage = "Video does not exist";
            }
            else if(video.lendingId != null)
            {
                videoLevel = MudBlazor.Severity.Error;
                videoMessage = "Video already borrowed. Lending id "+video.lendingId;
            }
            else
            {
                videoLevel = MudBlazor.Severity.Success;
                videoMessage = "Video added to table";
                videos.Add(video);
            }
        }
        public void Add()
        {
            lending.memberId = "" + memberId;
            lending.borrowedDate = borrowedDate.ToString();
            lending.returnDate = returnDate.ToString();
            if (!_lendingService.IsMemberExist(lending.memberId)) {
                level = MudBlazor.Severity.Error;
                message = "No member found for that id";
                return;
            }
            if (videos.Count() == 0) {
                level = MudBlazor.Severity.Error;
                message = "Add at least one video";
                return;
            }
            lending.details.videos = videos;
            bool status = _lendingService.Add(lending);
            if (status)
            {
                level = MudBlazor.Severity.Success;
                message = "Details added";
                lendings = _lendingService.GetLendings();
            }
            else
            {
                level = MudBlazor.Severity.Error;
                message = "Failed to Add Details ";
            }
        }
        public void SearchLendings()
        {
            lendings = _lendingService.Search(lendingSearchValue);
        }
        public void RemoveFromList(Data.Video video)
        {
            videos.Remove(video);
            _navigationManager.NavigateTo($"/lendings");
        }
    }
}
