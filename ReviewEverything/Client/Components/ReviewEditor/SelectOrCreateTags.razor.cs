using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;
using ReviewEverything.Shared.Contracts.Requests;
using ReviewEverything.Shared.Contracts.Responses;

namespace ReviewEverything.Client.Components.ReviewEditor
{
    public partial class SelectOrCreateTags
    {
        [Inject] private IStringLocalizer<Pages.ReviewEditor> Localizer { get; set; } = default!;
        [Inject] private ISnackbar Snackbar { get; set; } = default!;
        [Parameter] public ReviewRequest Review { get; set; } = default!;
        private TagRequest _tag = new();
        public List<TagRequest> CreateTags { get; } = new();

        private async Task<IEnumerable<TagResponse>> SearchTagsAsync(string search)
        {
            return (await HttpClient.GetFromJsonAsync<List<TagResponse>>($"api/Tag?page=1&pageSize=20{(!string.IsNullOrWhiteSpace(search) ? $"&search={search}" : null)}"))!;
        }

        private void AddTagInTagsList(TagResponse tag)
        {
            if (Review.Tags.All(x => x.Id != tag.Id) || (tag.Id == 0 && Review.Tags.All(x => x.Title.ToLower() != tag.Title.ToLower())))
                Review.Tags.Add(tag);
            else
                Snackbar.Add("Данный тег уже добавлен в список", Severity.Warning);

        }

        private void RemoveTagFromList(TagResponse tag)
        {
            Review.Tags.Remove(tag);
            CreateTags.RemoveAll(x => x.Title == tag.Title);
        }

        private async Task CreateTagAsync()
        {
            if (string.IsNullOrWhiteSpace(_tag.Title))
            {
                Snackbar.Add("Введите название тега", Severity.Error);
                return;
            }

            var httpResponseMessage = await HttpClient.GetAsync($"api/Tag/ExistByName/{_tag.Title}");

            if (httpResponseMessage.StatusCode == HttpStatusCode.NotFound)
            {
                CreateTags.Add(_tag);
                AddTagInTagsList(new TagResponse() { Title = _tag.Title });
                _tag = new();
            }
            else
            {
                Snackbar.Add(await httpResponseMessage.Content.ReadAsStringAsync(), Severity.Error);
            }
        }
    }
}
