﻿using System.ComponentModel.DataAnnotations;

namespace FREEFOODSERVER.Models.ViewModel.BagViewModel
{
    public class BagInfoViewModel
    {
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        /// <summary>
        /// Первое изображение в списке - превью
        /// </summary>
        public List<string>? ImagesId { get; set; }

        [Required]
        public uint Count { get; set; } = 0;

        [Required]
        public double Cost { get; set; } = 0;

        public UInt64 NumberOfViews { get; set; }

        public List<string> Tags { get; set; } = null!;

        public DateTime Created { get; set; }

        public bool IsDisabled { get; set; }

        public float? AvgEvaluation { get; set; }

        public static implicit operator BagInfoViewModel(Bag model)
        {
            return new()
            {
                Id = model.Id,
                ImagesId = model.ImagesId,
                Count = model.Count,
                Cost = model.Cost,
                Description = model.Description,
                Name = model.Name,
                NumberOfViews = model.NumberOfViews,
                Tags = model.Tags,
                Created = model.Created,
                IsDisabled = model.IsDisabled,
                AvgEvaluation = model.AvgEvaluation,
            };
        }

        

    }
}
