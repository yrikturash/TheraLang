﻿using System.Collections.Generic;

namespace TheraLang.DAL.Entities
{
    public class Page : BaseEntity
    {
        public string Content { get; set; }
        
        public string Header { get; set; }
        
        public string MenuName { get; set; }
        
        public int? ParentPageId { get; set; }

        public int SortOrder { get; set; }

        public string Language { get; set; }

        public int RouteId { get; set; }

        public virtual Page ParentPage { get; set; }
        
        public virtual ICollection<Page> SubPages { get; set; }

        public virtual PageRoute Route { get; set; }
    }
}