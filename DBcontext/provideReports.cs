//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace IISAutoParts.DBcontext
{
    using System;
    using System.Collections.Generic;
    
    public partial class provideReports
    {
        public int id { get; set; }
        public byte[] doc { get; set; }
        public Nullable<int> providerId { get; set; }
        public Nullable<System.DateTime> dateBegin { get; set; }
        public Nullable<System.DateTime> dateEnd { get; set; }
    }
}
