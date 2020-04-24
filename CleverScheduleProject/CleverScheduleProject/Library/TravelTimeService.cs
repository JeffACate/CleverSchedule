using BingMapsRESTToolkit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CleverScheduleProject.Constants;

namespace CleverScheduleProject.Library
{
    public class TravelTimeService
    {
        public async Task<double> GetTravelTime(Models.Address fromAddress, Models.Address toAddress)
        {
            var request = new RouteRequest()
            {
                Waypoints = new List<SimpleWaypoint>()
                {
                    new SimpleWaypoint(fromAddress.Lat, fromAddress.Lon),
                    new SimpleWaypoint(toAddress.Lat, toAddress.Lon)
                },
                BingMapsKey = Constants.API_Keys.Bing
            };
            double travelTime = 0;
            var response = await ServiceManager.GetResponseAsync(request);
            if (response != null &&
                response.ResourceSets != null &&
                response.ResourceSets.Length > 0 &&
                response.ResourceSets[0].Resources != null &&
                response.ResourceSets[0].Resources.Length > 0)
            {
                var result = response.ResourceSets[0].Resources[0] as BingMapsRESTToolkit.Route;

                //Do something with the result.
                travelTime = Convert.ToDouble(result.TravelDuration);

            }
            return travelTime;

        }
    }
}
