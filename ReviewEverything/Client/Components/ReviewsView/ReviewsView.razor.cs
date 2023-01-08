using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;
using ReviewEverything.Client.Helpers;
using ReviewEverything.Client.Services;
using ReviewEverything.Shared.Contracts.Responses;

namespace ReviewEverything.Client.Components.ReviewsView
{
    public partial class ReviewsView
    {
        [Inject] private DisplayHelper DisplayHelper { get; set; } = default!;
        [Inject] private BrowserService BrowserService { get; set; } = default!;
        [Inject] private IStringLocalizer<ReviewsView> Localizer { get; set; } = default!;
        [Parameter] public bool Editor { get; set; }
        [Parameter] public string? UserId { get; set; }
        public List<ReviewResponse> Reviews { get; set; } = default!;
        private TagsComponent _tags = default!;
        private CategoriesComponent _categories = default!;
        private FilterOptionView _filterOption = default!;

        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private int _page = 1;
        private int _pageSize = 10;
        private bool _loadingReviews = false;
        private bool _endReviews = false;

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
            StateHasChanged();
            
            var httpResponseMessage = await HttpClient.GetAsync($"api/Review?{GetParametersForReviewRequest()}", _cancellationTokenSource.Token);
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

        private string GetParametersForReviewRequest()
        {
            var page = $"page={_page}&";
            var pageSize = $"pageSize={_pageSize}&";
            var filter = _filterOption.GetFilterParameterUrl();
            var category = _categories.GetCategoryParameterUrl();
            var userId = UserId != null ? $"userId={UserId}&" : null;
            var tags = _tags.GetSelectedTags();

            return page + pageSize + filter + category + userId + tags;
        }

        private void CancelCancellationToken()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = new CancellationTokenSource();
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