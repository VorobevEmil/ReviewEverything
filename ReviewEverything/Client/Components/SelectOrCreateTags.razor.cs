using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using ReviewEverything.Shared.Contracts.Requests;
using ReviewEverything.Shared.Contracts.Responses;

namespace ReviewEverything.Client.Components
{
    public partial class SelectOrCreateTags
    {
        [Inject] private ISnackbar Snackbar { get; set; } = default!;
        [Parameter] public ReviewRequest Review { get; set; } = default!;
        private TagRequest _tag = new();

        private async Task<IEnumerable<TagResponse>> SearchTagsAsync(string search)
        {
            return (await HttpClient.GetFromJsonAsync<List<TagResponse>>($"api/Tag{(!string.IsNullOrWhiteSpace(search) ? $"?search={search}" : null)}"))!;
        }

        private void AddTagInTagsList(TagResponse tag)
        {
            if (!Review.Tags.Any(x => x.Id == tag.Id))
                Review.Tags.Add(tag);
        }

        private void RemoveTagFromList(TagResponse tag)
        {
            Review.Tags.Remove(tag);
        }

        private async Task CreateTagAsync()
        {
            if (string.IsNullOrWhiteSpace(_tag.Title))
            {
                Snackbar.Add("Введите название тега", Severity.Error);
                return;
            }

            var httpResponseMessage = await HttpClient.PostAsJsonAsync("api/Tag", _tag);
            if (httpResponseMessage.StatusCode == HttpStatusCode.Created)
            {
                _tag = new();
                AddTagInTagsList((await httpResponseMessage.Content.ReadFromJsonAsync<TagResponse>())!);
            }
            else
            {
                Snackbar.Add("Не удалось создать тег", Severity.Error);
            }

        }
    }
}
