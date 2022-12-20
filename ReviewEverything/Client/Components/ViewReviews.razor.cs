using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Services;
using ReviewEverything.Client.Components.Views;
using ReviewEverything.Client.Helpers;
using ReviewEverything.Client.Services;
using ReviewEverything.Shared.Contracts.Responses;

namespace ReviewEverything.Client.Components
{
    public partial class ViewReviews
    {
        [Inject] private DisplayHelper DisplayHelper { get; set; } = default!;
        [Inject] private IBreakpointService BreakpointListener { get; set; } = default!;
        [Inject] private BrowserService BrowserService { get; set; } = default!;
        [Parameter] public bool Editor { get; set; }
        [Parameter] public string? UserId { get; set; }
        public List<ReviewResponse> Reviews { get; set; } = default!;
        private TagsComponent _tags = default!;

        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private Breakpoint _breakpoint = default!;
        private int? _categoryId = default!;
        private int _page = 1;
        private int _pageSize = 10;
        private bool _loadingReviews = false;
        private bool _endReviews = false;


        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                var subscriptionResult = await BreakpointListener.Subscribe((breakpoint) =>
                {
                    _breakpoint = breakpoint;
                    InvokeAsync(StateHasChanged);
                }, new ResizeOptions
                {
                    NotifyOnBreakpointOnly = true,
                });

                _breakpoint = subscriptionResult.Breakpoint;
                StateHasChanged();
            }
        }

        private async Task InitializationReviewsAsync()
        {
            CancelCancellationToken();

            Reviews = null!;
            _page = 1;
            _endReviews = false;

            Reviews = await GetReviewsFromApiAsync();
        }


        private async Task PaginationReviewsAsync(ScrollEventArgs e)
        {
            if (_loadingReviews || _endReviews)
                return;

            var browserDimension = await BrowserService.GetDimensions();
            if (e.FirstChildBoundingClientRect.AbsoluteBottom < browserDimension.Height + 50)
                Reviews.AddRange(await GetReviewsFromApiAsync());
        }

        private async Task<List<ReviewResponse>> GetReviewsFromApiAsync()
        {
            _loadingReviews = true;
            var category = _categoryId != null ? $"categoryId={_categoryId}&" : null;
            var userId = UserId != null ? $"userId={UserId}&" : null;
            var tags = _tags.GetSelectedTags();

            var httpResponseMessage = await HttpClient.GetAsync($"api/Review?page={_page}&pageSize={_pageSize}&{category}{userId}{tags}", _cancellationTokenSource.Token);
            _loadingReviews = false;
            if (!_cancellationTokenSource.Token.IsCancellationRequested && httpResponseMessage.IsSuccessStatusCode)
            {
                _page++;
                var reviews = (await httpResponseMessage.Content.ReadFromJsonAsync<List<ReviewResponse>>())!;
                if (reviews.Count < _pageSize)
                    _endReviews = true;

                return reviews;
            }

            return new();
        }

        private void CancelCancellationToken()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = new CancellationTokenSource();
        }

        private async Task GetReviewsFromCategoryId(int? categoryId)
        {
            _categoryId = categoryId;
            await InitializationReviewsAsync();
        }

        private void NavigateToReviewEditor(int? reviewId = null)
        {
            NavigationManager.NavigateTo($"review-editor/{reviewId}");
        }

        private async Task DeleteReviewAsync(int reviewId)
        {
            if (await DisplayHelper.ShowDeleteMessageBoxAsync() != true)
                return;

            await HttpClient.DeleteAsync($"api/Review/{reviewId}");
            Reviews.RemoveAll(x => x.Id == reviewId);
        }
    }
}