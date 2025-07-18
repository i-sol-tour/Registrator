using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Registrator.services
{
    public static class Delay
    {
        public static async Task ExecuteWithDelay(int milliseconds)
        {
            await Task.Delay(milliseconds);
        }
    }
}
