using System;
using System.Collections.Generic;
using System.Text;

namespace Ewan.Domain.Enums
{
    public enum AdminRole
    {
        SuperAdmin = 1, // كل شيء بدون استثناء، بما فيها إدارة المستخدمين
        Editor = 2, // كل المحتوى (إنشاء/تعديل/حذف) + متابعة الاستفسارات
        ContentManager = 3, // المحتوى (إنشاء/تعديل) بدون صلاحية حذف
        LeadsViewer = 4 // الاستفسارات بس (عرض ومتابعة)، من غير أي وصول للمحتوى
    }
}
