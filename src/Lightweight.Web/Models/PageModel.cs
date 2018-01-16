using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Lightweight.Web.Models
{
    public enum PageMode { Create, Render, Layout, Edit }

    public class PageModel
    {
        public Guid Id { get; set; }
        public Guid? ParentId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Slug { get; set; }

        public string Url { get; set; }
        public string IconUrl { get; set; }
        public int Order { get; set; }

        public bool Published { get; set; }
        public bool MenuOnly { get; set; }
        public bool MembersVisible { get; set; }
        public bool GuestsVisible { get; set; }

        public bool Selected { get; set; }
        public PageMode Mode { get; set; } // Render / Edit / Layout / Permissions

        public List<PagePermissionModel> Permissions { get; set; }
        public List<PageWidgetModel> Widgets { get; set; }

        public PageModel()
        {
            Permissions = new List<PagePermissionModel>();
        }
    }
}