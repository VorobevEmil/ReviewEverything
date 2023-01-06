using System.Linq;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Localization;
using MudBlazor;
using ReviewEverything.Shared.Contracts.Requests;
using ReviewEverything.Shared.Models;

namespace ReviewEverything.Client.Components.ReviewEditor
{
    public partial class ImageEditor
    {
        [Parameter] public List<CloudImageRequest> CloudImages { get; set; } = default!;
        [Inject] private ISnackbar Snackbar { get; set; } = default!;
        [Inject] private IStringLocalizer<Pages.ReviewEditor> Localizer { get; set; } = default!;

        private Dictionary<CloudImageRequest, CancellationTokenSource> SendImagesDictionary { get; set; } = new();
        private static readonly string DefaultDragClass = "relative rounded-lg border-2 border-dashed pa-4 mud-width-full mud-height-full z-10";
        private string _dragClass = DefaultDragClass;

        private async Task OnInputFileChangedAsync(InputFileChangeEventArgs e)
        {
            ClearDragClass();

            List<Task> uploadTasks = new List<Task>();
            var files = e.GetMultipleFiles();
            foreach (var file in files)
            {
                if (CheckFileContentTypeContainsImage(file))
                {
                    Snackbar.Add($"Добавляемый файл {file.Name} не является изображением", Severity.Warning);
                    continue;
                }

                if (CheckImageContainsInCloudImages(file.Name))
                {
                    Snackbar.Add($"Добавляемое изображение {file.Name} уже содержится в списке", Severity.Warning);
                    continue;
                }

                uploadTasks.Add(UploadImageAsync(file));
            }

            await Task.WhenAll(uploadTasks);
        }

        private bool CheckFileContentTypeContainsImage(IBrowserFile file)
            => file.ContentType.Contains("Image");

        private bool CheckImageContainsInCloudImages(string fileName)
            => CloudImages.Any(x => x.Title == fileName);

        private async Task UploadImageAsync(IBrowserFile file)
        {
            var fileData = await ReadFileAsync(file);
            if (fileData is not null)
                await SendImageOnCloudAsync(fileData);
        }

        private async Task<FileData?> ReadFileAsync(IBrowserFile file)
        {
            var maxAllowedSize = await HttpClient.GetFromJsonAsync<int>("api/CloudImage/GetMaxAllowedSize");
            if (file.Size > maxAllowedSize)
            {
                Snackbar.Add($"У изображения {file.Name} превышен максимальный размер. Максимальный размер файла составляет {maxAllowedSize / 1024 / 1024} МБ", Severity.Warning);
                return null;
            }

            var buffers = new byte[file.Size];

            var readAsync = await file.OpenReadStream(maxAllowedSize).ReadAsync(buffers);
            return new()
            {
                FileName = file.Name,
                Data = buffers,
                ContentType = file.ContentType
            };
        }

        private async Task SendImageOnCloudAsync(FileData fileData)
        {

            var cloudImageRequest = AddCloudImageRequestInCloudImages(fileData);
            StateHasChanged();
            var token = CreateCancellationToken(cloudImageRequest);
            var httpResponseMessage = await HttpClient.PostAsJsonAsync("api/CloudImage", fileData, cancellationToken: token);
            if (!token.IsCancellationRequested)
            {
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    cloudImageRequest.Url = await httpResponseMessage.Content.ReadAsStringAsync(token);
                }
                else
                {
                    CloudImages.Remove(cloudImageRequest);
                    Snackbar.Add(await httpResponseMessage.Content.ReadAsStringAsync(token), Severity.Error);
                }
                StateHasChanged();
            }
        }

        private CloudImageRequest AddCloudImageRequestInCloudImages(FileData fileData)
        {
            var cloudImageRequest = new CloudImageRequest()
            {
                Title = fileData.FileName,
            };
            CloudImages.Add(cloudImageRequest);
            return cloudImageRequest;
        }

        private CancellationToken CreateCancellationToken(CloudImageRequest cloudImageRequest)
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            SendImagesDictionary[cloudImageRequest] = cancellationTokenSource;
            return cancellationTokenSource.Token;
        }

        private void ClearDragClass()
        {
            _dragClass = DefaultDragClass;
        }
        private void SetDragClass()
        {
            _dragClass = $"{DefaultDragClass} mud-border-primary";
        }

        private void RemoveCloudImage(CloudImageRequest cloudImage)
        {
            if (SendImagesDictionary.ContainsKey(cloudImage))
            {
                SendImagesDictionary[cloudImage].Cancel();
                SendImagesDictionary.Remove(cloudImage);
            }

            CloudImages.Remove(cloudImage);
            StateHasChanged();
        }

    }
}
