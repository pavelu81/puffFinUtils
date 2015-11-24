using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace processAceMoneyExport
{
    public class AceMoneyTransaction
    {
        private string[] csvRow;
        string kat, subkat;
        public AceMoneyTransaction(string[] csvRow)
        {
            this.csvRow = csvRow;
            string[] katArr = this.Kategorie.Split(':');
            kat = katArr.Length > 0 ? katArr[0] : "";
            subkat = katArr.Length > 1 ? katArr[1] : "";

        }

        public string Cislo
        {
            get { return this.csvRow[0]; }
        }
        public DateTime Datum
        {
            get
            {
                try
                {
                    return DateTime.Parse(csvRow[1]);
                }
                catch
                {
                    return DateTime.MinValue;
                }
            }

        }
        public string Ucet
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
        public string Stav
        {
            get { return this.csvRow[5]; }
        }
        public Decimal Vydej
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
        public Decimal Prijem
        {
            get
            {
                try
                {
                    return Decimal.Parse(csvRow[7]);
                }
                catch
                {
                    return 0;
                }
            }
        }
        public Decimal Celkem
        {
            get
            {
                try
                {
                    return Decimal.Parse(csvRow[8]);
                }
                catch
                {
                    return 0;
                }
            }
        }
        public string Poznamka
        {
            get { return this.csvRow[9]; }
        }

        public IEnumerable<MyTransaction> GetMyTransactions()
        {
            List<MyTransaction> retList = new List<MyTransaction>();

            Regex prevodRegex = new Regex(@"Převod z (?<zdroj>.+) na (?<cil>.+)");

            Match nalezenyPrevodRegexMatch = prevodRegex.Match(this.Prijemce);

            if (nalezenyPrevodRegexMatch.Success)
            {
                retList.Add(new MyTransaction("SKUTEČNOST", this.Datum, nalezenyPrevodRegexMatch.Groups["zdroj"].Value, this.Prijemce, this.kat, this.subkat, -1*this.Vydej, this.Poznamka));
                retList.Add(new MyTransaction("SKUTEČNOST", this.Datum, nalezenyPrevodRegexMatch.Groups["cil"].Value, this.Prijemce, this.kat, this.subkat, this.Prijem, this.Poznamka));
            }
            else
            {
                retList.Add(new MyTransaction("SKUTEČNOST", this.Datum, this.Ucet, this.Prijemce, kat, subkat, this.Prijem - this.Vydej, this.Poznamka));
            }
            return retList;
        }
    }
}
