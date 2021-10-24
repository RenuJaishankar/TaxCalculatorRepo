using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Part3
{
    class Program
    {
        public static void Main()
        {
            // these are three (LINQ) variables used to decide which column and which direction (ascending or decending)
            System.Collections.Generic.IEnumerable<Part2.EmployeeRecord> Q = from r in Part2.EmployeesList.Employees select r;
            // Q is the basic query returning all the records in the employees List
            System.Collections.Generic.IEnumerable<Part2.EmployeeRecord> R;
            // R is the basic query with the basic orderby added (by a switch)
            System.Collections.Generic.IEnumerable<Part2.EmployeeRecord> Final;
            // Final is R with a reverse added  when the order is descending
            // otherwise Final is the same as R when the order is ascending
            try
            {
                // this is the section to read the file verbose.txt to see if verbose mode is true or false
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


                do
                {

                    // this is the section to choose the sort column
                    Console.Write("choose a column to sort by: (S)tate (N)ame (I)d (P)ay (T)ax or (E)xit:");
                    string selection = Console.ReadLine();
                    // this switch selects the basic column and MAKEs R using Linq from the original Query Q
                    switch (selection.ToUpper())
                    {
                        case ("S"): R = from x in Q orderby x.StateCode select x; break;
                        case ("N"): R = from x in Q orderby x.Name select x; break;
                        case ("I"): R = from x in Q orderby x.ID select x; break;
                        // there was not a way to sort on the YearlyPay because Part2 did not indicate to create
                        // a YearlyPay property.  But when it was needed here to sort by, the YearlyPay property
                        // was added to the Employee Record in the Part2 portion of the solution
                        case ("P"): R = from x in Q orderby x.YearlyPay select x; break;
                        case ("T"): R = from x in Q orderby x.TaxDueForTheYear select x; break;
                        case ("E"): Console.WriteLine("Goodbye..."); return;
                        default:
                            Console.WriteLine("Choice not recognized, try again...");
                            continue;  // this continue is for the outer do (choose a column)
                    }
                    do
                    {
                        Console.Write("choose a direction to sort by: (A)scending (D)escending:");
                        string order = Console.ReadLine();
                        switch (order.ToUpper())
                        {
                            case ("A"): Final = R; break;             // Final is the same as R - break out of the switch
                            case ("D"): Final = R.Reverse(); break;   // Final is the reverse of R - break out of the switch
                            default:
                                Console.WriteLine("Choice not recognized, try again...");
                                continue;  // this continue is for the inner do (ascending or descending)
                        }
                        break;  // getting here means you have selected both a column and an order
                                // so this break gets out of the outer do so we can continue
                    } while (true);

                    foreach (Part2.EmployeeRecord r in Final)  // final was set in the inner do on line 65 or 66
                    {
                        try
                        {
                            Console.WriteLine(r);
                        }
                        catch (Exception ex)  // to catch the exceptions when calculating the tax,
                                              //  and go to the next since it is within the foreach
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                } while (true);

            }

            catch (Exception ex)  // global catch to catch all exceptions, and exit the program
            {
                Console.WriteLine(ex.Message);
            }
        }

    }
}

