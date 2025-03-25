using System.ComponentModel.DataAnnotations;

namespace DotNetTruyen.ViewModels.Management
{
    public class CreateRankViewModel
    {
        [Required(ErrorMessage = "Tên cấp bậc là bắt buộc")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Kinh nghiệm yêu cầu là bắt buộc")]
        [Range(0, int.MaxValue, ErrorMessage = "Kinh nghiệm yêu cầu phải lớn hơn hoặc bằng 0")]
        public int ExpRequired { get; set; }

        [Required(ErrorMessage = "Cấp độ là bắt buộc")]
        [Range(0, int.MaxValue, ErrorMessage = "Cấp độ phải lớn hơn hoặc bằng 0")]
        public int Level { get; set; }

        [Required(ErrorMessage = "Loại rank là bắt buộc")]
        public Guid RankTypeId { get; set; }


        public List<RankTypeViewModel>? RankTypes { get; set; }
    }
}
