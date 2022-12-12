﻿using System.ComponentModel.DataAnnotations;

namespace ReviewEverything.Shared.Contracts.Requests
{
    public class CompositionRequest
    {
        public int CategoryId { get; set; } = 1;
        [Required]
        public string Title { get; set; } = default!;
    }
}
