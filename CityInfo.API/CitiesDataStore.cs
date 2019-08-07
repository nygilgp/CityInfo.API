using Cityinfo.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cityinfo.API
{
    public class CitiesDataStore
    {
        // Added using the lates syntax of C#, which will help to keep
        // the current state of city data store intact till refreshed
        public static CitiesDataStore Current { get; } = new CitiesDataStore();
        public List<CityDto> Cities { get; set; }

        public CitiesDataStore()
        {
            Cities = new List<CityDto>()
            {
                new CityDto()
                {
                    Id = 1,
                    Name = "Delhi",
                    Description = "Capital city"
                },
                new CityDto()
                {
                    Id = 1,
                    Name = "Trivandrum",
                    Description = "Capital city of Kerala"
                }
            };
        }
    }
}
