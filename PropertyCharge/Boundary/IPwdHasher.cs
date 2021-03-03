using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jason5Lee.PropertyCharge.Boundary
{
    public interface IPwdHasher
    {
        public string Hash(string pwd);
    }
}
