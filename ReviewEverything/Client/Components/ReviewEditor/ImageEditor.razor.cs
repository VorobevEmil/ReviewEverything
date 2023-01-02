using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using MudBlazor.Services;
using ReviewEverything.Shared.Contracts.Requests;
using ReviewEverything.Shared.Models;

namespace ReviewEverything.Client.Components.ReviewEditor
{
    public partial class ImageEditor
    {
        [Inject] private IBreakpointService BreakpointListener { get; set; } = default!;
        [Parameter] public List<CloudImageRequest> CloudImages { get; set; } = default!; 

        private Breakpoint _breakpoint = default!;
        private static readonly string _defaultDragClass = "relative rounded-lg border-2 border-dashed pa-4 mud-width-full mud-height-full z-10";
        private string _dragClass = _defaultDragClass;
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
