using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Part1
{
    static class TaxCalculator
    {
        // Create a static dictionary field that holds a List of TaxRecords and is keyed by a string
        static Dictionary<string, List<TaxRecord>> states; // to hold all the lists for records for each state


        // create a static constructor that:

        // declare a streamreader to read a file
        // enter a try/catch block for the entire static constructor to print out a message if an error occurs
        // initialize the static dictionary to a newly create empty one
        // open the taxtable.csv file into the streamreader
        // loop over the lines from the streamreader
        // read a line from the file
        // constuct a taxrecord from the (csv) line in the file
        // see if the state in the taxrecord is already in the dictionary
        //     if it is:  add the new tax record to the list of records in that state
        //     if it is not
        //            create a new list of taxrecords
        //            add the new taxrecord to the list
        //            add the list to the dictionary under the state for the taxrecord
        //provide a way to get out of the loop when you are done with the file....
        // catch any exceptions while processing each line in another try/catch block located INSIDE the loop
        //   this way if the line in the CSV file is incorrect, you will continue to process the next line
        // make sure the streamreader is disposed no matter what happens.
        static TaxCalculator()
        {
            System.IO.StreamReader reader = null;
            try
            {
                states = new Dictionary<string, List<TaxRecord>>();
                Verbose = false;
                // there is a subtle error in here.  The records from the file are entered into the list in the order
                // they appear in the file.  If they are not sorted from low to high (the floors) in the file, the results will
                // be incorrect.  

                // a more rebust solution would sort all the lists from low to high and verify that the floors and ceilings 
                // do not overlap.  This solution assumes (and we know what that does) that the file is correct, and does not
                // check the data, other than when scanning a single line for correctness.  It does not attempt to analyze all
                // the data when the entire file is read, and it should probably do that as well.
                
                reader = System.IO.File.OpenText("taxtable.csv");
                string line;

                do
                {
                    line = reader.ReadLine();

                    try
                    {
                        TaxRecord r = new TaxRecord(line);
                        List<TaxRecord> records;
                        bool found = states.TryGetValue(r.StateCode, out records);
                        //Console.WriteLine("Records", records);
                        // this technique is used when you have a dictionary of lists.
                        // first check that the dictionary entry has at least one element
                        // and if not, then make a list with the new element in a newly created list
                        if (found)
                        {
                            records.Add(r);
                            // there is already a dictionary entry with a list, so just add this element to the 
                            // existing list

                        }
                        else
                        {
                            // there is not an existing dictionary entry with a list, so make a new list
                            // and add it to the dictionary
                            records = new List<TaxRecord>();
                            records.Add(r);
                            states.Add(r.StateCode, records);

                        }
                    }
                    catch (Exception ex)  // this catch is inside the loop so that if a single line
                    // in the file is incorrect, we will log it to the screen and move to the next line
                    // instead of exiting the entire progfram
                    {
                        Console.WriteLine(ex.Message);
                        Console.WriteLine("Continuing...");
                    }

                } while (!reader.EndOfStream);
            }
            catch (Exception ex)  // this catch is outside the loop to catch any errors in FILE IO
            // like missing taxtable.csv or the file is locked to another user
            {
                Console.WriteLine(ex);
                Console.WriteLine("Trying to continue...");

            }

            reader?.Dispose();  // the reader variable was created outside the try block, so it is possible that
                                // it was never given a value (if there was a file io error for example)
                                // so since the reader was initialized to null, we use the ?. invvocaction to not invoke if the left side is null



        }
        public static bool Verbose;

        static void VWrite(string s)  // only outputs the string s if Verbose is true
        {
            if (Verbose)
            {
                Console.WriteLine(s);
            }
        }






        // create a static method (ComputeTaxFor)  to return the computed tax given a state and income
        //  use the state as a key to find the list of taxrecords for that state
        //   throw an exception if the state is not found.
        //   otherwise use the list to compute the taxes

        //  Create a variable to hold the final computed tax.  set it to 0
        //  loop over the list of taxrecords for the state
        //     check to see if the income is within the tax bracket using the floor and ceiling properties in the taxrecord
        //     if NOT:  (the income is greater than the ceiling)
        //        compute the total tax for the bracket and add it to the running total of accumulated final taxes
        //        the total tax for the bracket is the ceiling minus the floor times the tax rate for that bracket.  
        //        all this information is located in the taxrecord
        //        after adding the total tax for this bracket, continue to the next iteration of the loop
        //     IF The income is within the tax bracket (the income is higher than the floor and lower than the ceiling
        //        compute the final tax by adding the tax for this bracket to the accumulated taxes
        //        the tax for this bracket is the income minus the floor time the tax rate for this bracket
        //        this number is the total final tax, and can be returned as the final answer

        public static decimal ComputeTaxFor(string state, decimal income)
        {

            List<TaxRecord> records;
            bool found = states.TryGetValue(state, out records);  // try to find the state.
            if (found)
            {
                VWrite($"Found {records.Count} records for state: {state}");
                decimal TotalTax = 0M;  // start with a default value of 0 for the tax
                foreach (TaxRecord r in records)
                {
                    if (income >= r.Floor && income < r.Ceiling)  // we are in this bracket!
                    {
                        decimal incomeForThisBracket = income - r.Floor;  // this is the excess income over the floor
                        decimal thisBracket = incomeForThisBracket * r.Rate;  // this is the tax for this bracket
                        VWrite($"Found Record {r} thisBracket: thisBracket: [income: {incomeForThisBracket} Tax:{thisBracket}]  Total Tax computed: {TotalTax}");
                        // this is the escape clause from this routine
                        // the subtle error is here
                        // if the tax brackets are not sorted correctly, you might not capture all the taxes from the
                        // lower brackets.  This solution assumes the brackets are in ascending order in the taxtable file
                        return TotalTax + (income - r.Floor) * r.Rate;
                    }
                    else  // we are not in the bracket !
                    {
                        decimal incomeForThisBracket = r.Ceiling - r.Floor;  // this is the total income for this bracket
                        decimal thisBracket = incomeForThisBracket * r.Rate;  // this is the total tax due for this bracket
                                                                              // remember that the income is taxed at the lower rate for the lower incomes in this progressive tax structure
                                                                              // so since we are above this tax bracket, we will owe tax on the maximum income of this bracket at this bracket's rate
                        TotalTax += thisBracket;
                        // TotalTax is the accumulated tax from all lower tax brackets
                        VWrite($"Found Record {r} thisBracket: [income: {incomeForThisBracket} Tax:{thisBracket}] Total Tax so far: {TotalTax}");
                    }
                }
                // it is not expected to ever arrive here, but always assume the worst and throw an exception just in case.

                throw new Exception($"Income was higher than the tax table Ceiling:{income}");
            }
            else  // this is the else for the state being found in the dictionary
            {
                throw new Exception($"No Tax Records found for the state '{state}'");
            }

        }


    }
    // try to figure out how to implement the Verbose option AFTER you have everything else done.

    // create a TaxRecord class representing a line from the file.  It shoudl have public properties of the correct type
    // for each of the columns in the file
    //  StateCode   (used as the key to the dictionary)
    //  State       (Full state name)
    //  Floor       (lowest income for this tax bracket)
    //  Ceiling     (highest income for this tax bracket )
    //  Rate        (Rate at which income is taxed for this tax bracket)
    //
    //  Create a ctor taking a single string (a csv) and use it to load the record
    //  Be sure to throw detailed exceptions when the data is invalid
    //
    //  Create an override of ToString to print out the tax record info nicely

    class TaxRecord
    {
        #region properties
        public string StateCode { get; init; }
        public string State { get; init; }
        public decimal Floor { get; init; }

        public decimal Ceiling { get; init; }

        public decimal Rate { get; init; }
        #endregion properties

        public TaxRecord(string csv)
        {
            string[] items = csv.Split(',');
            if (items.Length != 5)
            {
                throw new Exception($"csv does not have exactly 5 fields separated by commas (it has {items.Length})['{csv}']");
            }
            StateCode = items[0];
            State = items[1];
            decimal d;
            if (decimal.TryParse(items[2], out d))
            {
                Floor = d;
            }
            else
            {
                throw new Exception($"item Floor:3rd is not recognizable as a decimal['{items[2]}'] line=['{csv}']");
            }
            if (decimal.TryParse(items[3], out d))
            {
                Ceiling = d;
            }
            else
            {
                throw new Exception($"item Ceiling:4th is not recognizable as a decimal['{items[3]}'] line=['{csv}']");
            }
            if (decimal.TryParse(items[4], out d))
            {
                Rate = d;
            }
            else
            {
                throw new Exception($"item Rate:5th is not recognizable as a decimal['{items[4]}'] line=['{csv}']");
            }
        }

        public override string ToString()
        {
            return $"Tax Record for {StateCode} {State,20} Floor:{Floor,15:000000000.000} Ceiling:{Ceiling,15:000000000.000} Rate: {Rate:00.0000}";
        }
    }
    class Program
    {
        public static void Main()
        {
            do
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
                            TaxCalculator.Verbose = test;
                        }
                        verbosereader.Dispose();
                    }

                    Console.Write("Enter a state abbreviation:");
                    string state = Console.ReadLine().ToUpper();
                    Console.Write("Enter an income:");
                    string line;
                    decimal income;
                    while (!decimal.TryParse(line = Console.ReadLine(), out income))
                    {
                        Console.WriteLine("income was not recognized as a decimal... Try again");
                        Console.Write("Enter an income:");
                    }

                    decimal tax = TaxCalculator.ComputeTaxFor(state, income);
                    Console.WriteLine(tax);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            } while (true);


        }
    }
    
}
