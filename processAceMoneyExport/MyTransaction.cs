using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace processAceMoneyExport
{
    public class MyTransaction
    {

        #region fields
        private string typ;
        private DateTime datum;
        private string ucet;
        private string prijemce;
        private string kategorie;
        private string subkategorie;
        private Decimal obrat;
        private string poznamka;
        #endregion

        public MyTransaction(string typ, DateTime datum, string ucet, string prijemce, string kategorie, string subkategorie, Decimal obrat, string poznamka)
        {
            this.typ = typ;
            this.datum = datum;
            this.ucet = ucet;
            this.prijemce = prijemce;
            this.kategorie = kategorie;
            this.subkategorie = subkategorie;
            this.obrat = obrat;
            this.poznamka = poznamka;
        }

        public string CsvRowString
        {
            get
            {
                string[] csvFields = new string[9];
                csvFields[0] = this.typ;
                csvFields[1] = String.Format("{0}.{1}.{2}", this.datum.Day, this.datum.Month, this.datum.Year);
                csvFields[2] = this.ucet;
                csvFields[3] = this.prijemce;
                csvFields[4] = this.kategorie;
                csvFields[5] = this.subkategorie;
                csvFields[6] = this.obrat.ToString();
                csvFields[7] = this.poznamka;
                csvFields[8] = this.budgetType;



                //sem dopsat exportovane pole

                //pridame uvozovky
                for (int i = 0; i < csvFields.Length; i++)
                {
                    csvFields[i] = String.Format("\"{0}\"", csvFields[i]);
                }
                return string.Join(";", csvFields);
            }
        }

        public string Kategorie
        {
            get { return kategorie; }
        }
        public string Subkategorie
        {
            get { return subkategorie; }
        }
        public Decimal Obrat
        {
            get { return obrat; }
        }
        public DateTime Datum
        {
            get { return datum; }
        }
        public string budgetType
        {
            get
            {
                string kat = this.kategorie + ":" + this.subkategorie;
                switch (kat)
                {
                    case "Dárky:Vánoce":
                    case "Spoření:Stavební spoření":
                    case "Příjem:Mzda":
                    case "Spoření:ING":
                    case "Spoření:Důchodové připojištění":
                    case "Domácnost:Hypotéka":
                    case "Auto:Dálnice":
                    case "Příjem:Příspěvek na domácnost":
                    case "Pojištění:Životní investiční":
                    case "Služby:Bankovní poplatky":
                    case "Hobby:Dialog":
                    case "Pojištění:Cestovní pojištění":
                    case "Služby:Telefon":
                    case "Hobby:Internetové služby":
                    case "Pojištění:Platební karty":
                    case "Auto:Pojištění":
                    case "Pojištění:Bytu":
                    case "Dárky:Janinka":
                    case "Doprava:Městská hromadná":
                    case "Domácnost:Daň z nemovitosti":
                    case "Domácnost:Popelnice":
                    case "Povyražení:AkceKoleno":
                    case "Povyražení:Dovolená":
                    case "Domácnost:Elektřina":
                    case "Domácnost:Internet":
                    case "Domácnost:Koncesionářské poplatky":
                    case "Domácnost:Nájem":
                    case "Domácnost:Plyn":
                    case "Služby:Pošta":
                    case "Rozdělit:":
                        return "Mandatory";
                    case "Auto:Benzin":
                        return "Benzin";
                    default:
                        return "Kapesné";
                }

            }
        }

    }
}
