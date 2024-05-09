using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontEnd.Reports
{
    public interface IReportPage
    {
        public double PageWidth { get; set; }
        public double PageHeight { get; set; }

        public void AdjustPageSize();
    }
}
