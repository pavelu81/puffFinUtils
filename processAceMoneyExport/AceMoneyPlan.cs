using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace processAceMoneyExport
{
    public class AceMoneyPlan
    {
        private string[] csvRow;
        string kategorieA, kategorieB, source, target, type;
        public AceMoneyPlan(string[] csvRow)
        {
            this.csvRow = csvRow;

            string[] katArr = this.Kategorie.Split(':');
            
            this.kategorieA = katArr.Length > 0 ? katArr[0] : "";
            this.kategorieB = katArr.Length > 1 ? katArr[1] : "";

            Regex transakceRegex = new Regex(@"(((?<type>Výdej) z (?<source>.+))|((?<type>Příjem) na (?<target>.+)))|((?<type>Převod) z (?<source>.+) na (?<target>.+))");
            
            Match nalezenaPlanovanaTransakceRegexMatch = transakceRegex.Match(this.Transakce);
            if (nalezenaPlanovanaTransakceRegexMatch.Success)
            {
                this.type = nalezenaPlanovanaTransakceRegexMatch.Groups["type"].Value;
                this.source = nalezenaPlanovanaTransakceRegexMatch.Groups["source"].Value;
                this.target = nalezenaPlanovanaTransakceRegexMatch.Groups["target"].Value;
            }

        }

        #region CSVRow
        public DateTime Datum
        {
            get
            {
                try
                {
                    return DateTime.Parse(csvRow[0]);
                }
                catch
                {
                    return DateTime.MinValue;
                }
            }

        }
        public string Frekvence
        {
            get { return this.csvRow[1]; }
        }
        public string Transakce
        {
            get { return this.csvRow[2]; }
        }
        public string Prijemce
        {
            get { return this.csvRow[3]; }
        }
        public string Kategorie
        {
            get { return this.csvRow[4]; }
        }
        public string Auto
        {
            get { return this.csvRow[5]; }
        }
        public Decimal Castka
        {
            get
            {
                try
                {
                    return Decimal.Parse(csvRow[6]);
                }
                catch
                {
                    return 0;
                }
            }
        }
        public string Poznamka
        {
            get { return this.csvRow[7]; }
        }
        #endregion

        /// <summary>
        /// vytvoří transakce z tohoto plánu k určitému datu - pokud je to minulost, dá dnešek
        /// </summary>
        /// <param name="datum"></param>
        /// <returns></returns>
        private IEnumerable<MyTransaction> createMyTransactionWithDate(DateTime datum)
        {
            List<MyTransaction> retList = new List<MyTransaction>();
            DateTime datumOut=datum>DateTime.Today?datum:DateTime.Today;
            switch (this.type)
            {
                case "Příjem":
                    retList.Add(new MyTransaction("PLÁN",datumOut,this.target,this.Prijemce,this.kategorieA,this.kategorieB, this.Castka, this.Poznamka));
                    break;
                case "Výdej":
                    retList.Add(new MyTransaction("PLÁN",datumOut,this.source,this.Prijemce,this.kategorieA,this.kategorieB, this.Castka, this.Poznamka));
                    break;
                case "Převod":
                    retList.Add(new MyTransaction("PLÁN", datumOut, this.source, this.Prijemce, this.kategorieA, this.kategorieB, -1*this.Castka, this.Poznamka));
                    retList.Add(new MyTransaction("PLÁN", datumOut, this.target, this.Prijemce, this.kategorieA, this.kategorieB, this.Castka, this.Poznamka));
                    break;
            }
            return retList;
        }

        private DateTime nextDate(string aceMoneyFrekvence, DateTime datum)
        {
            switch (aceMoneyFrekvence)
            {
                case "Jednou":
                    return DateTime.MaxValue;
                case "Denně":
                    return datum.AddDays(1);
                case "Týdně":
                    return datum.AddDays(7);
                case "Každé dva týdny":
                    return datum.AddDays(14);
                /*
                case "Dvakrát měsíčně":
                    return datum;
                 */ 
                case "Every three weeks":
                    return datum.AddDays(21);
                case "Každé čtyři týdny":
                    return datum.AddDays(28);
                case "Měsíčně":
                    return datum.AddMonths(1);
                case "Každý druhý měsíc":
                    return datum.AddMonths(2);
                case "Čtvrtletně":
                    return datum.AddMonths(3);
                case "Každé 4 měsíce":
                    return datum.AddMonths(4);
                case "Každých 6 měsíců":
                    return datum.AddMonths(6);
                case "Ročně":
                    return datum.AddYears(1);
                case "Every two years":
                    return datum.AddYears(2);
                default:
                    return DateTime.MaxValue;
            }

        }

        public IEnumerable<MyTransaction> GetMyTransactions()
        {
            List<MyTransaction> retList = new List<MyTransaction>();

            DateTime tillDate = new DateTime(DateTime.Today.Year + 2, 12, 31); //konec prespristiho roku
            DateTime actTransDate = this.Datum;

            while (actTransDate <= tillDate)
            {
                retList.AddRange(this.createMyTransactionWithDate(actTransDate));
                actTransDate = nextDate(this.Frekvence, actTransDate);
            }
            return retList;
        }
    }
}
