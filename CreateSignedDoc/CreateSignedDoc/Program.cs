using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CreateSignedDoc
{
    class Program
    {
        static void Main(string[] args)
        {

            XmlDocument doc = new XmlDocument();
            using (WebClient client = new WebClient())
            {
                byte[] xmlBytes = client.DownloadData("https://www.wibit.net/labWebService/rest/getCoursesForCurriculum/1");
                doc.LoadXml(Encoding.UTF8.GetString(xmlBytes));
            }

            string pfxPath = @"C:\PATH\test-cert.pfx";
            X509Certificate2 cert = new X509Certificate2(File.ReadAllBytes(pfxPath), "test");

            SignXmlDocumentWithCertificate(doc, cert);


            File.WriteAllText(@"C:\PAth\signedXmlDoc.xml", doc.OuterXml);

            Console.WriteLine(doc.OuterXml);
            Console.ReadLine();
        }
        

        public static void SignXmlDocumentWithCertificate(XmlDocument doc, X509Certificate2 cert)
        {
            var signedXml = new SignedXml(doc);
            signedXml.SigningKey = cert.PrivateKey;
            var reference = new Reference();
            reference.Uri = "";
            reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());
            signedXml.AddReference(reference);

            var keyInfo = new KeyInfo();
            keyInfo.AddClause(new KeyInfoX509Data(cert));

            signedXml.KeyInfo = keyInfo;
            signedXml.ComputeSignature();
            XmlElement xmlSig = signedXml.GetXml();

            doc.DocumentElement.AppendChild(doc.ImportNode(xmlSig, true));
            
        }
    }
}
