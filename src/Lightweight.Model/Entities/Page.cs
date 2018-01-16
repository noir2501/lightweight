using System;
using System.Collections.Generic;
namespace Lightweight.Model.Entities
{
    public class Page : IEntity<Guid>
    {
        public virtual Guid Id { get; set; }

        public virtual string Name { get; set; }
        public virtual string Title { get; set; }
        public virtual string Slug { get; set; }
        public virtual string Url { get; set; }
        public virtual string IconUrl { get; set; }
        public virtual int Order { get; set; }
        public virtual bool Published { get; set; }
        public virtual bool MenuOnly { get; set; }

        public virtual Tenant Tenant { get; set; }
        public virtual Page Parent { get; set; }

        public virtual ISet<PagePermission> Permissions { get; set; }
        public virtual ISet<PageWidget> Widgets { get; set; }

        public Page()
        {
            Permissions = new HashSet<PagePermission>();
            Widgets = new HashSet<PageWidget>();
        }

        public Page(Guid pageId)
            : this()
        {
            Id = pageId;
        }

        public Page(Tenant tenant)
            : this()
        {
            Tenant = tenant;
        }

        public Page(Tenant tenant, string name, string title, string url)
            : this(tenant)
        {
            Name = name;
            Title = title;
            Url = url;
            Slug = title;
        }

        public Page(Page parent, string name, string title, string url)
            : this(parent.Tenant, name, title, url)
        {
            Parent = parent;
            Tenant = parent.Tenant;
        }
    }
}