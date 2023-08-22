﻿using System.ComponentModel.DataAnnotations;

namespace FREEFOODSERVER.Models.ViewModel.Company
{
    public class CompanyPreviewViewModel
    {
        public Guid Id { get; set; }
        
        public string? ImagePreview { get; set; }

        public string CompanyName { get; set; } = null!;

    }
}
