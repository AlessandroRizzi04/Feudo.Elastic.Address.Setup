using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Feudo.Elastic.Address.Setup.Services;
using Feudo.Elastic.Db.Address.Configuration;

namespace Feudo.Elastic.Address.Setup
{
    public class AddressSetup : IAddressSetup
    {
        private readonly IOsmService _osmService;
        private readonly OsmConfigurations _osmConfigurations;
        public AddressSetup(IOsmService osmService, OsmConfigurations osmConfigurations)
        {
            _osmService = osmService;
            _osmConfigurations = osmConfigurations;
        }

        public async Task Setup()
        {
            Console.WriteLine("Start index Osm");
            await _osmService.Index(_osmConfigurations.FileItaly);
            Console.WriteLine("End index Osm");
        }
    }
}
