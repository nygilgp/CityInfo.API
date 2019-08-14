using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CityInfo.API;
using Cityinfo.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Logging;

namespace CityInfo.API.Controllers
{
    [Route("api/cities")]
    public class PointsOfInterestController : Controller
    {
        private ILogger<PointsOfInterestController> _logger;

        public PointsOfInterestController(ILogger<PointsOfInterestController> logger)
        {
            _logger = logger;
        }

        [HttpGet("{cityId}/pointsofinterest")]
        public IActionResult GetPointsOfInterest(int cityId)
        {
            try
            {
                var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);

                if (city == null)
                {
                    _logger.LogInformation($"City with id {cityId} wasn't found accesing point of interest.");
                    return NotFound();
                }

                return Ok(city.PointsOfInterest);
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception while getting points of interest for city with id {cityId}.", ex);
                return StatusCode(500, "A problem happened while handling your request.");
            }            
        }

        [HttpGet("{cityId}/pointsofinterest/{id}", Name = "GetPointOfInterest")]
        public IActionResult GetPointOfInterest(int cityId, int id)
        {
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);

            if (city == null)
            {
                return NotFound();
            }

            var pointOfInterest = city.PointsOfInterest.FirstOrDefault(p => p.Id == id);

            if (pointOfInterest == null)
            {
                return NotFound();
            }

            return Ok(pointOfInterest);
        }
         
        [HttpPost("{cityId}/pointsofiterest")]
        public IActionResult CreatePointOfIntrest(int cityId,
            [FromBody] PointOfInterestForCreationDto pointOfIntereset)
        {
            if(pointOfIntereset == null)
            {
                return BadRequest();
            }

            if(pointOfIntereset.Name == pointOfIntereset.Description)
            {
                ModelState.AddModelError("Description", "The provided description should be different from name.");
            }

            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var city = CitiesDataStore.Current.Cities.Find(c => c.Id == cityId);

            if(city == null)
            {
                return NotFound();
            }

            var maxPointOfInterestId = CitiesDataStore.Current.Cities.SelectMany(
                c => c.PointsOfInterest).Max(p => p.Id);

            var finalPointOfInterest = new PointOfInterestDto()
            {
                Id = ++maxPointOfInterestId,
                Name = pointOfIntereset.Name,
                Description = pointOfIntereset.Description
            };

            city.PointsOfInterest.Add(finalPointOfInterest);

            return CreatedAtRoute("GetPointOfInterest", 
                new { cityId = cityId, id = finalPointOfInterest.Id }, finalPointOfInterest);
        }

        [HttpPut("{cityId}/pointsofiterest/{id}")]
        public IActionResult UpdatePointOfIntrest(int cityId, int id,
           [FromBody] PointOfInterestForUpdateDto pointOfIntereset)
        {
            if (pointOfIntereset == null)
            {
                return BadRequest();
            }

            if (pointOfIntereset.Name == pointOfIntereset.Description)
            {
                ModelState.AddModelError("Description", "The provided description should be different from name.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var city = CitiesDataStore.Current.Cities.Find(c => c.Id == cityId);

            if (city == null)
            {
                return NotFound();
            }

            var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(p => p.Id == id);

            if (pointOfInterestFromStore == null)
            {
                return NotFound();
            }

            pointOfInterestFromStore.Name = pointOfIntereset.Name;
            pointOfInterestFromStore.Description = pointOfIntereset.Description;

            return NoContent();
        }

        [HttpPatch("{cityId}/pointsofiterest/{id}")]
        public IActionResult PartiallyUpdatePointOfIntrest(int cityId, int id,
           [FromBody] JsonPatchDocument<PointOfInterestForUpdateDto> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            var city = CitiesDataStore.Current.Cities.Find(c => c.Id == cityId);

            if (city == null)
            {
                return NotFound();
            }

            var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(p => p.Id == id);

            if (pointOfInterestFromStore == null)
            {
                return NotFound();
            }

            var pointOfInterestToPatch = new PointOfInterestForUpdateDto()
            {
                Name = pointOfInterestFromStore.Name,
                Description = pointOfInterestFromStore.Description
            };
            patchDoc.ApplyTo(pointOfInterestToPatch, ModelState);

            if(!ModelState.IsValid)
            {
                return BadRequest();
            }
            // Validation for name and description 
            if (pointOfInterestToPatch.Name == pointOfInterestToPatch.Description)
            {
                ModelState.AddModelError("Description", "The provided description should be different from name.");
            }
            TryValidateModel(pointOfInterestToPatch);
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            pointOfInterestFromStore.Name = pointOfInterestToPatch.Name;
            pointOfInterestFromStore.Description = pointOfInterestToPatch.Description;

            return NoContent();
        }

        [HttpDelete("{cityId}/pointsofiterest/{id}")]
        public IActionResult DeletePointOfInterest(int cityId, int id)
        {
            var city = CitiesDataStore.Current.Cities.Find(c => c.Id == cityId);

            if (city == null)
            {
                return NotFound();
            }

            var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(p => p.Id == id);

            if (pointOfInterestFromStore == null)
            {
                return NotFound();
            }

            city.PointsOfInterest.Remove(pointOfInterestFromStore);

            return NoContent();
        }

    }
}
