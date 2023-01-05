using System.ComponentModel.DataAnnotations;
using ReviewEverything.Shared.Contracts.Responses;
using ReviewEverything.Shared.Models.CustomValidationAttribute;

namespace ReviewEverything.Shared.Contracts.Requests
{
    public class ReviewRequest
    {
        [Required(ErrorMessage = "Введите название обзора")]
        public string Title { get; set; } = default!;
        public string Subtitle { get; set; } = default!;
        [Required(ErrorMessage = "Введите тело обзора")]
        public string Body { get; set; } = default!;
        [Required(ErrorMessage = "Выберите оценку произведению")]
        [Range(1, 10, ErrorMessage = "Диапазон допустимых значении доступен от 1 до 10")]
        public int? AuthorScore { get; set; }
        [Required(ErrorMessage = "Выберите произведение")]
        public int? CompositionId { get; set; }

        public CompositionRequest? Composition { get; set; } = default!;
        public List<TagResponse> Tags { get; set; } = new();
        [EnsureOneElement(ErrorMessage = "Загрузите обложку")]
        public List<CloudImageRequest> CloudImages { get; set; } = new();
    }
}
