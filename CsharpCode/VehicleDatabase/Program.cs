using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace VehicleDatabase
{
    // Enum that defines all possible vehicle body types
    // Перелік типів кузова транспортних засобів
    enum BodyType
    {
        Sedan,
        Coupe,
        Crossover,
        Hatchback,
        Wagon,
        Pickup,
        Van,
        Minivan,
        Motorcycle
    }

    // ===== BASE CLASS =====
    abstract class Vehicle
    {
        public BodyType BodyType { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public double EngineCapacity { get; set; }
        public string Vin { get; set; }
        public int Mileage { get; set; }

        // Polymorphic method
        public virtual void ShowInfo()
        {
            Console.WriteLine(
                $"{BodyType};{Brand};{Model};{Year};{EngineCapacity};{Vin};{Mileage}");
        }
    }

    // ===== CAR =====
    class Car : Vehicle
    {
        public override void ShowInfo()
        {
            Console.WriteLine(
                $"[CAR]  {BodyType};{Brand};{Model};{Year};{EngineCapacity};{Vin};{Mileage}");
        }
    }

    // ===== MOTORCYCLE =====
    class Motorcycle : Vehicle
    {
        public override void ShowInfo()
        {
            Console.WriteLine(
                $"[MOTORCYCLE]  {Brand};{Model};{Year};{EngineCapacity};{Vin};{Mileage}");
        }
    }

    // ===== FILE SERVICE =====
    internal class FileService
    {
        private string GetPath()
        {
            string path = Path.Combine(AppContext.BaseDirectory, "Vehicle.txt");
            return Path.GetFullPath(path);
        }

        // Load vehicles with correct type
        public List<Vehicle> LoadVehicle()
        {
            string path = GetPath();

            if (!File.Exists(path))
                return new List<Vehicle>();

            return File.ReadAllLines(path)
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .Select(line =>
                {
                    // Split line by ';'
                    var p = line.Split(';');

                    // Parse body type
                    // Отримуємо тип кузова
                    BodyType body = Enum.Parse<BodyType>(p[0]);

                    Vehicle vehicle;

                    // Create object based on body type
                    // Створюємо обʼєкт залежно від типу кузова
                    if (body == BodyType.Motorcycle)
                        vehicle = new Motorcycle();
                    else
                        vehicle = new Car();

                    // Assign properties
                    // Присвоюємо властивості
                    vehicle.BodyType = body;
                    vehicle.Brand = p[1];
                    vehicle.Model = p[2];
                    vehicle.Year = int.Parse(p[3]);
                    vehicle.EngineCapacity = double.Parse(p[4]);
                    vehicle.Vin = p[5];
                    vehicle.Mileage = int.Parse(p[6]);

                    return vehicle;
                })
                .ToList();
        }

        // Save
        public void SaveVehicle(List<Vehicle> vehicles)
        {
            string path = GetPath();

            var lines = vehicles.Select(v =>
                $"{v.BodyType};{v.Brand};{v.Model};{v.Year};{v.EngineCapacity};{v.Vin};{v.Mileage}");

            File.WriteAllLines(path, lines);
        }

        // Show all
        public void ShowAllVehicle()
        {
            var vehicles = LoadVehicle();

            if (vehicles.Count == 0)
            {
                Console.WriteLine("No vehicles in database.");
                return;
            }

            foreach (var v in vehicles)
                v.ShowInfo();
        }

        // Show only motorcycles 
        public void ShowOnlyMotorcycles()
        {
            var vehicles = LoadVehicle()
                .Where(v => v.BodyType == BodyType.Motorcycle)
                .ToList();

            if (vehicles.Count == 0)
            {
                Console.WriteLine("No motorcycles in database.");
                return;
            }

            foreach (var v in vehicles)
                v.ShowInfo();
        }

        // Show by body type
        public void ShowByBodyType()
        {
            var vehicles = LoadVehicle();

            Console.WriteLine("Enter body type (Sedan, Coupe, Crossover, Hatchback, Wagon, Pickup, Van, Minivan): ");


            string input = Console.ReadLine();

            // Validate input
            // Перевіряємо правильність введення
            if (!Enum.TryParse(input, true, out BodyType bodyType))
            {
                Console.WriteLine("Invalid body type!");
                return;
            }

            var filtered = vehicles.Where(v => v.BodyType == bodyType).ToList();

            if (filtered.Count == 0)
            {
                Console.WriteLine("No vehicles of this type.");
                return;
            }

            foreach (var v in filtered)
                v.ShowInfo();
        }

        // Add vehicle
        public void AddVehicle()
        {
            var vehicles = LoadVehicle();

            Console.WriteLine("VIN: ");
            string vin = Console.ReadLine();

            // Check VIN uniqueness
            // Перевірка унікальності VIN
            if (vehicles.Any(v => v.Vin == vin))
            {
                Console.WriteLine("Vehicle with this VIN already exists!");
                return;
            }

            Console.WriteLine("Enter body type (Sedan, Coupe, Crossover, Hatchback, Wagon, Pickup, Van, Minivan, Motorcycle): ");

            BodyType body = Enum.Parse<BodyType>(Console.ReadLine());

            Vehicle newVehicle;

            // Create correct object type
            if (body == BodyType.Motorcycle)
                newVehicle = new Motorcycle();
            else
                newVehicle = new Car();

            newVehicle.BodyType = body;

            Console.WriteLine("Brand: ");
            newVehicle.Brand = Console.ReadLine();

            Console.WriteLine("Model: ");
            newVehicle.Model = Console.ReadLine();

            Console.WriteLine("Year: ");
            newVehicle.Year = int.Parse(Console.ReadLine());

            Console.WriteLine("Engine capacity: ");
            newVehicle.EngineCapacity = double.Parse(Console.ReadLine());

            Console.WriteLine("Mileage: ");
            newVehicle.Mileage = int.Parse(Console.ReadLine());

            newVehicle.Vin = vin;

            vehicles.Add(newVehicle);
            SaveVehicle(vehicles);

            Console.WriteLine("Vehicle added.");
        }

        // Delete
        public void DeleteVehicle()
        {
            var vehicles = LoadVehicle();

            Console.WriteLine("Enter VIN to delete vehicle: ");
            string vin = Console.ReadLine();

            var vehicle = vehicles.FirstOrDefault(v => v.Vin == vin);

            if (vehicle == null)
            {
                Console.WriteLine("Vehicle not found.");
                return;
            }

            vehicles.Remove(vehicle);
            SaveVehicle(vehicles);

            Console.WriteLine("Vehicle deleted.");
        }

        // Edit
        public void EditVehicle()
        {
            var vehicles = LoadVehicle();

            Console.WriteLine("Enter VIN to edit vehicle: ");
            string vin = Console.ReadLine();

            var vehicle = vehicles.FirstOrDefault(v => v.Vin == vin);

            if (vehicle == null)
            {
                Console.WriteLine("Vehicle not found.");
                return;
            }

            Console.WriteLine($"BodyType ({vehicle.BodyType}): ");
            string body = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(body))
                vehicle.BodyType = Enum.Parse<BodyType>(body);

            Console.WriteLine($"Brand ({vehicle.Brand}): ");
            string brand = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(brand))
                vehicle.Brand = brand;

            Console.WriteLine($"Model ({vehicle.Model}): ");
            string model = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(model))
                vehicle.Model = model;

            Console.WriteLine($"Year ({vehicle.Year}): ");
            string yearStr = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(yearStr))
                vehicle.Year = int.Parse(yearStr);

            Console.WriteLine($"Engine capacity ({vehicle.EngineCapacity}): ");
            string engineCapacityStr = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(engineCapacityStr))
                vehicle.EngineCapacity = double.Parse(engineCapacityStr);

            Console.WriteLine($"Mileage ({vehicle.Mileage}): ");
            string mileageStr = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(mileageStr))
                vehicle.Mileage = int.Parse(mileageStr);

            SaveVehicle(vehicles);
            Console.WriteLine("Vehicle updated.");
        }

        // ===== SORTING =====

        //Sort by brand
        public void SortByBrand()
        {
            var vehicles = LoadVehicle()
                .OrderBy(v => v.Brand, StringComparer.OrdinalIgnoreCase)
                .ToList();

            var brands = vehicles
                .Select(v => v.Brand)
                .Distinct()
                .OrderBy(b => b)
                .ToList();

            Console.WriteLine("\nAvailable brands:\n");

            int count = 0;
            foreach (var b in brands)
            {
                Console.Write($"{b,-15}");
                count++;

                if (count % 4 == 0)
                    Console.WriteLine();
            }

            Console.WriteLine("\n\nSelect which brand of vehicle you want to see: ");

            string input = Console.ReadLine();

            var filtered = vehicles
                .Where(v => v.Brand.Equals(input, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (filtered.Count == 0)
            {
                Console.WriteLine("There are no vehicles of this brand in the database.");
                return;
            }

            foreach (var v in filtered)
                v.ShowInfo();
        }

        //Sort by year
        public void ShowByYear()
        {
            var vehicles = LoadVehicle();

            Console.WriteLine("Select the year of the vehicle you want to see (1982 - 2013): ");

            string input = Console.ReadLine();

            if (!int.TryParse(input, out int year))
            {
                Console.WriteLine("Invalid input!");
                return;
            }

            var filtered = vehicles.Where(v => v.Year == year)
                .ToList();

            if (filtered.Count == 0)
            {
                Console.WriteLine("There are no vehicles manufactured this year in the database.");
                return;
            }

            foreach (var v in filtered)
                v.ShowInfo();
        }
    }

    // ===== MAIN =====
    class Program
    {
        static void Main()
        {
            var fileService = new FileService();

            while (true)
            {
                Console.WriteLine("\n==== VEHICLE DATABASE ====");
                Console.WriteLine("1. Show all vehicles");
                Console.WriteLine("2. Show by body type");
                Console.WriteLine("3. Show only motorcycles");
                Console.WriteLine("4. Add vehicle");
                Console.WriteLine("5. Delete vehicle");
                Console.WriteLine("6. Edit vehicle");
                Console.WriteLine("7. Sort by brand");
                Console.WriteLine("8. Sort by year");
                Console.WriteLine("9. Exit");

                Console.WriteLine("Please select your choice: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        fileService.ShowAllVehicle();
                        break;
                    case "2":
                        fileService.ShowByBodyType();
                        break;
                    case "3":
                        fileService.ShowOnlyMotorcycles();
                        break;
                    case "4":
                        fileService.AddVehicle();
                        break;
                    case "5":
                        fileService.DeleteVehicle();
                        break;
                    case "6":
                        fileService.EditVehicle();
                        break;
                    case "7":
                        fileService.SortByBrand();
                        break;
                    case "8":
                        fileService.ShowByYear();
                        break;
                    case "9":
                        return;
                    default:
                        Console.WriteLine("Invalid option!");
                        break;
                }
            }
        }
    }
}
 