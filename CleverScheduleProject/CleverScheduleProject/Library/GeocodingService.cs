using BingMapsRESTToolkit;
using CleverScheduleProject.Constants;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace CleverScheduleProject.Library
{
    public class GeocodingService
    {
        public async Task<double[]> GetCoords(Models.Address addressToCode)
        {
            //Create a request.
            var request = new GeocodeRequest()
            {
                Address = new SimpleAddress()
                {
                    AddressLine = addressToCode.Street,
                    Locality = addressToCode.City,
                    AdminDistrict = addressToCode.State,
                    PostalCode = addressToCode.Zip
                },
                BingMapsKey = API_Keys.Bing
            };


            // I Want to check the response, but it appears to be skipping over the steps below.

            //Process the request by using the ServiceManager.
            double[] coords = new double[2];
            var response = await ServiceManager.GetResponseAsync(request);
            if (response != null &&
                response.ResourceSets != null &&
                response.ResourceSets.Length > 0 &&
                response.ResourceSets[0].Resources != null &&
                response.ResourceSets[0].Resources.Length > 0)
            {
                var result = response.ResourceSets[0].Resources[0] as BingMapsRESTToolkit.Location;

                coords[0] = result.Point.Coordinates[0];
                coords[1] = result.Point.Coordinates[1];
            }
            return coords;
        }
    }
}
