﻿//2018.12.10, czs, create in hmx, RSA加密

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;  
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace Gnsser
{
    #region 本地算法

    public class Rsa
    {
        /// <summary>
        /// RSA加密
        /// </summary>
        /// <param name="publickey"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string RSAEncrypt(string publickey, string content)
        {
            publickey = @"<RSAKeyValue><Modulus>5m9m14XH3oqLJ8bNGw9e4rGpXpcktv9MSkHSVFVMjHbfv+SJ5v0ubqQxa5YjLN4vc49z7SVju8s0X4gZ6AzZTn06jzWOgyPRV54Q4I0DCYadWW4Ze3e+BOtwgVU1Og3qHKn8vygoj40J6U85Z/PTJu3hN1m75Zr195ju7g9v4Hk=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            byte[] cipherbytes;
            rsa.FromXmlString(publickey);
            cipherbytes = rsa.Encrypt(Encoding.UTF8.GetBytes(content), false);

            return Convert.ToBase64String(cipherbytes);
        }

        /// <summary>
        /// RSA解密
        /// </summary>
        /// <param name="privatekey"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string RSADecrypt(string privatekey, string content)
        {
            privatekey = @"<RSAKeyValue><Modulus>5m9m14XH3oqLJ8bNGw9e4rGpXpcktv9MSkHSVFVMjHbfv+SJ5v0ubqQxa5YjLN4vc49z7SVju8s0X4gZ6AzZTn06jzWOgyPRV54Q4I0DCYadWW4Ze3e+BOtwgVU1Og3qHKn8vygoj40J6U85Z/PTJu3hN1m75Zr195ju7g9v4Hk=</Modulus><Exponent>AQAB</Exponent><P>/hf2dnK7rNfl3lbqghWcpFdu778hUpIEBixCDL5WiBtpkZdpSw90aERmHJYaW2RGvGRi6zSftLh00KHsPcNUMw==</P><Q>6Cn/jOLrPapDTEp1Fkq+uz++1Do0eeX7HYqi9rY29CqShzCeI7LEYOoSwYuAJ3xA/DuCdQENPSoJ9KFbO4Wsow==</Q><DP>ga1rHIJro8e/yhxjrKYo/nqc5ICQGhrpMNlPkD9n3CjZVPOISkWF7FzUHEzDANeJfkZhcZa21z24aG3rKo5Qnw==</DP><DQ>MNGsCB8rYlMsRZ2ek2pyQwO7h/sZT8y5ilO9wu08Dwnot/7UMiOEQfDWstY3w5XQQHnvC9WFyCfP4h4QBissyw==</DQ><InverseQ>EG02S7SADhH1EVT9DD0Z62Y0uY7gIYvxX/uq+IzKSCwB8M2G7Qv9xgZQaQlLpCaeKbux3Y59hHM+KpamGL19Kg==</InverseQ><D>vmaYHEbPAgOJvaEXQl+t8DQKFT1fudEysTy31LTyXjGu6XiltXXHUuZaa2IPyHgBz0Nd7znwsW/S44iql0Fen1kzKioEL3svANui63O3o5xdDeExVM6zOf1wUUh/oldovPweChyoAdMtUzgvCbJk1sYDJf++Nr0FeNW1RB1XG30=</D></RSAKeyValue>";
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            byte[] cipherbytes;
            rsa.FromXmlString(privatekey);
            cipherbytes = rsa.Decrypt(Convert.FromBase64String(content), false);

            return Encoding.UTF8.GetString(cipherbytes);
        }

    }
    #endregion





    #region 其它算法
    public class TestRSA
    {
        public static void Test(string[] arg)
        {

            /**RSA加密测试,RSA中的密钥对通过SSL工具生成，生成命令如下：
             * 1 生成RSA私钥：
             * openssl genrsa -out rsa_private_key.pem 1024
             *2 生成RSA公钥
             * openssl rsa -in rsa_private_key.pem -pubout -out rsa_public_key.pem
             *
             * 3 将RSA私钥转换成PKCS8格式
             * openssl pkcs8 -topk8 -inform PEM -in rsa_private_key.pem -outform PEM -nocrypt -out rsa_pub_pk8.pem
             *
             * 直接打开rsa_private_key.pem和rsa_pub_pk8.pem文件就可以获取密钥对内容，获取密钥对内容组成字符串时，注意将换行符删除
             * */

            string publickey = "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQDzOqfNunFxFtCZPlq7fO/jWwjqmTvAooVBB4y87BizSZ9dl/F7FpAxYc6MmX2TqivCvvORXgdlYdFWAhzXOnIUv9OGG///WPLe9TMs9kIwAZ/APUXauvC01oFLnYkzwPlAh0tQ1Au9arTE/OG1V1dKgf8BXHLPhKL4BmGBEUZBtQIDAQAB";
            string privatekey = "MIICeQIBADANBgkqhkiG9w0BAQEFAASCAmMwggJfAgEAAoGBAPM6p826cXEW0Jk+Wrt87+NbCOqZO8CihUEHjLzsGLNJn12X8XsWkDFhzoyZfZOqK8K+85FeB2Vh0VYCHNc6chS/04Yb//9Y8t71Myz2QjABn8A9Rdq68LTWgUudiTPA+UCHS1DUC71qtMT84bVXV0qB/wFccs+EovgGYYERRkG1AgMBAAECgYEA2PmnPdgnYKnolfvQ9tXiLaBFGPpvGk4grz0r6FB5TF7N4rErwxECunq0xioaowK4HPc40qHd2SvkkWQ7FCjYIDsnMk1oOhxNKn0J3FG0n5Cg1/dFai4eoXHs/nKn3SVZ8YZC1T2cMtN2srectLqNqhB8aQEe8xmykyUlUpg/qmECQQD9vkwjUotG5oUUrOj6etcB4WcdyyH0FtThKgyoJUDwgBv6lGGzWyFJEREvp47IgV+FgC7zeP2mL4MhgnD3tNCZAkEA9WRrjOLBNc379XZpoDsH7rZjobVvhnTrEuRDx/whqZ+vk64EPrEW81XYh647bAbJlFn2jPhY+IUHkrxFEFT/fQJBAMoLNOULXQtfkqgb5odMONeue0Ul8itB4tBHgzyALW1TFPQ6InGGJsLfbCfd67uMCFts7fXAaXhibK/KBdm3iEECQQChwVAjzlUN4nnzk9qMhFz2PcPvFGovd2J9UXpcmRaXeWuDLXIe4Rz/ydaxmWgSDWdTIvoicpIzP31+fBwKZ/0BAkEAy0bh4weKmYF29//rK0sxmY8RtqkQeFrwWbqx1daa1w0DfWlNSvy47zyW1G5/AdZU6JSpXxlxdlM/HSDw+v7kcA==";

            //加密字符串
            string data = "yibao";

            Console.WriteLine("加密前字符串内容：" + data);
            //加密
            string encrypteddata = RSAFromPkcs8.encryptData(data, publickey, "UTF-8");
            Console.WriteLine("加密后的字符串为：" + encrypteddata);
            Console.WriteLine("解密后的字符串内容：" + RSAFromPkcs8.decryptData(encrypteddata, privatekey, "UTF-8"));

            Console.WriteLine("***********");

            //解密
            string endata = "LpnnvnfA72VnyjboX/OsCPO6FOFXeEnnsKkI7aAEQyVAPfCTfQ43ZYVZVqnADDPMW7VhBXJWyQMAGw2Fh9sS/XLHmO5XW94Yehci6JrJMynePgtIiDysjNA+UlgSTC/MlResNrBm/4MMSPvq0qLwScgpZDynhLsVZk+EQ6G8wgA=";
            string datamw = RSAFromPkcs8.decryptData(endata, privatekey, "UTF-8");
            Console.WriteLine("静态加密后的字符串为：" + endata);
            Console.WriteLine("解密后的字符串内容：" + datamw);

            //签名
            string signdata = "YB010000001441234567286038508081299";
            Console.WriteLine("签名前的字符串内容：" + signdata);
            string sign = RSAFromPkcs8.sign(signdata, privatekey, "UTF-8");
            Console.WriteLine("签名后的字符串：" + sign);

            Console.ReadLine();
        }
    }


    /// <summary>
    /// 类名：RSAFromPkcs8
    /// 功能：RSA加密、解密、签名、验签
    /// 详细：该类对Java生成的密钥进行解密和签名以及验签专用类，不需要修改
    /// 版本：3.0
    /// 日期：2013-07-08
    /// 说明：
    /// 以下代码只是为了方便商户测试而提供的样例代码，商户可以根据自己网站的需要，按照技术文档编写,并非一定要使用该代码。
    /// </summary>
    public sealed class RSAFromPkcs8
    {
        /// <summary>
        /// 签名
        /// </summary>
        /// <param name="content">待签名字符串</param>
        /// <param name="privateKey">私钥</param>
        /// <param name="input_charset">编码格式</param>
        /// <returns>签名后字符串</returns>
        public static string sign(string content, string privateKey, string input_charset)
        {
            byte[] Data = Encoding.GetEncoding(input_charset).GetBytes(content);
            RSACryptoServiceProvider rsa = DecodePemPrivateKey(privateKey);
            SHA1 sh = new SHA1CryptoServiceProvider();
            byte[] signData = rsa.SignData(Data, sh);
            return Convert.ToBase64String(signData);
        }

        /// <summary>
        /// 验签
        /// </summary>
        /// <param name="content">待验签字符串</param>
        /// <param name="signedString">签名</param>
        /// <param name="publicKey">公钥</param>
        /// <param name="input_charset">编码格式</param>
        /// <returns>true(通过)，false(不通过)</returns>
        public static bool verify(string content, string signedString, string publicKey, string input_charset)
        {
            bool result = false;
            byte[] Data = Encoding.GetEncoding(input_charset).GetBytes(content);
            byte[] data = Convert.FromBase64String(signedString);
            RSAParameters paraPub = ConvertFromPublicKey(publicKey);
            RSACryptoServiceProvider rsaPub = new RSACryptoServiceProvider();
            rsaPub.ImportParameters(paraPub);
            SHA1 sh = new SHA1CryptoServiceProvider();
            result = rsaPub.VerifyData(Data, sh, data);
            return result;
        }

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="resData">需要加密的字符串</param>
        /// <param name="publicKey">公钥</param>
        /// <param name="input_charset">编码格式</param>
        /// <returns>明文</returns>
        public static string encryptData(string resData, string publicKey, string input_charset)
        {
            byte[] DataToEncrypt = Encoding.ASCII.GetBytes(resData);
            string result = encrypt(DataToEncrypt, publicKey, input_charset);
            return result;
        }


        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="resData">加密字符串</param>
        /// <param name="privateKey">私钥</param>
        /// <param name="input_charset">编码格式</param>
        /// <returns>明文</returns>
        public static string decryptData(string resData, string privateKey, string input_charset)
        {
            byte[] DataToDecrypt = Convert.FromBase64String(resData);
            string result = "";
            for (int j = 0; j < DataToDecrypt.Length / 128; j++)
            {
                byte[] buf = new byte[128];
                for (int i = 0; i < 128; i++)
                {

                    buf[i] = DataToDecrypt[i + 128 * j];
                }
                result += decrypt(buf, privateKey, input_charset);
            }
            return result;
        }

        #region 内部方法

        private static string encrypt(byte[] data, string publicKey, string input_charset)
        {
            RSACryptoServiceProvider rsa = DecodePemPublicKey(publicKey);
            SHA1 sh = new SHA1CryptoServiceProvider();
            byte[] result = rsa.Encrypt(data, false);

            return Convert.ToBase64String(result);
        }

        private static string decrypt(byte[] data, string privateKey, string input_charset)
        {
            string result = "";
            RSACryptoServiceProvider rsa = DecodePemPrivateKey(privateKey);
            SHA1 sh = new SHA1CryptoServiceProvider();
            byte[] source = rsa.Decrypt(data, false);
            char[] asciiChars = new char[Encoding.GetEncoding(input_charset).GetCharCount(source, 0, source.Length)];
            Encoding.GetEncoding(input_charset).GetChars(source, 0, source.Length, asciiChars, 0);
            result = new string(asciiChars);
            //result = ASCIIEncoding.ASCII.GetString(source);
            return result;
        }

        private static RSACryptoServiceProvider DecodePemPublicKey(String pemstr)
        {
            byte[] pkcs8publickkey;
            pkcs8publickkey = Convert.FromBase64String(pemstr);
            if (pkcs8publickkey != null)
            {
                RSACryptoServiceProvider rsa = DecodeRSAPublicKey(pkcs8publickkey);
                return rsa;
            }
            else
                return null;
        }

        private static RSACryptoServiceProvider DecodePemPrivateKey(String pemstr)
        {
            byte[] pkcs8privatekey;
            pkcs8privatekey = Convert.FromBase64String(pemstr);
            if (pkcs8privatekey != null)
            {
                RSACryptoServiceProvider rsa = DecodePrivateKeyInfo(pkcs8privatekey);
                return rsa;
            }
            else
                return null;
        }

        private static RSACryptoServiceProvider DecodePrivateKeyInfo(byte[] pkcs8)
        {
            byte[] SeqOID = { 0x30, 0x0D, 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x01, 0x05, 0x00 };
            byte[] seq = new byte[15];

            MemoryStream mem = new MemoryStream(pkcs8);
            int lenstream = (int)mem.Length;
            BinaryReader binr = new BinaryReader(mem);    //wrap Memory Stream with BinaryReader for easy reading
            byte bt = 0;
            ushort twobytes = 0;

            try
            {
                twobytes = binr.ReadUInt16();
                if (twobytes == 0x8130)    //data read as little endian order (actual data order for Sequence is 30 81)
                    binr.ReadByte();    //advance 1 byte
                else if (twobytes == 0x8230)
                    binr.ReadInt16();    //advance 2 bytes
                else
                    return null;

                bt = binr.ReadByte();
                if (bt != 0x02)
                    return null;

                twobytes = binr.ReadUInt16();

                if (twobytes != 0x0001)
                    return null;

                seq = binr.ReadBytes(15);        //read the Sequence OID
                if (!CompareBytearrays(seq, SeqOID))    //make sure Sequence for OID is correct
                    return null;

                bt = binr.ReadByte();
                if (bt != 0x04)    //expect an Octet string
                    return null;

                bt = binr.ReadByte();        //read next byte, or next 2 bytes is  0x81 or 0x82; otherwise bt is the byte count
                if (bt == 0x81)
                    binr.ReadByte();
                else
                    if (bt == 0x82)
                    binr.ReadUInt16();
                //------ at this stage, the remaining sequence should be the RSA private key

                byte[] rsaprivkey = binr.ReadBytes((int)(lenstream - mem.Position));
                RSACryptoServiceProvider rsacsp = DecodeRSAPrivateKey(rsaprivkey);
                return rsacsp;
            }

            catch (Exception)
            {
                return null;
            }

            finally { binr.Close(); }

        }

        private static bool CompareBytearrays(byte[] a, byte[] b)
        {
            if (a.Length != b.Length)
                return false;
            int i = 0;
            foreach (byte c in a)
            {
                if (c != b[i])
                    return false;
                i++;
            }
            return true;
        }

        private static RSACryptoServiceProvider DecodeRSAPublicKey(byte[] publickey)
        {
            // encoded OID sequence for  PKCS #1 rsaEncryption szOID_RSA_RSA = "1.2.840.113549.1.1.1"
            byte[] SeqOID = { 0x30, 0x0D, 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x01, 0x05, 0x00 };
            byte[] seq = new byte[15];
            // ---------  Set up stream to read the asn.1 encoded SubjectPublicKeyInfo blob  ------
            MemoryStream mem = new MemoryStream(publickey);
            BinaryReader binr = new BinaryReader(mem);    //wrap Memory Stream with BinaryReader for easy reading
            byte bt = 0;
            ushort twobytes = 0;

            try
            {

                twobytes = binr.ReadUInt16();
                if (twobytes == 0x8130) //data read as little endian order (actual data order for Sequence is 30 81)
                    binr.ReadByte();    //advance 1 byte
                else if (twobytes == 0x8230)
                    binr.ReadInt16();   //advance 2 bytes
                else
                    return null;

                seq = binr.ReadBytes(15);       //read the Sequence OID
                if (!CompareBytearrays(seq, SeqOID))    //make sure Sequence for OID is correct
                    return null;

                twobytes = binr.ReadUInt16();
                if (twobytes == 0x8103) //data read as little endian order (actual data order for Bit String is 03 81)
                    binr.ReadByte();    //advance 1 byte
                else if (twobytes == 0x8203)
                    binr.ReadInt16();   //advance 2 bytes
                else
                    return null;

                bt = binr.ReadByte();
                if (bt != 0x00)     //expect null byte next
                    return null;

                twobytes = binr.ReadUInt16();
                if (twobytes == 0x8130) //data read as little endian order (actual data order for Sequence is 30 81)
                    binr.ReadByte();    //advance 1 byte
                else if (twobytes == 0x8230)
                    binr.ReadInt16();   //advance 2 bytes
                else
                    return null;

                twobytes = binr.ReadUInt16();
                byte lowbyte = 0x00;
                byte highbyte = 0x00;

                if (twobytes == 0x8102) //data read as little endian order (actual data order for Integer is 02 81)
                    lowbyte = binr.ReadByte();  // read next bytes which is bytes in modulus
                else if (twobytes == 0x8202)
                {
                    highbyte = binr.ReadByte(); //advance 2 bytes
                    lowbyte = binr.ReadByte();
                }
                else
                    return null;
                byte[] modint = { lowbyte, highbyte, 0x00, 0x00 };   //reverse byte order since asn.1 key uses big endian order
                int modsize = BitConverter.ToInt32(modint, 0);

                byte firstbyte = binr.ReadByte();
                binr.BaseStream.Seek(-1, SeekOrigin.Current);

                if (firstbyte == 0x00)
                {   //if first byte (highest order) of modulus is zero, don't include it
                    binr.ReadByte();    //skip this null byte
                    modsize -= 1;   //reduce modulus buffer size by 1
                }

                byte[] modulus = binr.ReadBytes(modsize);   //read the modulus bytes

                if (binr.ReadByte() != 0x02)            //expect an Integer for the exponent data
                    return null;
                int expbytes = (int)binr.ReadByte();        // should only need one byte for actual exponent data (for all useful values)
                byte[] exponent = binr.ReadBytes(expbytes);

                // ------- create RSACryptoServiceProvider instance and initialize with public key -----
                RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
                RSAParameters RSAKeyInfo = new RSAParameters();
                RSAKeyInfo.Modulus = modulus;
                RSAKeyInfo.Exponent = exponent;
                RSA.ImportParameters(RSAKeyInfo);
                return RSA;
            }
            catch (Exception)
            {
                return null;
            }

            finally { binr.Close(); }

        }

        private static RSACryptoServiceProvider DecodeRSAPrivateKey(byte[] privkey)
        {
            byte[] MODULUS, E, D, P, Q, DP, DQ, IQ;

            // ---------  Set up stream to decode the asn.1 encoded RSA private key  ------
            MemoryStream mem = new MemoryStream(privkey);
            BinaryReader binr = new BinaryReader(mem);    //wrap Memory Stream with BinaryReader for easy reading
            byte bt = 0;
            ushort twobytes = 0;
            int elems = 0;
            try
            {
                twobytes = binr.ReadUInt16();
                if (twobytes == 0x8130)    //data read as little endian order (actual data order for Sequence is 30 81)
                    binr.ReadByte();    //advance 1 byte
                else if (twobytes == 0x8230)
                    binr.ReadInt16();    //advance 2 bytes
                else
                    return null;

                twobytes = binr.ReadUInt16();
                if (twobytes != 0x0102)    //version number
                    return null;
                bt = binr.ReadByte();
                if (bt != 0x00)
                    return null;


                //------  all private key components are Integer sequences ----
                elems = GetIntegerSize(binr);
                MODULUS = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                E = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                D = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                P = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                Q = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                DP = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                DQ = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                IQ = binr.ReadBytes(elems);

                // ------- create RSACryptoServiceProvider instance and initialize with public key -----
                RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
                RSAParameters RSAparams = new RSAParameters();
                RSAparams.Modulus = MODULUS;
                RSAparams.Exponent = E;
                RSAparams.D = D;
                RSAparams.P = P;
                RSAparams.Q = Q;
                RSAparams.DP = DP;
                RSAparams.DQ = DQ;
                RSAparams.InverseQ = IQ;
                RSA.ImportParameters(RSAparams);
                return RSA;
            }
            catch (Exception)
            {
                return null;
            }
            finally { binr.Close(); }
        }

        private static int GetIntegerSize(BinaryReader binr)
        {
            byte bt = 0;
            byte lowbyte = 0x00;
            byte highbyte = 0x00;
            int count = 0;
            bt = binr.ReadByte();
            if (bt != 0x02)        //expect integer
                return 0;
            bt = binr.ReadByte();

            if (bt == 0x81)
                count = binr.ReadByte();    // data size in next byte
            else
                if (bt == 0x82)
            {
                highbyte = binr.ReadByte();    // data size in next 2 bytes
                lowbyte = binr.ReadByte();
                byte[] modint = { lowbyte, highbyte, 0x00, 0x00 };
                count = BitConverter.ToInt32(modint, 0);
            }
            else
            {
                count = bt;        // we already have the data size
            }



            while (binr.ReadByte() == 0x00)
            {    //remove high order zeros in data
                count -= 1;
            }
            binr.BaseStream.Seek(-1, SeekOrigin.Current);        //last ReadByte wasn't a removed zero, so back up a byte
            return count;
        }

        #endregion

        #region 解析.net 生成的Pem
        private static RSAParameters ConvertFromPublicKey(string pemFileConent)
        {

            byte[] keyData = Convert.FromBase64String(pemFileConent);
            if (keyData.Length < 162)
            {
                throw new ArgumentException("pem file content is incorrect.");
            }
            byte[] pemModulus = new byte[128];
            byte[] pemPublicExponent = new byte[3];
            Array.Copy(keyData, 29, pemModulus, 0, 128);
            Array.Copy(keyData, 159, pemPublicExponent, 0, 3);
            RSAParameters para = new RSAParameters();
            para.Modulus = pemModulus;
            para.Exponent = pemPublicExponent;
            return para;
        }

        private static RSAParameters ConvertFromPrivateKey(string pemFileConent)
        {
            byte[] keyData = Convert.FromBase64String(pemFileConent);
            if (keyData.Length < 609)
            {
                throw new ArgumentException("pem file content is incorrect.");
            }

            int index = 11;
            byte[] pemModulus = new byte[128];
            Array.Copy(keyData, index, pemModulus, 0, 128);

            index += 128;
            index += 2;//141
            byte[] pemPublicExponent = new byte[3];
            Array.Copy(keyData, index, pemPublicExponent, 0, 3);

            index += 3;
            index += 4;//148
            byte[] pemPrivateExponent = new byte[128];
            Array.Copy(keyData, index, pemPrivateExponent, 0, 128);

            index += 128;
            index += ((int)keyData[index + 1] == 64 ? 2 : 3);//279
            byte[] pemPrime1 = new byte[64];
            Array.Copy(keyData, index, pemPrime1, 0, 64);

            index += 64;
            index += ((int)keyData[index + 1] == 64 ? 2 : 3);//346
            byte[] pemPrime2 = new byte[64];
            Array.Copy(keyData, index, pemPrime2, 0, 64);

            index += 64;
            index += ((int)keyData[index + 1] == 64 ? 2 : 3);//412/413
            byte[] pemExponent1 = new byte[64];
            Array.Copy(keyData, index, pemExponent1, 0, 64);

            index += 64;
            index += ((int)keyData[index + 1] == 64 ? 2 : 3);//479/480
            byte[] pemExponent2 = new byte[64];
            Array.Copy(keyData, index, pemExponent2, 0, 64);

            index += 64;
            index += ((int)keyData[index + 1] == 64 ? 2 : 3);//545/546
            byte[] pemCoefficient = new byte[64];
            Array.Copy(keyData, index, pemCoefficient, 0, 64);

            RSAParameters para = new RSAParameters();
            para.Modulus = pemModulus;
            para.Exponent = pemPublicExponent;
            para.D = pemPrivateExponent;
            para.P = pemPrime1;
            para.Q = pemPrime2;
            para.DP = pemExponent1;
            para.DQ = pemExponent2;
            para.InverseQ = pemCoefficient;
            return para;
        }
        #endregion

    }
    #endregion
}