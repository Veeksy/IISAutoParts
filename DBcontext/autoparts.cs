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
    
    public partial class autoparts
    {
        public int id { get; set; }
        public string manufacturer { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public Nullable<decimal> price { get; set; }
        public Nullable<int> year { get; set; }
        public Nullable<int> count { get; set; }
        public Nullable<int> idCategory { get; set; }
    }
}
