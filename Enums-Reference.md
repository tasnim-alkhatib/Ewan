# قاموس الـ Enums – Ewan HR API
> ملحوظة: الـ API بيرجّع كل Enum كـ **نص (string)** في الـ JSON (بعد تفعيل JsonStringEnumConverter)، مش رقم.
> يعني هتستقبلي `"location": "HomeSlider"` مش `"location": 1`.
> ولما تبعتي Request (POST/PUT) ابعتي النص برضه، مش الرقم.

---

## 1) Sector — قطاع الخدمة
| القيمة (اللي تتبعت في الـ JSON) | المعنى |
|---|---|
| `Individual` | قطاع الأفراد |
| `Corporate` | قطاع الأعمال |
| `Healthcare` | الرعاية الصحية |
| `Recruitment` | التوسط / نقل الكفالة |

**بيُستخدم في:** `ServiceItem.Sector`, `Faq.Sector`, `Inquiry.Sector`

---

## 2) BannerLocation — مكان ظهور البانر
| القيمة | المعنى |
|---|---|
| `HomeSlider` | سلايدر الصفحة الرئيسية |
| `IndividualSectorTop` | أعلى صفحة قطاع الأفراد |
| `CorporateSectorTop` | أعلى صفحة قطاع الأعمال |
| `OffersTop` | أعلى صفحة العروض |
| `HealthcareTop` | أعلى صفحة الرعاية الصحية |

**بيُستخدم في:** `Banner.Location` — ده اللي بتفلتري بيه لما تجيبي بانرات صفحة معينة:
`GET /api/banners/public?location=HomeSlider`

---

## 3) InquiryStatus — حالة الاستفسار (Lead)
| القيمة | المعنى |
|---|---|
| `New` | جديد |
| `InProgress` | قيد المتابعة |
| `Contacted` | تم التواصل |
| `Closed` | مغلق |
| `Rejected` | مرفوض |

**بيُستخدم في:** `Inquiry.Status` — ده اللي هيتغير من لوحة التحكم لما فريق المبيعات يتابع الطلب.

---

## 4) InquirySource — مصدر الاستفسار
| القيمة | المعنى |
|---|---|
| `HomePage` | الصفحة الرئيسية |
| `IndividualSector` | صفحة قطاع الأفراد |
| `CorporateSector` | صفحة قطاع الأعمال |
| `Offer` | من عرض معين |
| `ContactPage` | صفحة اتصل بنا |
| `Sahla` | خدمة سهلة |

**بيُستخدم في:** `Inquiry.Source` — بتتبعت من الفرونت وقت إرسال أي فورم، عشان نعرف الطلب جاي منين.

---

## 5) AdminRole — صلاحيات مستخدم لوحة التحكم
| القيمة | المعنى | الصلاحيات المتوقعة |
|---|---|---|
| `SuperAdmin` | مدير عام | كل شيء (Users, Content, Leads, Settings) |
| `Editor` | محرر | البانرات، الخدمات، العروض (إضافة/تعديل/حذف) |
| `ContentManager` | مسؤول محتوى | البانرات، الخدمات، العروض (إضافة/تعديل بس، من غير حذف) |
| `LeadsViewer` | مسؤول متابعة عملاء | يشوف ويتابع الاستفسارات (Inquiries) بس |

**بيُستخدم في:** `AdminUser.Role` — وده اللي بيتحدد بيه أي عناصر في لوحة التحكم تظهر لكل مستخدم (Sidebar Items) وأي Endpoints مسموح لها تناديها (حسب الـ `[Authorize(Roles = "...")]` على كل Controller).

---

## ملاحظة تقنية للفرونت
- كل الـ Enums دي هتلاقيها موثقة تلقائيًا في Scalar (`/scalar/v1`) جوه أي Endpoint بيستخدمها، كـ Dropdown فيه القيم النصية دي بالظبط.
- لو حابة تعملي `enums.ts` ثابت في الفرونت بدل ما تكتبي الـ Strings يدوي في كل مكان، استخدمي نفس الأسماء دي حرفيًا (Case-Sensitive).
