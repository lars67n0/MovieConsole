using System;
using System.Data.SqlClient;

namespace MovieConsole
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // CONNECTION STRING
            string connectionString = "Data Source=LRF;Initial Catalog=MovieDB;Integrated Security=True";

            bool exit = false;

            do
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        Console.WriteLine("Connected to the database.\n");
                        Console.WriteLine();
                        Console.WriteLine("────────────────────────────────────────");
                        Console.WriteLine("Welcome To Hullu Movie Database");
                        Console.WriteLine("Please Follow The Instructions Below");
                        Console.WriteLine("────────────────────────────────────────");
                        Console.WriteLine();

                        Console.WriteLine("Please enter the number for the action:\n");

                        Console.WriteLine("┌────────────────────────────────────────────────┐");
                        Console.WriteLine("│ 1. Retrieve First X entries from NameBasics    │");
                        Console.WriteLine("├────────────────────────────────────────────────┤");
                        Console.WriteLine("│ 2. Retrieve First X entries from TitleBasics   │");
                        Console.WriteLine("├────────────────────────────────────────────────┤");
                        Console.WriteLine("│ 3. Retrieve First X entries from TitleCrew     │");
                        Console.WriteLine("├────────────────────────────────────────────────┤");
                        Console.WriteLine("│ 4. Search For A Movie Title                    │");
                        Console.WriteLine("├────────────────────────────────────────────────┤");
                        Console.WriteLine("│ 5. Add a new Movie to TitleBasics              │");
                        Console.WriteLine("├────────────────────────────────────────────────┤");
                        Console.WriteLine("│ 6. Add a new Person to NameBasics              │");
                        Console.WriteLine("├────────────────────────────────────────────────┤");
                        Console.WriteLine("│ 7. Delete a movie from TitleBasics             │");
                        Console.WriteLine("├────────────────────────────────────────────────┤");
                        Console.WriteLine("│ 8. Exit                                        │");
                        Console.WriteLine("└────────────────────────────────────────────────┘");




                        if (int.TryParse(Console.ReadLine(), out int choice))
                        {
                            switch (choice)
                            {
                                case 1:
                                    RetrieveData("NameBasics", connection);
                                    break;
                                case 2:
                                    RetrieveData("TitleBasics", connection);
                                    break;
                                case 3:
                                    RetrieveData("TitleCrew", connection);
                                    break;
                                case 4:
                                    Console.WriteLine("Enter a search string for the TitleBasics table:");
                                    string searchQuery = Console.ReadLine();
                                    SearchTitleBasicsByTitle(connection, searchQuery);
                                    break;
                                case 5:
                                    AddNewTitle(connection);
                                    break;
                                case 6:
                                    AddNewPerson(connection);
                                    break;
                                case 7:
                                    DeleteMovieByTitle(connection);
                                    break;
                                case 8:
                                    exit = true;
                                    break;
                                default:
                                    Console.WriteLine("Invalid choice.");
                                    break;
                            }
                        }
                        else
                        {
                            Console.WriteLine("Invalid input.");
                        }

                        connection.Close();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }

            } while (!exit);

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }

        // READ METHOD
        // RETRIEVES THE AMOUNT OF ENTRIES OF 'X' TABLE SET BY THE USER INPUT

        static void RetrieveData(string tableName, SqlConnection connection)
        {
            Console.WriteLine("Enter the amount of entries u wish to retrieve from the database Table");

            string input = Console.ReadLine();
            
            string query = $"SELECT TOP {input} * FROM {tableName};";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    Console.WriteLine($"Data from table: {tableName}");
                    while (reader.Read())
                    {
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            Console.WriteLine($"{reader.GetName(i)}: {reader.GetValue(i)}");
                        }
                        Console.WriteLine();
                    }
                }
            }
        }

        // WILD CARD SEARCH METHOD
        // SEARCHES FOR MATCHING 'PrimaryTitle' IN 'TitleBasics' TABLE

        static void SearchTitleBasicsByTitle(SqlConnection connection, string searchQuery)
        {
            string query = "SELECT TOP 5 * FROM TitleBasics WHERE [PrimaryTitle] LIKE @searchQuery ORDER BY [PrimaryTitle];";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@searchQuery", "%" + searchQuery + "%");

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    Console.WriteLine($"Search results from TitleBasics table for '{searchQuery}' in alphabetical order:");
                    while (reader.Read())
                    {
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            Console.WriteLine($"{reader.GetName(i)}: {reader.GetValue(i)}");
                        }
                        Console.WriteLine();
                    }
                }
            }
        }


        // ADD METHOD 
        // ADD A NEW TITLE IN 'TitleBasics' TABLE
        static void AddNewTitle(SqlConnection connection)
        {
            Console.WriteLine("Enter the TConst (primary key):");
            string tConst = Console.ReadLine();

            Console.WriteLine("Enter the Primary Title:");
            string primaryTitle = Console.ReadLine();

            Console.WriteLine("Enter the Title Type:");
            string titleType = Console.ReadLine();

            // Insert the values into the TitleBasics table
            string query = "INSERT INTO TitleBasics (TConst, [PrimaryTitle], TitleType) VALUES (@tConst, @primaryTitle, @titleType);";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@tConst", tConst);
                command.Parameters.AddWithValue("@primaryTitle", primaryTitle);
                command.Parameters.AddWithValue("@titleType", titleType);

                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    Console.WriteLine("New title added to TitleBasics table.");
                }
                else
                {
                    Console.WriteLine("Failed to add a new title.");
                }
            }
        }

        // ADD METHOD
        // ADD A NEW PERSON IN 'NameBasics' TABLE
        static void AddNewPerson(SqlConnection connection)
        {
            Console.WriteLine("Enter the NConst (primary key):");
            string nConst = Console.ReadLine();

            Console.WriteLine("Enter the Primary Name:");
            string primaryName = Console.ReadLine();

            Console.WriteLine("Enter the Known For Titles:");
            string knownForTitles = Console.ReadLine();

            // Insert the values into the NameBasics table
            string query = "INSERT INTO NameBasics (NConst, [PrimaryName], KnownForTitles) VALUES (@nConst, @primaryName, @knownForTitles);";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@nConst", nConst);
                command.Parameters.AddWithValue("@primaryName", primaryName);
                command.Parameters.AddWithValue("@knownForTitles", knownForTitles);

                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    Console.WriteLine("New person added to NameBasics table.");
                }
                else
                {
                    Console.WriteLine("Failed to add a new person.");
                }
            }
        }

        // DELETE METHOD
        // DELETES A ROW FROM 'TitleBasics' WITH SAME PRIMARY TITLE AS INPUT
        static void DeleteMovieByTitle(SqlConnection connection)
        {
            Console.WriteLine("Enter the Primary Title of the movie to delete:");
            string primaryTitle = Console.ReadLine();

            // Delete the movie from the TitleBasics table based on PrimaryTitle
            string query = "DELETE FROM TitleBasics WHERE [PrimaryTitle] = @primaryTitle;";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@primaryTitle", primaryTitle);

                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    Console.WriteLine("Movie deleted from TitleBasics table.");
                }
                else
                {
                    Console.WriteLine("Failed to delete the movie.");
                }
            }
        }

    }
}
