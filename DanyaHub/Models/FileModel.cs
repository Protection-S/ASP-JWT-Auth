using System.ComponentModel.DataAnnotations;

namespace DanyaHub.Models
{
    public class FileModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Имя файла обязательно для заполнения")]
        public string FileName { get; set; }

        [Required(ErrorMessage = "Путь к файлу обязателен для заполнения")]
        public string FilePath { get; set; }
    }
}
