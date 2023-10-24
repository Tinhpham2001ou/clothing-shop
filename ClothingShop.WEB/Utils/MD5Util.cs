using System.Security.Cryptography;
using System.Text;

namespace ClothingShop.WEB.Utils
{
    public class MD5Util
    {
        public static string GetMD5(string password)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] hashValue = md5.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToHexString(hashValue);
            }
        }
    }
}
