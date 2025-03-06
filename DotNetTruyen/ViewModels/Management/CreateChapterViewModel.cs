using System.ComponentModel.DataAnnotations;

namespace DotNetTruyen.ViewModels.Management
{
    public class CreateChapterViewModel
    {
        public Guid ComicId { get; set; }

        [Required(ErrorMessage = "Số chương không được để trống")]
        [Range(1, int.MaxValue, ErrorMessage = "Số chương phải lớn hơn 0")]
        public int ChapterNumber { get; set; }

        [Required(ErrorMessage = "Tiêu đề chương không được để trống")]
        [StringLength(255, ErrorMessage = "Tiêu đề chương không được vượt quá 255 ký tự")]
        public string ChapterTitle { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? PublishedDate { get; set; }

        [Required(ErrorMessage = "Vui lòng tải lên ít nhất một hình ảnh")]
        public List<IFormFile> Images { get; set; }
    }
}
