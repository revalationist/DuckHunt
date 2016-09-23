using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Duck_Hunt
{
    class Debug
    {
        ProcessStartInfo psi = new ProcessStartInfo("cmd.exe")
        {
            RedirectStandardError = true,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            UseShellExecute = false
        };

        Process test = new Process();
        
    }
}
