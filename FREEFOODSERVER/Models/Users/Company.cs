﻿using Microsoft.AspNetCore.Identity;

namespace FREEFOODSERVER.Models.Users
{
    public class Company : IdentityUser
    { 
        public string CompanyName { get; set; } = null!;

        public string? Discription { get; set; } 

        public List<Bag> Bags { get; set; } = new List<Bag>();

        public Guid? ImagePreview { get; set; } 

        public float? AvgEvaluation { get; set; }

    }
}
