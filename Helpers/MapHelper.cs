using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RestSharp;
using Ares.Models;
using Newtonsoft.Json;
using Ares.EntityData.Model;
namespace Ares.Helpers
{
    public class MapHelper
    {
        private static string apiKey = "AIzaSyBF4fZ_6PlAfMP2pgwo2sisPSh42aWlfXo";
        public static Geocode getGeocode(string address,string city)
        {
            var client = new RestClient("https://maps.googleapis.com/maps/api/geocode/json?key="+ apiKey + "&address=" + address + " "+ city + " Türkiye");
            var request = new RestRequest(Method.GET);
            //request.AddHeader("Postman-Token", "32133d55-cfee-4c5d-a1de-e6418a09c95a");
            request.AddHeader("Cache-Control", "no-cache");
            IRestResponse response = client.Execute(request);
            string jsonData = response.Content;
            Geocode geocode = JsonConvert.DeserializeObject<Geocode>(jsonData);
            return geocode;
        }
        public static Geocode getGeocode(double lat, double lng)
        {
            // Example  URL : https://maps.googleapis.com/maps/api/geocode/json?latlng=40.714224,-73.961452&key=AIzaSyBF4fZ_6PlAfMP2pgwo2sisPSh42aWlfXo
            string sLat = lat.ToString();
            sLat = sLat.Replace(",", ".");
            string sLng = lng.ToString();
            sLng = sLng.Replace(",", ".");
            string latlng = sLat + "," + sLng;
            var client = new RestClient("https://maps.googleapis.com/maps/api/geocode/json?latlng=" + latlng + "&key=" + apiKey);
            var request = new RestRequest(Method.GET);
            //request.AddHeader("Postman-Token", "32133d55-cfee-4c5d-a1de-e6418a09c95a");
            request.AddHeader("Cache-Control", "no-cache");
            IRestResponse response = client.Execute(request);
            string jsonData = response.Content;
            Geocode geocode = JsonConvert.DeserializeObject<Geocode>(jsonData);
            return geocode;
        }
        
        public static Distance getDistance(Location destination, Location point,Student student)
        {
            /// http://185.115.242.209/route/v1/driving/13.388860,52.517037;13.397634,52.529407?overview=false
            string url = "http://185.115.242.209/route/v1/driving/" + destination.lng.ToString().Replace(",",".") + "," + destination.lat.ToString().Replace(",", ".") + ";" + point.lng.ToString().Replace(",", ".") + "," + point.lat.ToString().Replace(",", ".") + "?overview=false";
            var client = new RestClient(url);
            var request = new RestRequest(Method.GET);
            //request.AddHeader("Postman-Token", "32133d55-cfee-4c5d-a1de-e6418a09c95a");
            request.AddHeader("Cache-Control", "no-cache");
            IRestResponse response = client.Execute(request);
            string jsonData = response.Content;
            Distance distance = JsonConvert.DeserializeObject<Distance>(jsonData);
            distance.student = student;
            return distance;
        }
        public static Distance farthestLocation(List<Student> studentList,Location destination,bool closer)
        {
            if (studentList.Count > 0)
            {

                List<Distance> distances = new List<Distance>();
                //// En Uzak Noktayı Bulma
                foreach (var item in studentList)
                {
                    Location _tmp = new Location();
                    _tmp.lat = item.Station.Lat;
                    _tmp.lng = item.Station.Lng;
                    distances.Add(MapHelper.getDistance(destination, _tmp, item));
                }
                List<Distance> sortedDistance = distances.OrderBy(o => o.routes.First().distance).ToList();
                /// En Uzak Noktayı Bulma
                /// 
                if (closer)
                    return sortedDistance.First();
                else
                    return sortedDistance.Last();
            }
            return null;
        }
        public static List<Distance> closerLocation(List<Student> studentList, Location destination)
        {
            if (studentList.Count > 0)
            {

                List<Distance> distances = new List<Distance>();
                //// En Uzak Noktayı Bulma
                foreach (var item in studentList)
                {
                    Location _tmp = new Location();
                    _tmp.lat = item.Station.Lat;
                    _tmp.lng = item.Station.Lng;
                    distances.Add(MapHelper.getDistance(destination, _tmp, item));
                }
                List<Distance> sortedDistance = distances.OrderBy(o => o.routes.First().distance).ToList();
                return sortedDistance;
            }
            else
                return null;
        }
    }
}