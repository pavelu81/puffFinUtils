using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LumenWorks.Framework.IO.Csv;
using System.IO;

namespace processAceMoneyExport
{
    class Program
    {
        const string ALLTRANSACTIONFILE = @"c:\Users\urbanovsky\Ubuntu One\personal\Report.csv";
        const string PLANFILE = @"c:\Users\urbanovsky\Ubuntu One\personal\Plánování.csv";
        const string OUTPUTFILE = @"c:\csvdb\finout.csv";
        static void Main(string[] args)
        {
            List<MyTransaction> transactions = new List<MyTransaction>();


            //nacteme skutecnosti
            Console.WriteLine("Zpracování skutečností AceMoney");
            using (CachedCsvReader csv = new CachedCsvReader(new StreamReader(ALLTRANSACTIONFILE, Encoding.GetEncoding("Windows-1250")), true, ',', '"'))
            {
                csv.MissingFieldAction = MissingFieldAction.ReplaceByNull;
                int i = 0;
                foreach (string[] row in csv)
                {
                    i++;
                    Console.Write("{0}", i); Console.CursorLeft = 0;
                    transactions.AddRange(new AceMoneyTransaction(row).GetMyTransactions());
                }
            }

            //nacteme plany
            Console.WriteLine("Zpracování plánu AceMoney");
            using (CachedCsvReader csv = new CachedCsvReader(new StreamReader(PLANFILE, Encoding.GetEncoding("Windows-1250")), true, ',', '"'))
            {
                csv.MissingFieldAction = MissingFieldAction.ReplaceByNull;
                int i=0 ;
                foreach (string[] row in csv)
                {
                    i++;
                    Console.Write("{0}",i); Console.CursorLeft = 0;
                    transactions.AddRange(new AceMoneyPlan(row).GetMyTransactions());
                }
            }


            
            //zapiseme prepracovane transakce
            StreamWriter sw = new StreamWriter(OUTPUTFILE, false, Encoding.GetEncoding("Windows-1250"));
            foreach (MyTransaction myT in transactions)
            {
                sw.WriteLine(myT.CsvRowString);
            }
            sw.Flush();
            sw.Close();

        }
    }
}
