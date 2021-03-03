using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Jason5Lee.PropertyCharge.Boundary
{
    public class AdminContext
    {
        private readonly IPwdHasher _pwdHasher;
        private volatile string _adminPwdHash;

        public AdminContext(string adminPwd, IPwdHasher pwdHasher)
        {
            _pwdHasher = pwdHasher;
            _adminPwdHash = pwdHasher.Hash(adminPwd);
        }

        public bool TryLogin(string pwd)
        {
            return _pwdHasher.Hash(pwd).SequenceEqual(_adminPwdHash);
        }

        public void Set(string newPwd)
        {
            _adminPwdHash = _pwdHasher.Hash(newPwd);
        }
    }
}
