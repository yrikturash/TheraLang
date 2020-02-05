﻿using System;
using System.Collections.Generic;
using System.Text;

namespace TheraLang.BLL.DataTransferObjects.NewsDtos
{
    public class NewsPreviewDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public string PreviewImageUrl { get; set; }
    }
}