﻿using System.Diagnostics.CodeAnalysis;

namespace DotNetTruyen.Models
{
    public class Chapter : BaseEnity<Guid>
    {
        [AllowNull]
        public string? ChapterTitle { get; set; }
        public int ChapterNumber { get; set; }
        public DateTime? PublishedDate { get; set; }
        public int Views { get; set; }
        public Guid ComicId { get; set; }
        public Comic Comic { get; set; }
        public bool IsPublished { get; set; }
        public ICollection<ChapterImage> Images { get; set; }
    }
}
