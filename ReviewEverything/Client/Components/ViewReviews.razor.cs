using System.Net.Http.Json;
using System.Threading;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Services;
using ReviewEverything.Client.Components.Views;
using ReviewEverything.Client.Helpers;
using ReviewEverything.Shared.Contracts.Responses;

namespace ReviewEverything.Client.Components
{
    public partial class ViewReviews
    {
        [Inject] private DisplayHelper DisplayHelper { get; set; } = default!;
        [Inject] private IBreakpointService BreakpointListener { get; set; } = default!;
        [Parameter] public bool Editor { get; set; }
        [Parameter] public string? UserId { get; set; }
        public List<ReviewResponse> Reviews { get; set; } = default!;
        private TagsComponent _tags = default!;

        private Breakpoint _breakpoint = default!;
        private int? _categoryId = default!;
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

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

        private async Task GetReviewsFromApiAsync()
        {
            CancelCancellationToken();

            Reviews = null!;

            var category = _categoryId != null ? $"categoryId={_categoryId}&" : null;
            var userId = UserId != null ? $"userId={UserId}&" : null;
            var tags = _tags.GetSelectedTags();

            var httpResponseMessage = await HttpClient.GetAsync($"api/Review?{category}{userId}{tags}", _cancellationTokenSource.Token);
            if (!_cancellationTokenSource.Token.IsCancellationRequested && httpResponseMessage.IsSuccessStatusCode)
            {
                Reviews = (await httpResponseMessage.Content.ReadFromJsonAsync<List<ReviewResponse>>())!;
            }
        }

        private void CancelCancellationToken()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = new CancellationTokenSource();

            //TODO после создания пагинации сделать нормальную отмену
            //if (_cancellationTokenSource.IsCancellationRequested)
            //{
            //    _cancellationTokenSource.Dispose();
            //    _cancellationTokenSource = new CancellationTokenSource();
            //}
        }

        private async Task GetReviewsFromCategoryId(int? categoryId)
        {
            _categoryId = categoryId;
            await GetReviewsFromApiAsync();
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