using System;
using System.Collections.Generic;
using System.Text;

class Program
{
    private static List<ContainerShip> containerShips = new List<ContainerShip>();
    private static List<Container> containers = new List<Container>();

    static void Main(string[] args)
    {
        bool exit = false;
        while (!exit)
        {
            Console.WriteLine("List of container ships: " + (containerShips.Count == 0 ? "None" : ""));
            foreach (var ship in containerShips)
            {
                Console.WriteLine($"Ship {ship.SerialNumber} (speed={ship.MaxSpeed}, maxContainerNum={ship.MaxContainerCapacity}, maxWeight={ship.MaxWeightCapacity})");
            }

            Console.WriteLine("List of containers: " + (containers.Count == 0 ? "None" : ""));
            foreach (var container in containers)
            {
                Console.WriteLine($"Container {container.SerialNumber}: {container.GetType().Name}, Cargo Mass = {container.CargoMass} kg");
            }

            Console.WriteLine("\nPossible actions:");
            Console.WriteLine("1. Add a container ship");
            Console.WriteLine("2. Remove a container ship");
            Console.WriteLine("3. Add a container");
            Console.WriteLine("4. Load a container onto a ship");
            Console.WriteLine("5. Exit");

            Console.Write("Select an action: ");
            var action = Console.ReadLine();

            switch (action)
            {
                case "1":
                    AddContainerShip();
                    break;
                case "2":
                    RemoveContainerShip();
                    break;
                case "3":
                    AddContainer();
                    break;
                case "4":
                    LoadContainerOntoShip();
                    break;
                case "5":
                    exit = true;
                    break;
                default:
                    Console.WriteLine("Invalid action.");
                    break;
            }
        }
    }

    private static void AddContainerShip()
    {
        Console.Write("Enter ship max speed: ");
        double maxSpeed = Convert.ToDouble(Console.ReadLine());

        Console.Write("Enter max number of containers: ");
        int maxContainerCapacity = Convert.ToInt32(Console.ReadLine());

        Console.Write("Enter max weight capacity: ");
        double maxWeightCapacity = Convert.ToDouble(Console.ReadLine());

        var ship = new ContainerShip(maxSpeed, maxContainerCapacity, maxWeightCapacity);
        containerShips.Add(ship);

        Console.WriteLine("Container ship added.");
    }

    private static void RemoveContainerShip()
    {
        Console.Write("Enter container ship serial number to remove: ");
        string serialNumber = Console.ReadLine();

        var ship = containerShips.Find(s => s.SerialNumber == serialNumber);
        if (ship != null)
        {
            containerShips.Remove(ship);
            Console.WriteLine("Container ship removed.");
        }
        else
        {
            Console.WriteLine("Container ship not found.");
        }
    }

    private static void AddContainer()
    {
        Console.WriteLine("Select container type:");
        Console.WriteLine("1. Liquid Container");
        Console.WriteLine("2. Gas Container");
        Console.WriteLine("3. Refrigerated Container");
        string type = Console.ReadLine();

        double height = 200, tareWeight = 100, depth = 200, maxPayload = 1000;
        bool isHazardous = false;
        double pressure = 1.0;
        string storedProductType = "";
        double temperature = 0;

        Container container = null;

        switch (type)
        {
            case "1":
                Console.Write("Is it hazardous? (true/false): ");
                isHazardous = Convert.ToBoolean(Console.ReadLine());
                container = new LiquidContainer(height, tareWeight, depth, maxPayload, isHazardous);
                break;
            case "2":
                Console.Write("Enter pressure: ");
                pressure = Convert.ToDouble(Console.ReadLine());
                container = new GasContainer(height, tareWeight, depth, maxPayload, pressure);
                break;
            case "3":
                Console.Write("Enter stored product type: ");
                storedProductType = Console.ReadLine();
                Console.Write("Enter temperature: ");
                temperature = Convert.ToDouble(Console.ReadLine());
                container = new RefrigeratedContainer(height, tareWeight, depth, maxPayload, storedProductType, temperature);
                break;
            default:
                Console.WriteLine("Invalid container type.");
                return;
        }

        containers.Add(container);
        Console.WriteLine("Container added.");
    }
    public class GasContainer : Container, IHazardNotifier
    {
        public double Pressure { get; private set; }

        public GasContainer(double height, double tareWeight, double depth, double maxPayload, double pressure)
            : base(height, tareWeight, depth, maxPayload)
        {
            Pressure = pressure;
        }

        public override void LoadCargo(double mass)
        {
            if (mass > MaxPayload)
            {
                NotifyHazard($"Attempting to overload: {SerialNumber}");
                throw new OverfillException($"Attempted to overload container {SerialNumber}.");
            }
            CargoMass = mass;
        }

        public override void EmptyCargo()
        {
            CargoMass = CargoMass * 0.05;
        }

        public void NotifyHazard(string message)
        {
            Console.WriteLine($"Hazard Notification for {SerialNumber}: {message}");
        }
    }
    public class RefrigeratedContainer : Container
    {
        public string StoredProductType { get; private set; }
        public double Temperature { get; private set; }

        public RefrigeratedContainer(double height, double tareWeight, double depth, double maxPayload, string storedProductType, double temperature)
            : base(height, tareWeight, depth, maxPayload)
        {
            StoredProductType = storedProductType;
            Temperature = temperature;
        }

        public override void LoadCargo(double mass)
        {
            if (mass > MaxPayload)
            {
                throw new OverfillException($"Attempted to overload container {SerialNumber}.");
            }
            CargoMass = mass;
        }

        public override void EmptyCargo()
        {
            CargoMass = 0;
        }
    }
    public class LiquidContainer : Container, IHazardNotifier
    {
        public bool IsHazardous { get; private set; }

        public LiquidContainer(double height, double tareWeight, double depth, double maxPayload, bool isHazardous)
            : base(height, tareWeight, depth, maxPayload)
        {
            IsHazardous = isHazardous;
        }

        public override void LoadCargo(double mass)
        {
            double allowedCapacity = IsHazardous ? MaxPayload * 0.5 : MaxPayload * 0.9;
            if (mass > allowedCapacity)
            {
                NotifyHazard($"Attempting to overload: {SerialNumber}");
                throw new OverfillException($"Attempted to overload container {SerialNumber}.");
            }
            CargoMass = mass;
        }

        public override void EmptyCargo()
        {
            CargoMass = 0;
        }

        public void NotifyHazard(string message)
        {
            Console.WriteLine($"Hazard Notification for {SerialNumber}: {message}");
        }
    }

    private static void LoadContainerOntoShip()
    {
        Console.Write("Enter container serial number to load: ");
        string containerSerialNumber = Console.ReadLine();
        Container container = containers.Find(c => c.SerialNumber == containerSerialNumber);
        if (container == null)
        {
            Console.WriteLine("Container not found.");
            return;
        }

        Console.Write("Enter container ship serial number to load onto: ");
        string shipSerialNumber = Console.ReadLine();
        ContainerShip ship = containerShips.Find(s => s.SerialNumber == shipSerialNumber);
        if (ship == null)
        {
            Console.WriteLine("Container ship not found.");
            return;
        }

        try
        {
            ship.LoadContainer(container);
            Console.WriteLine($"Container {container.SerialNumber} loaded onto ship {ship.SerialNumber}.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to load container: {ex.Message}");
        }
    }
}

public interface IHazardNotifier
{
    void NotifyHazard(string message);
}

public abstract class Container
{
    public double CargoMass { get; protected set; }
    public double Height { get; private set; }
    public double TareWeight { get; private set; }
    public double Depth { get; private set; }
    public string SerialNumber { get; protected set; }
    public double MaxPayload { get; private set; }

    protected Container(double height, double tareWeight, double depth, double maxPayload)
    {
        Height = height;
        TareWeight = tareWeight;
        Depth = depth;
        MaxPayload = maxPayload;
        SerialNumber = SerialNumberGenerator.GenerateSerialNumber(this.GetType().Name.Substring(0, 1));
    }

    public abstract void LoadCargo(double mass);
    public abstract void EmptyCargo();
}

public class LiquidContainer : Container, IHazardNotifier
{
    public bool IsHazardous { get; private set; }

    public LiquidContainer(double height, double tareWeight, double depth, double maxPayload, bool isHazardous)
        : base(height, tareWeight, depth, maxPayload)
    {
        IsHazardous = isHazardous;
    }

    public override void LoadCargo(double mass)
    {
        double allowedCapacity = IsHazardous ? MaxPayload * 0.5 : MaxPayload * 0.9;
        if (mass > allowedCapacity)
        {
            NotifyHazard($"Attempting to overload: {SerialNumber}");
            throw new OverfillException($"Attempted to overload container {SerialNumber}.");
        }
        CargoMass = mass;
    }

    public override void EmptyCargo()
    {
        CargoMass = 0;
    }

    public void NotifyHazard(string message)
    {
        Console.WriteLine($"Hazard Notification for {SerialNumber}: {message}");
    }
}
public class GasContainer : Container, IHazardNotifier
{
    public double Pressure { get; private set; }

    public GasContainer(double height, double tareWeight, double depth, double maxPayload, double pressure)
        : base(height, tareWeight, depth, maxPayload)
    {
        Pressure = pressure;
    }

    public override void LoadCargo(double mass)
    {
        if (mass > MaxPayload)
        {
            NotifyHazard($"Attempting to overload: {SerialNumber}");
            throw new OverfillException($"Attempted to overload container {SerialNumber}.");
        }
        CargoMass = mass;
    }

    public override void EmptyCargo()
    {
        CargoMass = CargoMass * 0.05; // Retain 5%
    }

    public void NotifyHazard(string message)
    {
        Console.WriteLine($"Hazard Notification for {SerialNumber}: {message}");
    }
}

public class RefrigeratedContainer : Container
{
    public string StoredProductType { get; private set; }
    public double Temperature { get; private set; }

    public RefrigeratedContainer(double height, double tareWeight, double depth, double maxPayload, string storedProductType, double temperature)
        : base(height, tareWeight, depth, maxPayload)
    {
        StoredProductType = storedProductType;
        Temperature = temperature;
    }

    public override void LoadCargo(double mass)
    {
        if (mass > MaxPayload)
        {
            throw new OverfillException($"Attempted to overload container {SerialNumber}.");
        }
        CargoMass = mass;
    }

    public override void EmptyCargo()
    {
        CargoMass = 0;
    }
}
public class ContainerShip
{
    public string SerialNumber { get; private set; }
    public double MaxSpeed { get; private set; }
    public int MaxContainerCapacity { get; private set; }
    public double MaxWeightCapacity { get; private set; }
    private List<Container> loadedContainers = new List<Container>();

    public ContainerShip(double maxSpeed, int maxContainerCapacity, double maxWeightCapacity)
    {
        MaxSpeed = maxSpeed;
        MaxContainerCapacity = maxContainerCapacity;
        MaxWeightCapacity = maxWeightCapacity;
        SerialNumber = SerialNumberGenerator.GenerateSerialNumber("S");
    }

    public void LoadContainer(Container container)
    {
        if (loadedContainers.Count >= MaxContainerCapacity)
        {
            throw new InvalidOperationException("Cannot add more containers: capacity reached.");
        }
        loadedContainers.Add(container);
    }
}

public class OverfillException : Exception
{
    public OverfillException(string message) : base(message) { }
}

public static class SerialNumberGenerator
{
    private static Dictionary<string, int> NextNumbers = new Dictionary<string, int>();

    public static string GenerateSerialNumber(string prefix)
    {
        if (!NextNumbers.ContainsKey(prefix))
        {
            NextNumbers[prefix] = 1;
        }
        else
        {
            NextNumbers[prefix]++;
        }
        return $"KON-{prefix}-{NextNumbers[prefix]}";
    }
}