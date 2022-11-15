using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BackendAssessment
{
    public class Event
    {
        public string Name { get; set; }
        public string City { get; set; }
    }
    public class Customer
    {
        public string Name { get; set; }
        public string City { get; set; }
    }


    public class Solution
    {
        public static Dictionary<string, double> distancesBetweenCities;

        static void Main(string[] args)
        {
            distancesBetweenCities = new Dictionary<string, double>();

            var events = new List<Event>{
                                            new Event{ Name = "Phantom of the Opera", City = "New York"},
                                            new Event{ Name = "Metallica", City = "Los Angeles"},
                                            new Event{ Name = "Metallica", City = "New York"},
                                            new Event{ Name = "Metallica", City = "Boston"},
                                            new Event{ Name = "LadyGaGa", City = "New York"},
                                            new Event{ Name = "LadyGaGa", City = "Boston"},
                                            new Event{ Name = "LadyGaGa", City = "Chicago"},
                                            new Event{ Name = "LadyGaGa", City = "San Francisco"},
                                            new Event{ Name = "LadyGaGa", City = "Washington"}
                                            };
            //1. find out all events that arein cities of customer
            // then add to email.
            var customer = new Customer { Name = "Mr. Fake", City = "New York" };

            //compute and save distances between cities into a dictionary
            populateDistanceBetweenCities(events, customer);

            //send email of events in customer's location
            Console.WriteLine("Sending events happening in customer's city:");
            SendEventsToCustomer(customer, events);

            //send to closest
            Console.WriteLine("Sending five events closest to customer's city:");
            SendClosestEvents(customer, events);
           
            Console.ReadLine();

        }

        public static void populateDistanceBetweenCities(List<Event> events,Customer customer)
        {
            //get unique cities
            List<string> uniqCities = events.Select(e => e.City).Distinct().ToList();
            
            //cross-product of cities excluding city to itself
            var crossProduct = from city1 in uniqCities
                               from city2 in uniqCities
                               where city1 != city2
                               select new { city1, city2 };

            double distance = 0;
            foreach (var entry in crossProduct)
            {
                if (!(distancesBetweenCities.ContainsKey(entry.city1 + "#" + entry.city2) ||
                    distancesBetweenCities.ContainsKey(entry.city2 + "#" + entry.city1)))
                {
                    distance = GetDistance(entry.city1, entry.city2);
                    distancesBetweenCities.Add(entry.city1 + "#" + entry.city2, distance);
                    //we could store city2 to city1 as well as shown below but that represent avoidable memory usage
                    //distancesBetweenCities.Add(entry.city2 + "#" + entry.city1, distance);
                }
            }
        }

        public static double getDistanceBetweenCities(string city1, string city2)
        {
            if (city1.Equals(city2))
                return 0;
            double distance = 0;
            //if distance is not in dictionary then compute it and add to dict
            string key1 = city1 + "#" + city2;
            string key2 = city2 + "#" + city1;
            if (!(distancesBetweenCities.ContainsKey(key1) || distancesBetweenCities.ContainsKey(key2)))
            {
                distance = GetDistance(city1, city2);
                distancesBetweenCities.Add(key1, distance);
            }

            distance = distancesBetweenCities.ContainsKey(key1) == true ? 
                distancesBetweenCities[key1] : distancesBetweenCities[key2];

            return distance;
        }
      
        static void SendEventsToCustomer(Customer customer, List<Event> events)
        {
            //select events matching customer city
            IEnumerable<Event> eventsInCustomerCity = from evt in events
                                                       where evt.City.Equals(customer.City)
                                                       select evt;
            //get distances and send events
            foreach(Event evt in eventsInCustomerCity)
                AddToEmail(customer, evt);
        }

        static void SendClosestEvents(Customer customer, List<Event> events)
        {
            //sort distance between cities dictionary by distances
            var sortedDistances = distancesBetweenCities.OrderBy(dc => dc.Value).Select(dc => dc);
            // send to 5 closest cities
            //send email to 5 closest cities
            int count = 0;
            string otherCity = "";
           foreach(var dc in sortedDistances)
            {
                if(dc.Key.Contains("#"+customer.City) || dc.Key.Contains(customer.City+"#"))
                {
                    //get events happening at this city
                    otherCity = dc.Key.Contains("#" + customer.City) ? dc.Key.Split('#')[0] : dc.Key.Split('#')[1];
                    IEnumerable<Event> eventsAtCity = from evt in events
                                                      where evt.City.Equals(otherCity)
                                                      select evt;
                   
                    foreach(Event evt in eventsAtCity)
                    {
                        AddToEmail(customer, evt);
                        count++;
                        if (count >= 5)
                            return;
                    }
                      
                }
            }

            
        }




        /*Answers to Question 1 :
         * 
         * 1. Since the list is not sorted, a linear search for events matching customer's city is the best approach
         * 2. If the AddToEmail is not in the same namespace as current project, then it must be imported with a using 
         *    statement. After that we can use the AddToEmail method by calling with Customer and Event objects like:
         *    AddToEmail(customer,event); // where customer is Customer object and event is Event object
         * 3. If John Smith is the only client, the current implementation with only send email of events happening in his city to him
         *    but we may consider sending him email of all events to maximize our chances of selling an event.
         *    If that decision is against company policy then we may consider sending him only events happening in his city.
         * 4. We can improve the search code to use binary search instead of linear search, in which case should consider sorting
         *    events by city. Sorting events each time before performing binary search is not an improvement since sorting is 
         *    nlogn operation, which is more expensive than linear runtime operation. So sorting and performing binary operation is
         *    nlogn + n runtime operation.
         * 
         *  
         *  Answers to Question 2 :
         *  1. GetDistance is an expensive method so we make effort to avoid making duplicate calls. Using dictionary to 
         *     store previously computed distance is an efficient approach. We loop through events computing distances between
         *     customer's city and each event city except already computed distances.
         *  2. After computing distances in (1), we assign event and their distances from customer city to sortable objects - objects
         *     of a class that implements IComparable interface. We then call Sort() to sort list of such objects. CompareTo() is 
         *     implemented to sort by distances. We further loop through first five objects of sorted list and call AddToEmail on each
         *  3. Current implementation will send emails for the first five events that are closest to John Smith. This can change based
         *     on company policy.
         *  4. This implementation uses two dictionaries which can be improved to improve memory efficiency.
         *  
         *  
         *  Answers to Question 3:
         *  We can store results of previously searched distance in a global dictionary object. We first search this object for
         *  the result and only make call to API when results is not in that object. When result is not in dictionary and API call
         *  fails we return 0 for distance.
         *  
         *  
         *  Answers to Question 4:
         *  The improvements introduced in (3) caters for this situation, except in place of returning 0 when global dictionary does
         *  not contain search results and API call fails, company policy can dictate what should be done.
         *  
         *  Answers to Question 5:
         *  First we need to add Price property to Event. We can assign objects of searched result to objects implement IComparable
         *  interface but this time CompareTo() sorts by Price.
         */




        //code below is just dummies for API methods
        static void AddToEmail(Customer c, Event e, int? price = null)
        {
            var distance = GetDistance(c.City, e.City);
            Console.Out.WriteLine($"{c.Name}: {e.Name} in {e.City}"
            + (distance > 0 ? $" ({distance} miles away)" : "")
            + (price.HasValue ? $" for ${price}" : ""));
        }
        static int GetPrice(Event e)
        {
            return (AlphebiticalDistance(e.City, "") + AlphebiticalDistance(e.Name, "")) / 10;
        }
        static int GetDistance(string fromCity, string toCity)
        {
            return AlphebiticalDistance(fromCity, toCity);
        }
        private static int AlphebiticalDistance(string s, string t)
        {
            var result = 0;
            var i = 0;
            for (i = 0; i < Math.Min(s.Length, t.Length); i++)
            {
                // Console.Out.WriteLine($"loop 1 i={i} {s.Length} {t.Length}");
                result += Math.Abs(s[i] - t[i]);
            }
            for (; i < Math.Max(s.Length, t.Length); i++)
            {
                // Console.Out.WriteLine($"loop 2 i={i} {s.Length} {t.Length}");
                result += s.Length > t.Length ? s[i] : t[i];
            }
            return result;
        }

    }
}


