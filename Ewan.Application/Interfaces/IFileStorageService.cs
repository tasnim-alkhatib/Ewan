using System;
using System.Collections.Generic;
using System.Text;

namespace Ewan.Application.Interfaces
{
    public interface IFileStorageService
    {
        /// <summary>
        /// بيرفع الملف ويرجع الـ Absolute URL بتاعه جاهز يتحط في imageUrl مباشرة
        /// </summary>
        /// <param name="fileStream">محتوى الملف</param>
        /// <param name="fileName">اسم الملف الأصلي (هيتولد اسم فريد منه)</param>
        /// <param name="contentType">مثلا image/jpeg</param>
        /// <param name="folder">تصنيف داخل الـ Container، مثلا "banners" أو "offers"</param>
        Task<string> UploadAsync(Stream fileStream, string fileName, string contentType, string folder);

        /// <summary>بيحذف الملف من التخزين باستخدام الـ URL اللي اترجع وقت الرفع</summary>
        Task<bool> DeleteAsync(string fileUrl);
    }
}
