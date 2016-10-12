using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;


namespace snpid
{
    class Program
    {
        static void Main(string[] args)
        {

            

            if (args.Length != 1)
            {
                Console.WriteLine("Usage - snp_id filenmae, where filename is the input filename");
                return;
            }
            string filename = args[0];
            if (!File.Exists(filename))
            {
               Console.WriteLine("Error - file not found");
                return;
            }

            string filenameOutput = filename + ".output";
            char[] splitter = new char[] { ':' };
            List<string> output = new List<string>();
            List<OncoSeek.Genomics.SNP> snps = new List<OncoSeek.Genomics.SNP>();
            foreach (string line in File.ReadLines(filename))
                snps.Add(OncoSeek.Genomics.SNP.GetInstanceFromBamReadcountLine(line));
            OncoSeek.Genomics.SNP_ID snpid = new OncoSeek.Genomics.SNP_ID();
            snpid.SNPs = snps;
            snpid.Sort();
            output.Add("SNP_ID (HASH " + snpid.Hash + "):" + snpid.ToString());
            foreach (OncoSeek.Genomics.SNP snp in snpid.SNPs)
                output.Add(snp.instance_data + "\t" + snp.AlleleFreqs + "\t" + snp.variant);
            File.WriteAllLines(filenameOutput, output);
            Console.WriteLine("Output completed and stored at " + filenameOutput);
        }




    }
}
