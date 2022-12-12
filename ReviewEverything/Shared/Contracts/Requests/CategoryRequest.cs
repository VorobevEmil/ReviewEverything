﻿using System.ComponentModel.DataAnnotations;

namespace ReviewEverything.Shared.Contracts.Requests
{
    public class CategoryRequest
    {
        [Required]
        public string Title { get; set; } = default!;
    }
}
