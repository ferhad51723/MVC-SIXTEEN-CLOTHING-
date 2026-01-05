using Microsoft.AspNetCore.Mvc;

namespace MyProject.Utilities.Extensions
{
    public static class Validator
    {
        public static bool ValidateType(this IFormFile formFile, string type)
        {
            if (formFile.ContentType.Contains(type))
            {
                return true;
            }
            return false;
        }
        public static bool ValidateSize(this IFormFile formFile, int size, Enums.FileSize fileSize)
        {
            switch (fileSize)
            {
                case Enums.FileSize.KB:
                    return formFile.Length > size * 1024;
                case Enums.FileSize.MB:
                    return formFile.Length > size * 1024 * 1024;
                case Enums.FileSize.GB:
                    return formFile.Length > size * 1024 * 1024 * 1024;
            }
            return false;
        }
        public async static Task<string> CreateFile(this IFormFile formFile, params string[] roots)
        {
            string fileName = Guid.NewGuid().ToString() + "_" + formFile.FileName;
            string path = string.Empty;
            for (int i = 0; i < roots.Length; i++)
            {
                path = Path.Combine(path, roots[i]);
            }
            path = Path.Combine(path, fileName);
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                await formFile.CopyToAsync(fs);
            }
            return fileName;
        }
    }
}