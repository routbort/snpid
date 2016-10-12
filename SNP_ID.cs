using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace OncoSeek.Genomics

{
    public class SNP_ID
    {

        public List<SNP> SNPs { get; set; }

        public void Sort()
        {
            SNPs.Sort(
                delegate (SNP a, SNP b)
                {
                    int xdiff = Convert.ToInt32(a.contig.Replace("chr", "")).CompareTo(Convert.ToInt32(b.contig.Replace("chr", "")));
                    if (xdiff != 0) return xdiff;
                    else return a.position.CompareTo(b.position);
                });
        }

        public string Hash
        {
            get
            {
                Sort();
                string hash_pre = "";
                foreach (SNP snp in SNPs)
                    hash_pre += snp.contig + "|" + snp.position.ToString() + ",";
                return Convert.ToBase64String(new MD5CryptoServiceProvider().ComputeHash(Encoding.ASCII.GetBytes(hash_pre)));
            }
        }

        public override string ToString()

        {
            string SNP_ID = "";
            Sort();
            foreach (SNP snp in SNPs)
                SNP_ID += snp.variant;
            return SNP_ID;
        }

    }

}
