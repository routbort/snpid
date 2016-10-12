using System;
using System.Collections.Generic;

namespace OncoSeek.Genomics
{
    public class SNP
    {

        static double LOW_HET { get; set; } = 0.45f;
        static double HIGH_HET { get; set; } = 0.55f;
        static double HIGH_NEGATIVE { get; set; } = 0.05f;
        static double LOW_HOMO { get; set; } = 0.90f;
        static char[] tab_splitter = new char[] { '\t' };
        static char[] colon_splitter = new char[] { ':' };

        public string hash
        {
            get
            {
                return genome + "|" + contig + "|" + position.ToString() + "|" + reference + "|" + variant;
            }
        }
        public string genome { get; set; } = "hg19";
        public string contig { get; set; }
        public int position { get; set; }
        public char reference { get; set; }
        public char variant { get; set; }
        public int depth { get; set; }

        public Dictionary<char, double> AF = new Dictionary<char, double>();

        public string instance_data { get; set; }

        public static SNP GetInstanceFromBamReadcountLine(string line)

        {
            SNP snp = new SNP();
            string[] items = line.Split(tab_splitter);
            string contig = items[0];
            int position = Convert.ToInt32(items[1]);
            char reference = items[2][0];
            int DP = Convert.ToInt32(items[3]);
            double A = Convert.ToDouble(items[5].Split(colon_splitter)[1]);
            double C = Convert.ToDouble(items[6].Split(colon_splitter)[1]);
            double G = Convert.ToDouble(items[7].Split(colon_splitter)[1]);
            double T = Convert.ToDouble(items[8].Split(colon_splitter)[1]);
            double X = Convert.ToDouble(items[9].Split(colon_splitter)[1]);
            snp.AF['A'] = A / DP;
            snp.AF['C'] = C / DP;
            snp.AF['G'] = G / DP;
            snp.AF['T'] = T / DP;
            snp.AF['X'] = X / DP;
            string bases = "";
            bases += GetBaseFromFrequency("A", snp.AF['A']);
            bases += GetBaseFromFrequency("C", snp.AF['C']);
            bases += GetBaseFromFrequency("G", snp.AF['G']);
            bases += GetBaseFromFrequency("T", snp.AF['T']);
            snp.instance_data = line;
            snp.position = position;
            snp.contig = contig;
            snp.reference = reference;
            snp.depth = DP;
            snp.variant = GetIUPACCode(bases);
            return snp;
        }

        public string AlleleFreqs
        {
            get
            {
                string output = "";
                foreach (char allele in AF.Keys)
                    output += allele + ":" + AF[allele].ToString("0.00") + ",";
                return output.TrimEnd(new char[] { ',' });
            }
        }

        static char GetIUPACCode(string bases)
        {
            string Bases = bases.ToUpper();
            bool A = bases.Contains("A");
            bool C = bases.Contains("C");
            bool G = bases.Contains("G");
            bool T = bases.Contains("T");

            if (bases.Contains("?") || bases.Contains("X"))
                return '?';

            if (A && C && G && T) return 'N';
            if (A && C && G) return 'V';
            if (A && C && T) return 'H';
            if (A && G && T) return 'D';
            if (C && G && T) return 'B';

            if (A && C) return 'M';
            if (G && T) return 'K';
            if (A && T) return 'W';
            if (G && C) return 'S';
            if (C && T) return 'Y';
            if (A && G) return 'R';

            if (A) return 'A';
            if (C) return 'C';
            if (G) return 'G';
            if (T) return 'T';

            throw new Exception("No valid nucleotide codes found in submitted string");
        }

        static string GetBaseFromFrequency(string nucleotide, double freq)
        {

            if (freq < HIGH_NEGATIVE) return "";
            if (freq > LOW_HET && freq < HIGH_HET) return nucleotide;
            if (freq > LOW_HOMO) return nucleotide;
            return "?";
        }

    }

}
