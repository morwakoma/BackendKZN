using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SITA.Cryptography.Interfaces
{
   public interface ICryptography
    {
        string EncryptText(string value);
        string DecryptText(string value);
    }
}
