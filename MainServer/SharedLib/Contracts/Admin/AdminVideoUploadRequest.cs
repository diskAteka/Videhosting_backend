using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace SharedLib.Contracts.Admin
{
    /// <summary>
    /// Загрузка видео из AdminPanel: файл видео и (опционально) файл постера
    /// отправляются на сервер, сервер сам кладёт их в облачное хранилище.
    /// </summary>
    public class AdminVideoUploadRequest
    {
        [FromForm(Name = "name")]
        [Required(ErrorMessage = "Название видео обязательно")]
        public string Name { get; set; } = null!;

        [FromForm(Name = "description")]
        public string? Description { get; set; }

        [FromForm(Name = "dateUpload")]
        public DateTime DateUpload { get; set; } = DateTime.UtcNow;

        [FromForm(Name = "isVerified")]
        public bool IsVerified { get; set; }

        [FromForm(Name = "authorId")]
        [Range(1, int.MaxValue, ErrorMessage = "Автор обязателен")]
        public int AuthorId { get; set; }

        [FromForm(Name = "likes")]
        public int Likes { get; set; }

        [FromForm(Name = "dislikes")]
        public int Dislikes { get; set; }

        [FromForm(Name = "views")]
        public int Views { get; set; }

        [FromForm(Name = "video")]
        public IFormFile? Video { get; set; }

        /// <summary>
        /// Если не передан — сервер извлечёт кадр из видео.
        /// </summary>
        [FromForm(Name = "poster")]
        public IFormFile? Poster { get; set; }
    }
}
