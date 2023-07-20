using SITA.Cryptography.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SITA.Cryptography
{
    public class CryptographyBL
    {
        public ICryptography _crypto;

        public CryptographyBL(ICryptography cryptography)
        {
            this._crypto = cryptography;
        }

        public string EncryptText(string value)
        {
            _crypto = new Cryptograph();
            return _crypto.EncryptText(value);
        }
        public string DecryptText(string value)
        {
            _crypto = new Cryptograph();
            return _crypto.DecryptText(value);
        }
    }
}
