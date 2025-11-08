using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feudo.Elastic.Address.Setup.Services
{
    public interface IOsmService
    {
        Task Index(string osmFilePath);
    }
}
