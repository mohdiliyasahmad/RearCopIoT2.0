using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SvichEx.Services
{
    public interface IQRScannerService
    {
        Task<string> ScanAsync();
    }
}
