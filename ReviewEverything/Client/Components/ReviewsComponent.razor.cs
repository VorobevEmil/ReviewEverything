using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using ReviewEverything.Shared.Contracts.Responses;

namespace ReviewEverything.Client.Components
{
    public partial class ReviewsComponent
    {
        [Parameter] public bool Editor { get; set; }
        [Parameter] public List<ReviewResponse> Reviews { get; set; } = new List<ReviewResponse>()
        {
            new ReviewResponse()
            {
                Id = 1,
                Title = "Tommy мать его Shelby!",
                Subtitle = "Вы такого ещё не видели, он самый острый из всех козырьков",
                CloudImage = new CloudImageResponse()
                {
                    Url = "https://sun9-22.userapi.com/impg/XruCHrOaxqOtKzhlmd-k4LRu-MXbq8x9_k6jbw/dJisNJWwwZ0.jpg?size=1271x714&quality=96&sign=c86a1744a378dcebc671671969ef7f3c&type=album"
                },
                Composition = "Tommy Shelby",
                Author = "Джон Шелби",
                AuthorId = Guid.NewGuid().ToString(),
                CompositionId = 1,
                AuthorScore = 10,
                CommentCount = 30,
                LikeUsers = 100,
                CreationDate = DateTime.UtcNow
            },
            new ReviewResponse()
            {
                Id = 1,
                Title = "Tommy Shelby 2",
                Subtitle = "Вы такого ещё не видели",
                CloudImage = new CloudImageResponse()
                {
                    Url = "https://sun9-81.userapi.com/impg/SCRM6JtSlApwazwsE7VoOE7E6tFg5GlmMVMJZw/CejAXCIm258.jpg?size=1072x711&quality=96&sign=2ba6ca2aa0d5aaef9fe3ad933cbcc15f&type=album"
                },
                Composition = "Tommy Shelby",
                Author = "Артур Шелби",
                AuthorId = Guid.NewGuid().ToString(),
                CompositionId = 1,
                AuthorScore = 10,
                CommentCount = 30,
                LikeUsers = 100,
                CreationDate = DateTime.UtcNow
            },
            new ReviewResponse()
            {
                Id = 2,
                Title = "Томас мать его Шелби",
                Subtitle = "Такого Томаса Шелби вы ещё не видели",
                CloudImage = new CloudImageResponse()
                {
                    Url = "https://sun9-75.userapi.com/impg/nxiRwgXrFZblfv3cYqyNGnxVhbYdKVfXxf9qqA/cqc_SqAXTPo.jpg?size=715x714&quality=96&sign=d8e5262b3af3cdcfa09a3faaee5e83d7&type=album"
                },
                Composition = "Tommy Shelby",
                Author = "Томас Шелби",
                AuthorId = Guid.NewGuid().ToString(),
                CompositionId = 1,
                AuthorScore = 10,
                CommentCount = 30,
                LikeUsers = 100,
                CreationDate = DateTime.UtcNow
            },
            new ReviewResponse()
            {
                Id = 3,
                Title = "Томас Шелби и бебра",
                Subtitle = "Томас шелби мощно занюхнул беброчку",
                CloudImage = new CloudImageResponse()
                {
                    Url = "https://sun7-14.userapi.com/impg/AJTfPbV1fn9-CYpO-0SagM2HGSg9MNd3kMWMKw/xUaWZ45QAlM.jpg?size=714x711&quality=96&sign=9540d6bba726d4aee47ee35ef3398a70&type=album"
                },
                Composition = "Томас мать его шелби!",
                Author = "Томас Бебра младший",
                AuthorId = Guid.NewGuid().ToString(),
                CompositionId = 1,
                AuthorScore = 10,
                CommentCount = 30,
                LikeUsers = 100,
                CreationDate = DateTime.UtcNow
            },
        };
        [Parameter] public EventCallback<int?> GetReviewsFromApiAsync { get; set; }
        private List<CategoryResponse> Categories { get; set; } = default!;

        private string _titleCategory = default!;


        protected override async Task OnInitializedAsync()
        {
            Categories = (await HttpClient.GetFromJsonAsync<List<CategoryResponse>>("api/Category"))!;
            await SelectedCategoryAsync();
        }

        private async Task SelectedCategoryAsync(int? categoryId = null)
        {
            _titleCategory = categoryId == null ? "Все Обзоры" : $"Обзоры на {Categories.First(x => x.Id == categoryId.Value).Title}";

            await GetReviewsFromApiAsync.InvokeAsync(categoryId);
        }
    }
}