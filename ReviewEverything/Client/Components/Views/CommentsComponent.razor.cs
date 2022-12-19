using System.Net.Http.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;
using ReviewEverything.Shared.Contracts.Requests;
using ReviewEverything.Shared.Contracts.Responses;

namespace ReviewEverything.Client.Components.Views
{
    public partial class CommentsComponent
    {
        [Parameter] public int Id { get; set; }
        [Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;
        private ClaimsPrincipal User { get; set; } = default!;

        [Inject] private ISnackbar Snackbar { get; set; } = default!;
        private HubConnection _hubConnection = default!;

        private List<CommentResponse> Comments { get; set; } = default!;

        private int _pageNumber = 1;
        private int _pageSize = 10;
        private int _elementSkip = 0;
        private bool _hiddenButtonLoadMore = false;

        private string _bodyComment = default!;

        protected override async Task OnInitializedAsync()
        {

            Comments = new List<CommentResponse>();
            await LoadMoreCommentAsync();
            User = (await AuthenticationStateProvider.GetAuthenticationStateAsync()).User;
            await ConfigureHubConnectionAsync();
        }

        private async Task LoadMoreCommentAsync()
        {
            var comments = await GetCommentsAsync();

            if (comments.Count < _pageSize)
                _hiddenButtonLoadMore = true;

            Comments.AddRange(comments);
        }

        private async Task<List<CommentResponse>> GetCommentsAsync()
        {
            var httpResponseMessage = await HttpClient.GetAsync($"api/Comment/GetByReviewId/{Id}?pageNumber={_pageNumber}&pageSize={_pageSize}&elementSkip={_elementSkip}");
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                _pageNumber++;
                return (await httpResponseMessage.Content.ReadFromJsonAsync<List<CommentResponse>>())!;
            }

            Snackbar.Add("Не удалось загрузить комментарии", Severity.Error);
            return new List<CommentResponse>();
        }

        private async Task ConfigureHubConnectionAsync()
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(NavigationManager.ToAbsoluteUri("/commentHub"))
                .Build();

            _hubConnection.On<CommentResponse>("ReceiveComment", (comment) =>
            {
                _elementSkip++;
                Comments.Insert(0, comment);
                StateHasChanged();
            });

            await _hubConnection.StartAsync();

            await _hubConnection.SendAsync("EnterToArticle", Id);

        }

        private async Task SendCommentAsync()
        {
            if (User.Identity!.IsAuthenticated)
            {
                CommentRequest newComment = new() { Body = _bodyComment, ReviewId = Id };
                var httpResponseMessage = await HttpClient.PostAsJsonAsync("api/Comment", newComment);
                if (!httpResponseMessage.IsSuccessStatusCode)
                {
                    Snackbar.Add("Не удалось отправить комментарии", Severity.Error);
                }

                _bodyComment = default!;
            }
        }
    }
}