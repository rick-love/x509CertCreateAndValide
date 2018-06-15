using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ValidateSignedDoc
{
    class Program
    {
        static void Main(string[] args)
        {
            string xmlString = File.ReadAllText(@"C:\PATH\signedXmlDoc.xml");
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlString);

            X509Certificate2 pubCert = new X509Certificate2(@"C:\PATH\test-cert-public.pem");


            Console.WriteLine(ValidateDocument(doc, pubCert));
            Console.ReadLine();
        }

        public static bool ValidateDocument(XmlDocument doc, X509Certificate2 cert)
        {
            try
            {
                SignedXml signedXml = new SignedXml(doc);
                XmlNode signatureNode = doc.GetElementsByTagName("Signature")[0];
                
                signedXml.LoadXml((XmlElement)signatureNode);
                return signedXml.CheckSignature(cert, true);

            }
            catch
            {
                return false;
            }
            
        }
    }
}
