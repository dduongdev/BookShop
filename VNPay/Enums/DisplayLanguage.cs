using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VNPay.Enums
{
    public enum DisplayLanguage : sbyte
    {
        /// <summary>
        /// Giao diện hiển thị bằng tiếng Việt.
        /// </summary>
        [Description("vn")]
        Vietnamese,

        /// <summary>
        /// Giao diện hiển thị bằng tiếng Anh.
        /// </summary>
        [Description("en")]
        English
    }
}
