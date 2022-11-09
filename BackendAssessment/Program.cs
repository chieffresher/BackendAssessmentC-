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
        static void Main(string[] args)
        {
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

            //send email of events in customer's location
            SendEventsToCustomer(customer, events);

        }

        static void SendEventsToCustomer(Customer customer, List<Event> events)
        {
            //conduct linear search for events matching customer city
            // for each such event send email to customer about it
            bool foundMatchingEvent = false;
            foreach(Event evnt in events)
            {
                if (evnt.Name.Equals(customer.City))
                {
                    if(!foundMatchingEvent)
                     foundMatchingEvent = true;
                    //send email
                    AddToEmail(customer, evnt);
                }
                    
            }
            // if customer has no event matching his city then send him email for all events to maximize our chances 
            // of selling events to him
            if (!foundMatchingEvent)
                foreach (Event evnt in events)
                    AddToEmail(customer, evnt);
        }

        static void SendClosestEvents(Customer customer, List<Event> events)
        {
            /*GetDistance may be an expensive method so we should avoid calling it unnecessarily. 
             * To achieve this we keep a dictionary of already computed distances.
             */
            Dictionary<string, double> searchedDistances = new Dictionary<string, double>();
            Dictionary<Event, double> eventDistances = new Dictionary<Event, double>();
            double distance = 0;
            foreach (Event evt in events)
            {
                if (!searchedDistances.ContainsKey(evt.City))
                {
                    distance = GetDistance(customer.City, evt.City);
                    searchedDistances.Add(evt.City, distance);
                    eventDistances.Add(evt, distance);
                }
                else
                    eventDistances.Add(evt, searchedDistances[evt.City]);
            }

            //sort searched result
            List<SortedDistance> sortedDistances = new List<SortedDistance>();
            foreach (var item in eventDistances)
                sortedDistances.Add(new SortedDistance { Event = item.Key, Distance = item.Value });
            sortedDistances.Sort();
            //send email to 5 closest cities
            for (int i = 0; i < 5 && i < sortedDistances.Count; i++)
                AddToEmail(customer, sortedDistances[i].Event);
        }

        //Question 3:
        Dictionary<string, double> searchedDistances = new Dictionary<string, double>();
        static void SendClosestEventsImprovement(Customer customer, List<Event> events,Dictionary<string,double> searchedDistances)
        {
            /*GetDistance may be an expensive method so we should avoid calling it unnecessarily. 
             * To achieve this we keep a dictionary of already computed distances.
             */
            
            Dictionary<Event, double> eventDistances = new Dictionary<Event, double>();
            double distance = 0;
            foreach (Event evt in events)
            {
                //check if results is global dictionary
                if (!searchedDistances.ContainsKey(evt.City))
                {
                    try
                    {
                        distance = GetDistance(customer.City, evt.City);
                    }
                    catch(Exception )
                    {
                           distance = 0;
                    }
                    
                    searchedDistances.Add(evt.City, distance);
                    eventDistances.Add(evt, distance);
                }
                else
                    eventDistances.Add(evt, searchedDistances[evt.City]);
            }

            //sort searched result
            List<SortedDistance> sortedDistances = new List<SortedDistance>();
            foreach (var item in eventDistances)
                sortedDistances.Add(new SortedDistance { Event = item.Key, Distance = item.Value });
            sortedDistances.Sort();
            //send email to 5 closest cities
            for (int i = 0; i < 5 && i < sortedDistances.Count; i++)
                AddToEmail(customer, sortedDistances[i].Event);
        }


        class SortedDistance : IComparable
        {
            public Event Event { set; get; }
            public double Distance { set; get; }
            public int CompareTo(Object obj)
            {
                if (obj == null)
                    return 1;
                SortedDistance otherObjet = obj as SortedDistance;
                if (otherObjet != null)
                    return this.Distance.CompareTo(otherObjet.Distance);
                else
                   throw new ArgumentException("Object is not a valid DistanceFromCity");
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
        static void AddToEmail(Customer cus, Event evt)
        { }
        static double GetDistance(string fromCity,string toCity)
        {
            return 0.00;
        }
    }
}


