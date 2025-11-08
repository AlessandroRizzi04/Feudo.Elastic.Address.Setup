using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Feudo.Elastic.Address.Setup.Services;

namespace Feudo.Elastic.Address.Setup
{
    public class AddressSetup : IAddressSetup
    {
        private readonly IOsmService _osmService;

        public AddressSetup(IOsmService osmService)
        {
            _osmService = osmService;
        }

        public async Task Setup()
        {
            string osmFilePath = Path.Combine("Data", "italy-latest.osm.pbf");
            
            Console.WriteLine("Start index Osm");
            await _osmService.Index(osmFilePath);
            Console.WriteLine("End index Osm");
        }
    }
}
