using System;
using System.Collections.Generic;

namespace CMS.src.Application.DTOs.Content
{
    public class PageTranslationDto
    {
        public string Language { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string BlocksJson { get; set; } = null!;
    }

    public class PageSaveDto
    {
        public string Slug { get; set; } = null!;
        public bool IsPublished { get; set; }
        public List<PageTranslationDto> Translations { get; set; } = new();
    }

    public class PageDto
    {
        public Guid Id { get; set; }
        public Guid SiteId { get; set; }
        public string Slug { get; set; } = null!;
        public bool IsPublished { get; set; }
        public List<PageTranslationDto> Translations { get; set; } = new();
    }
}
