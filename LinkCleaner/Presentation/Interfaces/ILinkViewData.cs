using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkCleaner.Presentation.Interfaces
{
    public interface ILinkViewData
    {
        bool IsMonitoring { get; set; }

        string Name { get; set; }

        Guid Guid { get; init; }
    }
}
