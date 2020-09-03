using System;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections.Generic;
using Google.OrTools.ConstraintSolver;
using Microsoft.AspNetCore.Mvc;

/// <summary>
///   Vehicles Routing Problem (VRP) with Time Windows.
/// </summary>
public class RouteOptimization
{
    class DataModel
    {
        public long[,] TimeMatrix = { };

        public long[,] TimeWindows = {
          {0, 5},    // depot
          {7, 12},   // 1
          {10, 15},  // 2
          {16, 18},  // 3
          {10, 13},  // 4
          {0, 5},    // 5
          {5, 10},   // 6
          {0, 4},    // 7
          {5, 10},   // 8
          {0, 3},    // 9
          {10, 16},  // 10
          {10, 15},  // 11
          {0, 5},    // 12
          {5, 10},   // 13
          {7, 8},    // 14
          {10, 15},  // 15
          {11, 15},  // 16
        };
        public int VehicleNumber = 4;
        public int Depot = 0;

        public void setTimeMatrix(long[,] newTimeMatrix)
        {
            TimeMatrix = newTimeMatrix;
        }
    };

    /// <summary>
    ///   Print the solution.
    /// </summary>
    static void PrintSolution(
        in DataModel data,
        in RoutingModel routing,
        in RoutingIndexManager manager,
        in Assignment solution)
    {
        RoutingDimension timeDimension = routing.GetMutableDimension("Time");
        // Inspect solution.
        long totalTime = 0;
        for (int i = 0; i < data.VehicleNumber; ++i)
        {
            Console.WriteLine("Route for Vehicle {0}:", i);
            var index = routing.Start(i);
            while (routing.IsEnd(index) == false)
            {
                var timeVar = timeDimension.CumulVar(index);
                Console.Write("{0} Time({1},{2}) -> ",
                    manager.IndexToNode(index),
                    solution.Min(timeVar),
                    solution.Max(timeVar));
                index = solution.Value(routing.NextVar(index));
            }
            var endTimeVar = timeDimension.CumulVar(index);
            Console.WriteLine("{0} Time({1},{2})",
                manager.IndexToNode(index),
                solution.Min(endTimeVar),
                solution.Max(endTimeVar));
            Console.WriteLine("Time of the route: {0}min", solution.Min(endTimeVar));
            totalTime += solution.Min(endTimeVar);
        }
        Console.WriteLine("Total time of all routes: {0}min", totalTime);
    }

    public static void Main(String[] args)
    {
        List<string> addresses = new List<string>
        {
            "3610+Hacks+Cross+Rd+Memphis+TN", // depot
                       "1921+Elvis+Presley+Blvd+Memphis+TN",
                       "149+Union+Avenue+Memphis+TN",
                       "1034+Audubon+Drive+Memphis+TN",
                       "1532+Madison+Ave+Memphis+TN",
                       "706+Union+Ave+Memphis+TN",
                       "3641+Central+Ave+Memphis+TN",
                       "926+E+McLemore+Ave+Memphis+TN",
                       "4339+Park+Ave+Memphis+TN",
                       "600+Goodwyn+St+Memphis+TN",
                       "2000+North+Pkwy+Memphis+TN",
                       "262+Danny+Thomas+Pl+Memphis+TN",
                       "125+N+Front+St+Memphis+TN",
                       "5959+Park+Ave+Memphis+TN",
                       "814+Scott+St+Memphis+TN",
                       "1005+Tillman+St+Memphis+TN"
        };

        string apiKey = "AIzaSyBtSc0dRb4m4S6TBfnft52euaAr0qQt1Ls"; //TODO make env variable
        
        
        // Instantiate the data problem.
        DataModel data = new DataModel(); //TODO pass addresses and apiKey into dataModel instance

        data.setTimeMatrix(create_time_matrix(addresses, apiKey));

        // Create Routing Index Manager
        RoutingIndexManager manager = new RoutingIndexManager(
            data.TimeMatrix.GetLength(0),
            data.VehicleNumber,
            data.Depot);

        // Create Routing Model.
        RoutingModel routing = new RoutingModel(manager);

        // Create and register a transit callback.
        int transitCallbackIndex = routing.RegisterTransitCallback(
            (long fromIndex, long toIndex) => {
            // Convert from routing variable Index to distance matrix NodeIndex.
            var fromNode = manager.IndexToNode(fromIndex);
                var toNode = manager.IndexToNode(toIndex);
                return data.TimeMatrix[fromNode, toNode];
            }
            );

        // Define cost of each arc.
        routing.SetArcCostEvaluatorOfAllVehicles(transitCallbackIndex);

        // Add Distance constraint.
        routing.AddDimension(
            transitCallbackIndex, // transit callback
            30, // allow waiting time
            30, // vehicle maximum capacities
            false,  // start cumul to zero
            "Time");
        RoutingDimension timeDimension = routing.GetMutableDimension("Time");
        // Add time window constraints for each location except depot.
        for (int i = 1; i < data.TimeWindows.GetLength(0); ++i)
        {
            long index = manager.NodeToIndex(i);
            timeDimension.CumulVar(index).SetRange(
                data.TimeWindows[i, 0],
                data.TimeWindows[i, 1]);
        }
        // Add time window constraints for each vehicle start node.
        for (int i = 0; i < data.VehicleNumber; ++i)
        {
            long index = routing.Start(i);
            timeDimension.CumulVar(index).SetRange(
                data.TimeWindows[0, 0],
                data.TimeWindows[0, 1]);
        }

        // Instantiate route start and end times to produce feasible times.
        for (int i = 0; i < data.VehicleNumber; ++i)
        {
            routing.AddVariableMinimizedByFinalizer(
                timeDimension.CumulVar(routing.Start(i)));
            routing.AddVariableMinimizedByFinalizer(
                timeDimension.CumulVar(routing.End(i)));
        }

        // Setting first solution heuristic.
        RoutingSearchParameters searchParameters =
          operations_research_constraint_solver.DefaultRoutingSearchParameters();
        searchParameters.FirstSolutionStrategy =
          FirstSolutionStrategy.Types.Value.PathCheapestArc;

        // Solve the problem.
        Assignment solution = routing.SolveWithParameters(searchParameters);

        // Print solution on console.
        PrintSolution(data, routing, manager, solution);
    }

    public long[,] create_time_matrix(List<string> addresses, string API_key)
    {
        // Distance Matrix API only accepts 100 elements per request, so get rows in multiple requests.
        int max_elements = 100;
        int num_addresses = addresses.Count;
        // Maximum number of rows that can be computed per request
        int max_rows = max_elements; // num_addresses
        // num_addresses = q * max_rows + r (q = 2 and r = 4 in this example).
        int r;
        int q = Math.DivRem(num_addresses, max_rows, out r); // is this right?

        List<string> dest_addresses = addresses;
        long[,] distance_matrix = new long[num_addresses, num_addresses];
        // Send q requests, returning max_rows rows per request.
        for(int i = 0; i < q; i++)
        {
            //List<string> origin_addresses = addresses[i * max_rows: (i + 1) * max_rows]
            //string response = send_request(origin_addresses, dest_addresses, API_key)
            //distance_matrix += build_distance_matrix(response)
        }
        return distance_matrix;
    }

    //def send_request(origin_addresses, dest_addresses, API_key):  
    private string send_request(List<string> origin_addresses, List<string> dest_addresses, string API_key)
    { //Build and send request for the given origin and destination addresses.
        string request = 'https://maps.googleapis.com/maps/api/distancematrix/json?units=imperial'
        string origin_address_str = build_address_str(origin_addresses);
        string dest_address_str = build_address_str(dest_addresses);
        request = request + "&origins=" + origin_address_str + "&destinations=" + 
                            dest_address_str + "&key=" + API_key;
        jsonResult = urllib.urlopen(request).read();
        
    }
   
    /*
        request = 'https://maps.googleapis.com/maps/api/distancematrix/json?units=imperial'
        origin_address_str = build_address_str(origin_addresses)
        dest_address_str = build_address_str(dest_addresses)
        request = request + '&origins=' + origin_address_str + '&destinations=' + \
                            dest_address_str + '&key=' + API_key
        jsonResult = urllib.urlopen(request).read()
        response = json.loads(jsonResult)
        return response
    */

    private string build_address_str(List<string> addresses) //# Build a pipe-separated string of addresses
    {
        string address_str = String.Join("|", addresses);
        /*for i in range(len(addresses) - 1):
          address_str += addresses[i] + '|'
        address_str += addresses[-1]*/
        return address_str;
    }
        
     

  
}