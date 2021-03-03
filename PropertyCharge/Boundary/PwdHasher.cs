using System;
using System.Security.Cryptography;
using System.Text;

namespace Jason5Lee.PropertyCharge.Boundary
{
    public class PwdHasher : IPwdHasher
    {
        private readonly byte[] _salts; // TODO

        public PwdHasher(byte[] salts)
        {
            if (salts.Length != 8)
            {
                throw new ArgumentException($"wrong length of salt, expect: 8, actual: {salts.Length}");
            }
            _salts = (byte[])salts.Clone();
        }

        public PwdHasher(string saltsHex)
        {
            if (saltsHex.Length != 16)
            {
                throw new ArgumentException($"wrong length of salt hex string, expect: 16, actual: {saltsHex.Length}");
            }
            _salts = new byte[8];
            for (int i = 0; i < 16; i += 2)
                _salts[i / 2] = Convert.ToByte(saltsHex.Substring(i, 2), 16);
        }

        public string Hash(string pwd)
        {
            var bytes = new Rfc2898DeriveBytes(pwd, _salts).GetBytes(16);
            var sb = new StringBuilder(32);
            foreach (var b in bytes)
            {
                sb.AppendFormat("{0:x2}", b);
            }
            return sb.ToString();
        }
    }
}
