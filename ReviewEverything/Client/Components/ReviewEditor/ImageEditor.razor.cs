using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Localization;
using ReviewEverything.Shared.Contracts.Requests;
using ReviewEverything.Shared.Models;

namespace ReviewEverything.Client.Components.ReviewEditor
{
    public partial class ImageEditor
    {
        [Parameter] public List<CloudImageRequest> CloudImages { get; set; } = default!;
        [Inject] private IStringLocalizer<Pages.ReviewEditor> Localizer { get; set; } = default!;

        private static readonly string _defaultDragClass = "relative rounded-lg border-2 border-dashed pa-4 mud-width-full mud-height-full z-10";
        private string _dragClass = _defaultDragClass;
        
        private async Task OnInputFileChanged(InputFileChangeEventArgs e)
        {
            ClearDragClass();
            var files = e.GetMultipleFiles();
            foreach (var file in files)
            {
                var buffers = new byte[file.Size];
                await file.OpenReadStream(20971520L).ReadAsync(buffers);

                FileData fileData = new()
                {
                    FileName = file.Name,
                    Data = buffers
                };

                var httpResponseMessage = await HttpClient.PostAsJsonAsync("api/CloudImage", fileData);
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    CloudImages.Add(new CloudImageRequest()
                    {
                        Title = file.Name,
                        Url = await httpResponseMessage.Content.ReadAsStringAsync()
                    });
                }

                StateHasChanged();
            }
        }
        private void ClearDragClass()
        {
            _dragClass = _defaultDragClass;
        }

        private void RemoveCloudImage(CloudImageRequest cloudImage)
        {
            CloudImages.Remove(cloudImage);
        }

        private void SetDragClass()
        {
            _dragClass = $"{_defaultDragClass} mud-border-primary";
        }
    }
}
