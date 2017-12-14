using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace TakuCodeChecker {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        // Vars
        private readonly char[] ALLOWEDCHARS = { '1', '0', 'e' };
        private OpenFileDialog ofd = new OpenFileDialog();

        public MainWindow() {
            // General init
            InitializeComponent();
            tbOutput.AppendText("Welcome, please give me some data.\n");
            tbOutput.ScrollToEnd();

            // Init OpenFileDialog
            ofd.InitialDirectory = "c:\\";
            ofd.Filter = "json files (*.json)|*.json";
            ofd.RestoreDirectory = true;
        }

        private void BtnChooseFile_Click(object sender, RoutedEventArgs e) {
            // File loading
            if (ofd.ShowDialog() == true) {
                tbOutput.AppendText("File " + Path.GetFileNameWithoutExtension(ofd.FileName) + " loaded.\n");
                tbOutput.ScrollToEnd();
                btnFileCheck.IsEnabled = true;
                tbFile.Text = Path.GetFileName(ofd.FileName);
            } else {
                ofd.FileName = "";
                tbOutput.AppendText("No file selected.\n");
                tbOutput.ScrollToEnd();
                btnFileCheck.IsEnabled = false;
                tbFile.Text = "";
            }
        }

        private void BtnFileCheck_Click(object sender, RoutedEventArgs e) {
            // Get data
            Stream myStream;
            Data input = new Data();
            try {
                if ((myStream = ofd.OpenFile()) != null) using (StreamReader r = new StreamReader(myStream)) input = JsonConvert.DeserializeObject<Data>(r.ReadToEnd());
            } catch (Exception ex) {
                tbOutput.AppendText("Reading file failed loaded.\n");
                tbOutput.AppendText("---------------------------------------\n");
                tbOutput.ScrollToEnd();
                return;
            }

            // Check if there is data
            if(input.Name == "") {
                tbOutput.AppendText("No data fount in file...\n");
                tbOutput.AppendText("---------------------------------------\n");
                tbOutput.ScrollToEnd();
                return;
            }
            
            // Vars
            Dictionary<string, bool> uniquePuzzles = new Dictionary<string, bool>();
            int totErrors = 0;
            int totPuzzles = 0;

            // Go over Diffs
            foreach (Diff diff in input.Diff) {
                // Log
                tbOutput.AppendText("** Current diff: "+diff.Name+"\n");
                tbOutput.ScrollToEnd();
                int errors = 0;

                // Go over Puzzles
                foreach(Puzzle puzzle in diff.Puzzle) {
                    int res = CheckString(puzzle.Code);

                    // Error encountered
                    if(res != 0) {
                        errors++;
                        tbOutput.AppendText("Error in: " + puzzle.Code + "; Err: " + res + "\n");
                        tbOutput.ScrollToEnd();
                    }
                }

                // End log of Diff
                totErrors += errors;
                totPuzzles += diff.Puzzle.Count;
                tbOutput.AppendText("** diff " + diff.Name + " passed with (" + (diff.Puzzle.Count - errors) + "/" + diff.Puzzle.Count + ")\n");
                tbOutput.AppendText("******\n");
                tbOutput.ScrollToEnd();
            }

            // End log
            tbOutput.AppendText("** Total result: ("+ (totPuzzles -totErrors) + "/" + totPuzzles + ")\n");
            tbOutput.AppendText((totErrors == 0) ? "All okay!\n" : "Errors encountered, check the logs!\n");
            tbOutput.AppendText("---------------------------------------\n");
            tbOutput.ScrollToEnd();
        }

        private void BtnStringCheck_Click(object sender, RoutedEventArgs e) {
            string input = tbString.Text.Trim();

            // Empty input
            if (input == "") {
                // No valid input
                tbOutput.AppendText("There's nothing there to validate...\n");
                tbOutput.AppendText("---------------------------------------\n");
                tbOutput.ScrollToEnd();
                return;
            }

            // Check the string
            int errors = CheckString(input);

            // Show errors
            if ((errors & 0b10) == 0b10) tbOutput.AppendText("Invalid Char in input.\n");
            if ((errors & 0b01) == 0b01) tbOutput.AppendText("Invalid Length of input.\n");
            if (errors == 0) tbOutput.AppendText("Valid input!\n");
            tbOutput.AppendText("---------------------------------------\n");
            tbOutput.ScrollToEnd();
        }

        private int CheckString(string toCheck) {
            // Vars
            int errs = 0b00;

            // Check length
            if (!IsPerfectSquare(toCheck.Length)) errs |= 0b01;

            // Check Chars
            if (!StringOnlyContainsChars(toCheck)) errs |= 0b10;

            // No issue
            return errs;
        }

        private bool IsPerfectSquare(int number) {
            // This only for puzzles up to 14x14. So unless you have giant puzzles, this will suffice.

            // Odd
            if (number % 2 == 1) return false;

            switch (number) {
                case 4:     // 2x2
                case 16:    // 4X4
                case 36:    // 6x6
                case 64:    // 8x8
                case 100:   // 10x10
                case 144:   // 12x12
                case 196:   // 14x14
                    return true;
                default:
                    return false;
            }
        }

        private bool StringOnlyContainsChars(string toCheck) {
            // Loop over string
            foreach (char c in toCheck) {
                bool matched = false;
                foreach (char k in ALLOWEDCHARS) if (c == k) matched = true;
                if (!matched) return false;
            }

            // No errors found
            return true;
        }
    }
}
