namespace BookTicket
{
    public enum SeatPref
    {
        Window = 1,
        Aisle
    }

    public class Seat
    {
        public string Label { get; }
        public bool IsBooked { get; set; }
        public Passenger Occupant { get; set; }

        public Seat(string label)
        {
            Label = label;
            IsBooked = false;
            Occupant = null;
        }
    }

    public class Passenger
    {
        public string FirstName { get; }
        public string LastName { get; }
        public SeatPref Pref { get; }
        public Seat BookedSeat { get; set; }

        public Passenger(string firstName, string lastName, SeatPref pref)
        {
            FirstName = firstName;
            LastName = lastName;
            Pref = pref;
            BookedSeat = null;
        }
    }

    public class Row
    {
        public List<Seat> Seats { get; }

        public Row(int numSeats)
        {
            Seats = new List<Seat>();
            for (char c = 'A'; c < 'A' + numSeats; c++)
            {
                Seats.Add(new Seat(c.ToString()));
            }
        }
    }

    class Program
    {
        static List<Row> Chart = new List<Row>();
        const string dataFilePath = "D:\\MacEwan\\Intoduction to C# (Assignment)\\Assignment 4\\Assignment 4\\data.txt";

        static void Main(string[] args)
        {
            InitChart();

            string choice;
            while (true)
            {
                Console.WriteLine("Please enter 1 to book a ticket.");
                Console.WriteLine("Please enter 2 to see seating chart.");
                Console.WriteLine("Please enter 3 to exit the application.");

                choice = Console.ReadLine();

                if (choice == "1")
                {
                    Book();
                }
                else if (choice == "2")
                {
                    DisplayChart();
                }
                else if (choice == "3")
                {
                    Environment.Exit(0);
                }
                else
                {
                    Console.WriteLine("Invalid choice. Please enter a valid option.");
                }
            }
        }

        static void InitChart()
        {
            for (int i = 0; i < 12; i++)
            {
                Chart.Add(new Row(4));
            }
        }

        static void Book()
        {
            Console.WriteLine("Please enter the passenger's first name:");
            string fName = Console.ReadLine();

            string lName;
            do
            {
                Console.WriteLine("Please enter the passenger's last name:");
                lName = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(lName))
                {
                    Console.WriteLine("Error: Please enter the last name.");
                }
            } while (string.IsNullOrWhiteSpace(lName));

            Console.WriteLine("Please enter 1 for a Window seat preference, 2 for an Aisle seat preference, or hit enter to pick the first available seat:");
            string prefInput = Console.ReadLine();

            SeatPref pref;
            if (prefInput == "1")
            {
                pref = SeatPref.Window;
            }
            else if (prefInput == "2")
            {
                pref = SeatPref.Aisle;
            }
            else
            {
                pref = SeatPref.Window;
            }

            Passenger p = new Passenger(fName, lName, pref);
            Seat seat = FindSeat(p);

            if (seat != null)
            {
                seat.IsBooked = true;
                seat.Occupant = p;
                p.BookedSeat = seat;
                Console.WriteLine($"The seat located in {seat.Label} has been booked.");

                SavePassengerToFile(p);
            }
            else
            {
                Console.WriteLine("Sorry, the plane is completely booked.");
            }
        }

        static Seat FindSeat(Passenger p)
        {
            foreach (Row row in Chart)
            {
                foreach (Seat seat in row.Seats)
                {
                    if (!seat.IsBooked)
                    {
                        if (p.Pref == SeatPref.Window && (seat.Label == "A" || seat.Label == "D"))
                        {
                            return seat;
                        }
                        else if (p.Pref == SeatPref.Aisle && (seat.Label == "B" || seat.Label == "C"))
                        {
                            return seat;
                        }
                    }
                }
            }
            return null;
        }

        static void SavePassengerToFile(Passenger p)
        {
            using (StreamWriter writer = File.AppendText(dataFilePath))
            {
                writer.WriteLine($"{p.FirstName},{p.LastName},{p.BookedSeat.Label}");
            }
        }

        static void DisplayChart()
        {
            ReadPassengersFromFile();

            foreach (Row row in Chart)
            {
                foreach (Seat seat in row.Seats)
                {
                    if (seat.IsBooked)
                    {
                        Console.Write($"{seat.Occupant.FirstName.Substring(0, 1)}{seat.Occupant.LastName.Substring(0, 1)} ");
                    }
                    else
                    {
                        Console.Write($"{seat.Label} ");
                    }
                }
                Console.WriteLine();
            }
        }

        static void ReadPassengersFromFile()

        {
            if (!File.Exists(dataFilePath))
                return;

            string[] lines = File.ReadAllLines(dataFilePath);

            foreach (string line in lines)
            {
                string[] parts = line.Split(',');
                string firstName = parts[0];
                string lastName = parts[1];
                string seatLabel = parts[2];

                Seat seat = FindSeatByLabel(seatLabel);
                if (seat != null)
                {
                    Passenger passenger = new Passenger(firstName, lastName, SeatPref.Window);
                    seat.IsBooked = true;
                    seat.Occupant = passenger;
                    passenger.BookedSeat = seat;
                }
            }
        }

        static Seat FindSeatByLabel(string label)
        {
            foreach (Row row in Chart)
            {
                foreach (Seat seat in row.Seats)
                {
                    if (seat.Label == label)
                    {
                        return seat;
                    }
                }
            }
            return null;
        }
    }
}
