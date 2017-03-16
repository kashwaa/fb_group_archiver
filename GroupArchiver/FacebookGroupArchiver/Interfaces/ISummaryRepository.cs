using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacebookGroupArchiver.Interfaces
{
    public interface ISummaryRepository
    {
        void SaveSummary(List<string> files);
    }
}
