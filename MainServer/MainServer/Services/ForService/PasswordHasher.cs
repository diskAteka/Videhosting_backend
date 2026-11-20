using MainServer.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace MainServer.Services.ForService
{
    public class PasswordHasher : IPasswordHasher
    {
        public (string Hash, string Salt) HashPassword(string password)
        {
            // 1. Создаём массив из 16 байт под соль
            byte[] salt = new byte[16];
            // 2. Заполняем его случайными числами
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            // 3. Преобразуем байты соли в Base64-строку
            string saltString = Convert.ToBase64String(salt);
            // 4. Считаем SHA-256 от конкатенации "пароль + соль"
            string hash = GetHash(password + saltString);
            // 5. Возвращаем кортеж: хеш и соль — оба нужно сохранить в базе
            return (hash, saltString);
        }

        public bool VerifyPassword(string password, string hash, string salt)
        {
            // 1. Убираем возможные пробелы/переносы строк по краям
            hash = hash?.Trim();
            salt = salt?.Trim();
            // 2. Считаем хеш от введённого пароля с той же солью из БД
            string newHash = GetHash(password + salt);
            // 3. Сравниваем с сохранённым хешем
            return hash == newHash;
        }

        public string GetHash(string input)
        {
            using (var sha256 = SHA256.Create())
            {
                // 1. Кодируем строку в UTF-8 байты
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
                // 2. Возвращаем хеш в виде Base64-строки
                return Convert.ToBase64String(bytes);
            }
        }
    }
}
