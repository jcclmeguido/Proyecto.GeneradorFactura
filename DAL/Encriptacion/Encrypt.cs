using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Axon.DAL.Encriptacion
{
    public class Encrypt
    {

        string patron_Aes = "s0ftw4r3_f4ct0ry";

        public string EncriptarCadena(string CadenaEncriptar)
        {
            byte[] bytTemp = null;
            string strtemp = "";
            bytTemp = EncriptarCadena(CadenaEncriptar, patron_Aes);
            strtemp = Convert.ToBase64String(bytTemp);
            return strtemp;
        }

        private byte[] EncriptarCadena(string CadenaEncriptar, string patron_Aes)
        {
            return EncriptarCadena(CadenaEncriptar, (new PasswordDeriveBytes(patron_Aes, null).GetBytes(32)));
        }

        private byte[] EncriptarCadena(string CadenaEncriptar, byte[] p)
        {
            Rijndael miRijndael = Rijndael.Create();
            byte[] encrypted = null;
            byte[] returnValue = null;
            try
            {
                miRijndael.Key = p;
                miRijndael.IV = Encoding.UTF8.GetBytes("1234567890123456");
                byte[] toEncryot = Encoding.UTF8.GetBytes(CadenaEncriptar);
                encrypted = (miRijndael.CreateEncryptor()).TransformFinalBlock(toEncryot, 0, toEncryot.Length);
                returnValue = new byte[miRijndael.IV.Length + encrypted.Length];
                miRijndael.IV.CopyTo(returnValue, 0);
                encrypted.CopyTo(returnValue, miRijndael.IV.Length);
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                miRijndael.Clear();
                miRijndael = null;
            }
            return returnValue;

        }



        public string DesencriptarCadena(string cadenaEncriptada)
        {
            string strtemp = "";
            if (cadenaEncriptada != null && String.Compare(cadenaEncriptada, "", true) != 0)
            {
                byte[] bytTemp = Convert.FromBase64String(cadenaEncriptada);
                strtemp = Desencriptar(bytTemp, patron_Aes);
                bytTemp = null;
            }
            return strtemp;
        }

        private string Desencriptar(byte[] bytTemp, string patron_Aes)
        {
            return Desencriptar(bytTemp, (new PasswordDeriveBytes(patron_Aes, null).GetBytes(32)));

        }

        private string Desencriptar(byte[] bytTemp, byte[] p)
        {
            Rijndael miRijndael = Rijndael.Create();
            var tempArray = new byte[miRijndael.IV.Length];
            var encryoted = new byte[bytTemp.Length - miRijndael.IV.Length];
            string returnValue = string.Empty;
            try
            {
                miRijndael.Key = p;
                Array.Copy(bytTemp, tempArray, tempArray.Length);
                Array.Copy(bytTemp, tempArray.Length, encryoted, 0, encryoted.Length);
                miRijndael.IV = tempArray;
                returnValue = Encoding.UTF8.GetString(miRijndael.CreateDecryptor().TransformFinalBlock(encryoted, 0, encryoted.Length));
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                miRijndael.Clear();
            }
            return returnValue;
        }

    }
}
