using System.Xml.Linq;

namespace LandXMLValidationChecker;

class Surface
{
    public int PointCount { get; set; }
    public string? Name { get; set; }
}

class SurfaceGroup
{
    public string? Name { get; set; }
    public List<Surface> SurfaceCollection { get; set; } = [];
}

class SurfaceSummary
{
    public List<SurfaceGroup> SurfaceGroups { get; set; } = [];
}

class Program
{
    const int TheoreticalPointLimit = 160_000_000;
    const int TheoreticalSurfaceLimit = 850;
    const int TheoreticalSurfaceGroupPointLimit = 16_000_000;

    static void Main(string[] args)
    {
        if (args.Length == 0 || !File.Exists(args[0]))
        {
            Console.WriteLine("Usage: LandXMLValidationChecker.exe <input.xml> [--summary|-s] [--check|-c]");
            return;
        }

        string filePath = args[0];
        bool showSummary = args.Contains("--summary") || args.Contains("-s");
        bool doCheck = args.Contains("--check") || args.Contains("-c");

        SurfaceSummary surfaces = LoadSurfaces(filePath);

        if (showSummary)
            PrintSummary(surfaces);

        if (doCheck)
            CheckLimits(surfaces);
    }

    static SurfaceSummary LoadSurfaces(string xmlPath)
    {
        var doc = XDocument.Load(xmlPath);
        var surfaceGroupsXE = doc.Descendants().Where(x => x.Name.LocalName == "Surfaces");

        List<SurfaceGroup> surfaceGroups = new();

        foreach (var groupXE in surfaceGroupsXE)
        {
            var groupName = groupXE.Attribute("name")?.Value ?? "UnnamedGroup";
            var surfaceListXE = groupXE.Descendants().Where(x => x.Name.LocalName == "Surface");

            var surfaces = surfaceListXE.Select(surface =>
            {
                var name = surface.Attribute("name")?.Value ?? "UnnamedSurface";
                int pointCount = surface.Descendants().Count(x => x.Name.LocalName == "P");
                return new Surface { Name = name, PointCount = pointCount };
            }).ToList();

            surfaceGroups.Add(new SurfaceGroup { Name = groupName, SurfaceCollection = surfaces });
        }

        return new SurfaceSummary { SurfaceGroups = surfaceGroups };
    }

    static void PrintSummary(SurfaceSummary summary)
    {
        Console.WriteLine("\n=== Surface Summary ===");

        int totalSurfaces = 0;
        int totalPoints = 0;

        foreach (var group in summary.SurfaceGroups)
        {
            int groupPoints = group.SurfaceCollection.Sum(s => s.PointCount);
            int groupSurfaces = group.SurfaceCollection.Count;

            totalSurfaces += groupSurfaces;
            totalPoints += groupPoints;

            Console.WriteLine($"\nGroup: {group.Name}");
            Console.WriteLine($"  Number of Surfaces: {groupSurfaces}");
            Console.WriteLine($"  Total Points:        {groupPoints}");
        }

        Console.WriteLine("\n----------------------------");
        Console.WriteLine($"Total Surfaces: {totalSurfaces}");
        Console.WriteLine($"Total Points:   {totalPoints}");
    }

    static void CheckLimits(SurfaceSummary summary)
    {
        int totalPoints = summary.SurfaceGroups.Sum(g => g.SurfaceCollection.Sum(s => s.PointCount));
        int totalSurfaces = summary.SurfaceGroups.Sum(g => g.SurfaceCollection.Count);

        Console.WriteLine("\n=== Limit Checks ===");

        // Check overall point limit
      
        if (totalPoints > TheoreticalPointLimit)
        {
            Console.Write("❌ Total Points: ");
            Warn($"Exceeded! ({totalPoints} > {TheoreticalPointLimit})");
        }
        else if (totalPoints == 0)
        {
            Console.Write("❌ Total Points: ");
            Warn($"No points found!");
        }
        else if (totalPoints < 0)
        {
            Console.Write("❌ Total Points: ");
            Warn($"Negative point count! ({totalPoints})");
        }
        else
        {
            Console.Write("✔ Total Points: ");
            Pass($"OK ({totalPoints} ≤ {TheoreticalPointLimit})");
        }

        // Check surface count limit

        if (totalSurfaces > TheoreticalSurfaceLimit) {
            Console.Write("❌ Total Surfaces: ");
            Warn($"Exceeded! ({totalSurfaces} > {TheoreticalSurfaceLimit})");
        }
        else if (totalSurfaces == 0)
        {
            Console.Write("❌ Total Surfaces: ");
            Warn($"No surfaces found!");
        }
        else if (totalSurfaces < 0)
        {
            Console.Write("❌ Total Surfaces: ");
            Warn($"Negative surface count! ({totalSurfaces})");
        }
        else
        {
            Console.Write("✔ Total Surfaces: ");
            Pass($"OK ({totalSurfaces} ≤ {TheoreticalSurfaceLimit})");
        }
          

        // Check group-wise point limit
        foreach (var group in summary.SurfaceGroups)
        {
            int groupPoints = group.SurfaceCollection.Sum(s => s.PointCount);
           
            if (groupPoints > TheoreticalSurfaceGroupPointLimit)
            {
                Console.Write($"❌ {group.Name}: ");
                Warn($"Exceeded! ({groupPoints} > {TheoreticalSurfaceGroupPointLimit})");
            }
            else if (groupPoints == 0)
            {
                Console.Write($"❌ {group.Name}: ");
                Warn($"No points found!");
            }
            else if (groupPoints < 0)
            {
                Console.Write($"❌ {group.Name}: ");
                Warn($"Negative point count! ({groupPoints})");
            }             
            else
            {
                Console.Write($"✔ {group.Name}: ");
                Pass($"OK ({groupPoints} ≤ {TheoreticalSurfaceGroupPointLimit})");
            }                
        }

        Console.WriteLine("\nAll limits checked.");
    }

    static void Warn(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("❌ " + message);
        Console.ResetColor();
    }

    static void Pass(string message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("✅ " + message);
        Console.ResetColor();
    }
}
