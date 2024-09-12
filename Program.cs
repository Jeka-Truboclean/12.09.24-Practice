using Microsoft.Data.SqlClient;

namespace _12._09._24_Practice
{
    internal class Program
    {
        static string connectionString = "Server=(localdb)\\ProjectModels;Database=CarsDb;Trusted_Connection=true";

        static void Main(string[] args)
        {
            CreateDatabase();
            CreateTable();

            AddCarsToTable();

            var allCars = GetAllCars();

            Console.WriteLine("Cars: ");
            foreach (var car in allCars)
            {
                Console.WriteLine($"{car.Id}: {car.Model}, {car.Year}");
            }

            var carsYoungerThan2018 = GetCarsYoungerThan2018(allCars);

            Console.WriteLine("\nCars made before 2018: ");
            foreach (var car in carsYoungerThan2018)
            {
                Console.WriteLine($"{car.Id}: {car.Model}, {car.Year}");
            }
        }

        public class Car
        {
            public int Id { get; set; }
            public string? Model { get; set; }
            public int Year { get; set; }
        }

        private static void CreateDatabase()
        {
            using (SqlConnection connection = new SqlConnection("Server=(localdb)\\ProjectModels;Database=master;Trusted_Connection=true"))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("CREATE DATABASE [CarsDb];", connection);
                command.ExecuteNonQuery();
            }
        }

        private static void CreateTable()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("CREATE TABLE Cars (Id INT PRIMARY KEY IDENTITY, Model NVARCHAR(100), Year INT);",
                    connection);
                command.ExecuteNonQuery();
            }
        }

        private static void AddCarsToTable()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var cars = new List<Car>
                {
                    new Car { Model = "Toyota Camry", Year = 2019 },
                    new Car { Model = "Honda Civic", Year = 2017 },
                    new Car { Model = "Ford Focus", Year = 2016 },
                    new Car { Model = "BMW 3 Series", Year = 2020 },
                    new Car { Model = "Audi A4", Year = 2015 }
                };

                foreach (var car in cars)
                {
                    SqlCommand command = new SqlCommand("INSERT INTO Cars (Model, Year) VALUES (@Model, @Year);", connection);
                    command.Parameters.AddWithValue("@Model", car.Model);
                    command.Parameters.AddWithValue("@Year", car.Year);
                    command.ExecuteNonQuery();
                }
            }
        }

        private static List<Car> GetAllCars()
        {
            var cars = new List<Car>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("SELECT * FROM Cars;", connection);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        cars.Add(new Car
                        {
                            Id = reader.GetInt32(0),
                            Model = reader.GetString(1),
                            Year = reader.GetInt32(2)
                        });
                    }
                }
            }
            return cars;
        }

        private static List<Car> GetCarsYoungerThan2018(List<Car> allCars)
        {
            return allCars.FindAll(car => car.Year < 2018);
        }
    }
}
