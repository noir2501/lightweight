using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lightweight.Model.Entities
{
    public class PageWidget : IEntity<Guid>
    {
        public virtual Guid Id { get; set; }

        public virtual string Title { get; set; }

        public virtual int Col { get; set; }
        public virtual int Row { get; set; }
        public virtual int Width { get; set; }
        public virtual int Height { get; set; }

        public virtual string Content { get; set; }

        public virtual Page Page { get; set; }
        public virtual Module Module { get; set; }
    }
}
