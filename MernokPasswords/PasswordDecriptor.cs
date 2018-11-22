using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MernokPasswords
{

    public class PasswordDecriptor
    {

        private string requestName;

        private uint _requesterUID;

        private string _CreatorName;

        private uint _creatorUID;

        private string _PassWordOut;

        public static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        public string[] PasswordToDetails(string Password)
        {

            byte[] PassBytes = StringToByteArray(Password);
            
            string[] details = new string[4];

            var index = PassBytes.Where(n => n == (byte)'$').First();

            var NewArray = System.Text.Encoding.Default.GetString(PassBytes.Take(index).ToArray());

           _requesterUID = UInt32.Parse(NewArray.Substring(0,NewArray.IndexOf('$')));
            requestName = NewArray.Substring(NewArray.IndexOf('$')+1, NewArray.IndexOf('%')-2);
            _creatorUID = Convert.ToUInt32(NewArray.Substring(NewArray.IndexOf('#')+1, NewArray.LastIndexOf(NewArray.Last())- NewArray.IndexOf('#')));
            _CreatorName = NewArray.Substring(NewArray.IndexOf('%')+1, NewArray.IndexOf('#')- NewArray.IndexOf('%')-1);


            details[0] = _requesterUID.ToString();
            details[1] = requestName;
            details[2] = _creatorUID.ToString();
            details[3] = _CreatorName;


            return details;
        }

 
    }
}
