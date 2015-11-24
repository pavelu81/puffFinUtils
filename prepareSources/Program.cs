using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LumenWorks.Framework.IO.Csv;
using System.IO;
using System.Net;

namespace prepareSources
{
	class Program
	{
		static void Main(string[] args)
		{
			List<string> allowedBankPaths = new List<string>();
			allowedBankPaths.Add(@"c:\Documents and Settings\Urbanovsky\Dokumenty\Downloads");
			allowedBankPaths.Add(@"d:\Data\Downloads\Sporoziro");
			string bankFilePattern = "TH_*.csv";

			List<string> allowedBankHypoPaths = new List<string>();
			allowedBankHypoPaths.Add(@"c:\Documents and Settings\Urbanovsky\Dokumenty\Downloads");
			allowedBankHypoPaths.Add(@"d:\Data\Downloads\Hypoteka");
			string bankHypoFilePattern = "TH_*.csv";

			List<string> allowedBankSporPaths = new List<string>();
			allowedBankSporPaths.Add(@"c:\Documents and Settings\Urbanovsky\Dokumenty\Downloads");
			allowedBankSporPaths.Add(@"d:\Data\Downloads\Sporici");
			string bankSporFilePattern = "TH_*.csv";

			FileInfo chosenBankOsobFile = chooseLastEditedFile(allowedBankPaths, bankFilePattern);
			FileInfo chosenBankHypoFile = chooseLastEditedFile(allowedBankHypoPaths, bankHypoFilePattern);
			FileInfo chosenBankSporFile = chooseLastEditedFile(allowedBankSporPaths, bankSporFilePattern);

			if (chosenBankOsobFile != null) rewriteBankCSV(chosenBankOsobFile.FullName, AccountType.OsobniKonto);
			if (chosenBankHypoFile != null) rewriteBankCSV(chosenBankHypoFile.FullName, AccountType.Hypoteka);
			if (chosenBankSporFile != null) rewriteBankCSV(chosenBankSporFile.FullName, AccountType.SporiciUcet);
		}

		private static FileInfo chooseLastEditedFile(List<string> allowedPaths, string filePattern)
		{
			FileInfo chosenFile = null;
			foreach (string path in allowedPaths)
			{
				try
				{
					DirectoryInfo di = new DirectoryInfo(path);
					foreach (FileInfo fi in di.GetFiles(filePattern))
					{
						if (chosenFile == null || chosenFile.LastWriteTime < fi.LastWriteTime)
						{
							chosenFile = fi;
						}
					}

				}
				catch (DirectoryNotFoundException ex)
				{
					//Console.WriteLine("Adresář {0} nenalezen...", path);
				}
				catch (IOException ex)
				{
					Console.WriteLine(ex.Message);
				}
			}
			if (chosenFile != null) Console.WriteLine("Nalezen soubor {0}...", chosenFile.FullName);
			return chosenFile;
		}

		static void rewriteBankCSV(string file)
		{
			rewriteBankCSV(file, AccountType.OsobniKonto);
		}
		static void rewriteBankCSV(string file, AccountType accType)
		{
			string outFileName = "";
			switch (accType)
			{
				case AccountType.OsobniKonto:
					outFileName = "banka.CSV";
					break;
				case AccountType.Hypoteka:
					outFileName = "hypo.CSV";
					break;
				case AccountType.SporiciUcet:
					outFileName = "sporici.CSV";
					break;
				default:
					break;
			}

			Console.WriteLine("Zpracovávám " + file);
			StreamWriter sw = new StreamWriter(String.Format(outFileName, file), false, Encoding.GetEncoding("Windows-1250"));
			using (CachedCsvReader csv = new CachedCsvReader(new StreamReader(file, Encoding.GetEncoding("Windows-1250")), true, ',', '"'))
			{
				csv.MissingFieldAction = MissingFieldAction.ReplaceByNull;
				int fieldCount = csv.FieldCount;
				string[] headers = csv.GetFieldHeaders();
				while (csv.ReadNextRecord())
				{
					string newLine = composeOutLine(csv, accType);
					sw.WriteLine(newLine);
				}
			}
			sw.Flush();
			sw.Close();
		}

		static string composeOutLine(CachedCsvReader csv, AccountType accType)
		{
			string[] outArr = new string[9];

			switch (accType)
			{
				case AccountType.OsobniKonto:
					outArr[OutColumn.Cislo] = String.Format("\"{0}\"", csv[12]);
					outArr[OutColumn.Datum] = String.Format("\"{2}/{1}/{0}\"", csv[01].Split('/'));
					outArr[OutColumn.Prijemce] = String.Format("\"{0}\"", csv[03]);
					outArr[OutColumn.Kategorie] = String.Format("\"{0}\"", "");
					outArr[OutColumn.S] = String.Format("\"{0}\"", "");
					outArr[OutColumn.Vydej] = String.Format("\"{0}\"", "");
					outArr[OutColumn.Prijem] = String.Format("\"{0}\"", csv[02]);
					outArr[OutColumn.Celkem] = String.Format("\"{0}\"", "");
					//outArr[OutColumn.Poznamka] = String.Format("\"{0} {1} {2} {3} {4}\"", csv[10], csv[0], csv[07], csv[13], csv[11]);
					outArr[OutColumn.Poznamka] = joinSelectedStringsFromCsv(csv, " * ", 13, 11, 10, 0, 7);

					break;
				case AccountType.Hypoteka:
					outArr[OutColumn.Cislo] = String.Format("\"{0}\"", csv[11]);
					outArr[OutColumn.Datum] = String.Format("\"{2}/{1}/{0}\"", csv[01].Split('/'));
					outArr[OutColumn.Prijemce] = String.Format("\"{0}\"", csv[04]);
					outArr[OutColumn.Kategorie] = String.Format("\"{0}\"", "");
					outArr[OutColumn.S] = String.Format("\"{0}\"", "");
					outArr[OutColumn.Vydej] = String.Format("\"{0}\"", "");
					outArr[OutColumn.Prijem] = String.Format("\"{0}\"", csv[03]);
					outArr[OutColumn.Celkem] = String.Format("\"{0}\"", "");
					outArr[OutColumn.Poznamka] = joinSelectedStringsFromCsv(csv, " * ", 0, 9, 10);
					break;
				case AccountType.SporiciUcet:
					outArr[OutColumn.Cislo] = String.Format("\"{0}\"", "");
					outArr[OutColumn.Datum] = String.Format("\"{2}/{1}/{0}\"", csv[01].Split('/'));
					outArr[OutColumn.Prijemce] = String.Format("\"{0}\"", csv[05]);
					outArr[OutColumn.Kategorie] = String.Format("\"{0}\"", "");
					outArr[OutColumn.S] = String.Format("\"{0}\"", "");
					outArr[OutColumn.Vydej] = String.Format("\"{0}\"", "");
					outArr[OutColumn.Prijem] = String.Format("\"{0}\"", csv[03]);
					outArr[OutColumn.Celkem] = String.Format("\"{0}\"", "");
					outArr[OutColumn.Poznamka] = joinSelectedStringsFromCsv(csv, " * ", 17, 14, 15, 10, 0, 16);
					break;
				default:
					break;
			}



			string outStr = String.Join(";", outArr);
			return outStr;
		}


		static string joinSelectedStringsFromCsv(CachedCsvReader csv, string delimiter, params int[] selectedIndexes)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("\"");

			bool sthAdded = false;
			foreach (int index in selectedIndexes)
			{
				if (!String.IsNullOrEmpty(csv[index].ToString()))
				{
					if (sthAdded) sb.Append(delimiter);
					sb.Append(csv[index].ToString());
					sthAdded = true;
				}
			}
			sb.Append("\"");

			return sb.ToString();
		}

		/*
		static void rewriteCashCSV(string file)
		{
			Console.WriteLine("Zpracovávám " + file);
			StreamWriter sw = new StreamWriter(String.Format("hotovost.CSV", file), false, Encoding.GetEncoding("Windows-1250"));
			using (CachedCsvReader csv = new CachedCsvReader(new StreamReader(file), true, ',', '"'))
			{
				csv.MissingFieldAction = MissingFieldAction.ReplaceByNull;
				int fieldCount = csv.FieldCount;
				string[] headers = csv.GetFieldHeaders();
				while (csv.ReadNextRecord())
				{
					string datum = csv[00].Split(' ')[0];
					string[] strArr = new string[9];
					strArr[00] = String.Format("\"{0}\"", "");							//cislo
					strArr[01] = String.Format("\"{0}/{1}/{2}\"", datum.Split('.'));	//datum
					strArr[02] = String.Format("\"{0}\"", "");							//prijemce
					strArr[03] = String.Format("\"{0}\"", "");							//kategorie
					strArr[04] = String.Format("\"{0}\"", "");							//S
					strArr[05] = String.Format("\"{0}\"", csv[01]);						//vydej
					strArr[06] = String.Format("\"{0}\"", "");							//prijem
					strArr[07] = String.Format("\"{0}\"", "");							//celkem
					strArr[08] = String.Format("\"{0}\"", csv[02]);						//poznamka
					sw.WriteLine(String.Join(";", strArr));
				}
			}
			sw.Flush();
			sw.Close();

		}

		*/

		/*
		static void rewriteCashCSV(string URL)
		{
			Console.WriteLine( "Zpracovávám hotovost z webu");
			HttpWebRequest httpRequest = ( HttpWebRequest )WebRequest.Create( URL );

			httpRequest.Timeout = 10000;     // 10 secs
			httpRequest.UserAgent = "Code Sample Web Client";

			HttpWebResponse webResponse = ( HttpWebResponse )httpRequest.GetResponse();
			StreamReader responseStream = new StreamReader( webResponse.GetResponseStream() );

			string content = responseStream.ReadToEnd();
			StreamWriter sw = new StreamWriter( "hotovost.CSV", false, Encoding.GetEncoding( "Windows-1250" ) );
			sw.Write( content );
			sw.Flush();
			sw.Close();
		}*/

	}
}