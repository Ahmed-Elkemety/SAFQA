using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.DAL.Enums
{
    public enum Unit
    {
        None = 0,          // من غير وحدة

        // الأطوال
        Centimeter = 1,
        Meter = 2,
        Inch = 3,

        // الوزن
        Gram = 10,
        Kilogram = 11,
        Pound = 12,

        // الحجم
        Liter = 20,
        Milliliter = 21,

        // المساحة
        SquareMeter = 30,

        // الوقت
        Second = 40,
        Minute = 41,
        Hour = 42
    }
}
