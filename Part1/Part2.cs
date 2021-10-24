using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Part2
{
    class EmployeeRecord
    {
        public int ID { get; init; }
        public string Name { get; init; }
        public string StateCode { get; init; }
        public double HoursWorkedInTheYear { get; init; }

        public decimal HourlyRate { get; set; }


        // this property was not requested in the problem statement, but when trying to sort in part 3, 
        // it was discovered that this property needed to be added in order to easily sort on the total yearly pay
        // strangely enough, you can not multiply decimal and double, so you have to cast one to the desired final type.
        public decimal YearlyPay { get { return (decimal)HoursWorkedInTheYear * HourlyRate; } }

        public override string ToString()
        {
            return $"Employee: ID:{ID,5} Name:{Name,20} State:{StateCode,5} Hours:{HoursWorkedInTheYear,10:0000000.00}, Rate:{HourlyRate:####:000} income:{YearlyPay,12:#######.00} TaxDue: {TaxDueForTheYear}";
        }

        public EmployeeRecord(string csv)
        {
            string[] items = csv.Split(',');
            if (items.Length != 5)
            {
                throw new Exception($"Invalid number of elements in the csv string: expecting 5, found {items.Length} in csv: '{csv}'");
            }
            int id;
            if (int.TryParse(items[0], out id))
            {
                ID = id;
            }
            else
            {
                throw new Exception($"Invalid id, not an integer: '{items[0]}' in csv: '{csv}'");
            }
            Name = items[1];
            StateCode = items[2];
            double hours;
            if (double.TryParse(items[3], out hours))
            {
                HoursWorkedInTheYear = hours;
            }
            else
            {
                throw new Exception($"Invalid HoursWorkedInTheYear, not a double: '{items[3]}' in csv: '{csv}'");
            }
            decimal rate;
            if (decimal.TryParse(items[4], out rate))
            {
                HourlyRate = rate;
            }
            else
            {
                throw new Exception($"Invalid HourlyRate, not a decimal: '{items[4]}' in csv: '{csv}'");
            }
        }

        public decimal TaxDueForTheYear
        {
            get
            {
                decimal income = HourlyRate * (decimal)HoursWorkedInTheYear;
                try
                {
                    return Part1.TaxCalculator.ComputeTaxFor(StateCode, income);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return 0;
                }
            }
        }
    }

    static class EmployeesList
    {
        static List<EmployeeRecord> employees;
        public static List<EmployeeRecord> Employees { get { return employees; } }
        static EmployeesList()
        {
            System.IO.StreamReader reader = null;
            try
            {
                employees = new List<EmployeeRecord>();


                reader = System.IO.File.OpenText("employees.csv");
                string line;
                do
                {
                    line = reader.ReadLine();

                    try
                    {
                        EmployeeRecord r = new EmployeeRecord(line);
                        employees.Add(r);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        Console.WriteLine("Continuing...");
                    }

                } while (!reader.EndOfStream);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.WriteLine("Trying to continue...");

            }

            reader?.Dispose();

        }
    }


    class Program
    {
        public static void Main()
        {

            try
            {
                if (System.IO.File.Exists("verbose.txt"))
                {
                    var verbosereader = System.IO.File.OpenText("verbose.txt");
                    string v = verbosereader.ReadLine();
                    bool test;
                    if (bool.TryParse(v, out test))
                    {
                        Part1.TaxCalculator.Verbose = test;
                    }
                    verbosereader.Dispose();
                }

                foreach (EmployeeRecord r in EmployeesList.Employees)
                {
                    try
                    {
                        Console.WriteLine(r);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }



            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
    }
}
