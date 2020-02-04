﻿using System.Collections.Generic;

namespace TheraLang.Web.ViewModels
{
    public class SiteMapViewModel
    {
        public int Id { get; set; }
        
        public string MenuTitle { get; set; }
        
        public string Route { get; set; }
        
        public string Header { get; set; }

        public IEnumerable<SiteMapViewModel> SubPages { get; set; }
    }
}