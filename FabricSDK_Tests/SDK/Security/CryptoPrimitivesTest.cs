/*
 *  Copyright 2016 DTCC, Fujitsu Australia Software Technology, IBM - All Rights Reserved.
 *
 *  Licensed under the Apache License, Version 2.0 (the "License");
 *  you may not use this file except in compliance with the License.
 *  You may obtain a copy of the License at
 *    http://www.apache.org/licenses/LICENSE-2.0
 *  Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *  limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Hyperledger.Fabric.SDK.Helper;
using Hyperledger.Fabric.SDK.Security;
using Hyperledger.Fabric.Tests.Helper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using CryptoException = Hyperledger.Fabric.SDK.Exceptions.CryptoException;
using X509Certificate = Org.BouncyCastle.X509.X509Certificate;

namespace Hyperledger.Fabric.Tests.SDK.Security
{
    [TestClass]
    [TestCategory("SDK")]
    public class CryptoPrimitivesTest
    {
        // run End2EndIT test and copy from first peer ProposalResponse ( fabric at
        // commit level 230f3cc )
        public static readonly string PLAIN_TEXT_HEX = "0A205E87B04D3B137E4F2BD2B7E435C96D86E62F3DA5147863A2051F39803AB519BD12E60F0AE30F02046C636363010D6578616D706C655F63632E676F00010D6578616D706C655F63632E676F009D0F0A0D6578616D706C655F63632E676F1201301A880F0A2B0801120F120D6578616D706C655F63632E676F1A160A04696E69740A01610A033130300A01620A03323030120708B2B4FFAF9D2B1ACF0E1F8B08000000000000FFED586D73DA4610CE57F42BB69A26869420C0A49E71EB0F603B2D93165C709A64624FE610075C2D24727702D34EFE7B77EF2410183B4E4CD2990E3B9380B8BBBD67DF9E5DF924F2AFB81C88803FFA6A52AE94CB3FD66A50B6B2FE59AE1E54A0B25FAE552BFBD5F2F30328572ACF0F6A8FA0FCF5202D25569AC947E507DFB56EDC16A07D0B79D169FF0EA3F984CB80F7875C7A03D693C27FE6FB3C9C3A8072DC3E7B0B25F8FE97F659FDFC574F49DFEBC522E83FF3474C847ED4E79ED9F7BADD7979D2ECA41BCD6F9D572D18462042747210C0DA3980274F603C5DA8EE89705DF5CA1ABF66E349C0DFFB7E6918FDD78EFB9FC8B7F0E9DDF55FA9D69E574DFD570EF66BCF0FF6B1FEABD5DAC1AEFEBF85784F9DE36832976238D2D06CFC0EC7919C94A05AAEFC08752CD90E2D28E870C5E594F74B8EF39B406A50BC0F71D8E712F488437DC27CFC48568AF027974A4421544B65C8D3063759720B3F39F32886319B4318698815470542013520E0D73E9F68640BF0234C4AC1429FC34CE891B924515172DE260AA29E46960086BB27F834C8EE02A61D27978391D69343CF9BCD66256640962239F402BB4979BF358F4F5BDDD36708D4715E8501570A24FF100B89F6F5E6C02608C3673D0417B0194412D850725CD3446A3093428B705804150DF48C49EEF485D252F462BDE29F14141A9ADD801E6221B8F52E34BB2E34EADD66B7E8BC6E9EFFDA7E750EAFEB9D4EBD75DE3CED42BB8324DC3A699E37DB2D7C7A01F5D65B78D96C9D1481A377F0127E3D91841D010AF21C05AACBF9CAE583C8825113EE8B81F0D1A27018B32147869E7219A221806D602C14C54E21B4BE1388B1D04C9BE71BE6949CA79EE3A057AF48C91863E1380E5E1F490D7927E7722923A95CFC36186BFA40CBFD289CBA18197788C0E35E0903EDDDEC3E9E1F49EE2DFB8B1A89B1EB141CC7F3A04B06F2E3458348080C94F91D960BE679CC438BDFD178CB8DC38828F635FCE37C749C411CFA98ADF0746D53019AA1D079A5E31E1090D262A58B3F35438DE313F3715BFEDD656FAE31FD8DE105D49A7B5F0426870A0E8F80CE977EE1FA055E4380EA61FF8C4936E6785EE50B4E6ECA24D48BD02050140B1434F734D498635C25EB5316E016FC1FD34FD37A5D29AE6114057D3C93ECC2FB2D0674B41840C0C33CA128C0774750235839C9752C43084590A055A5169FE5DD26DA2525479784F1B887E1C6AAC2A331B95195E0F41A7387321E6A2E2246A7E51002B947B040FC6DF36D11362757872363FFBBF2253E18EC848D9C6132A154D79130D8DE552E0B062CAD234C447627D02512F403C7CC01D41EDB24675997589CB9468AA48A481A7722D9FFBA48B0164A6718603DC8BBE412C4F0B89FC494BE5E846E3113E782F5F16B641AEB5E6C57F42D320FB66A9C5C6A0BA6D859ACBBB4258FB96413329FDAD8D411CB93EA42E13E26DA006F52DDD8ACBAF199AAB33FE27F7717E11F3197F32FABC25B63F72ABC0AA359082A9E106B2125FB38A662A03E1AAA39972C54CC542BD2DB15573061732A052A8B374888023BE3404663A853401A7752C834BAE25F067F9030C667708909417A8EE2E00A03C0BD973B1607136F9860AD10C9CF50DD0E8F308D5A99D250CDDC9350061C21EE3E0FD02A8B1B4372621EA9390127669C5BFF53204C612C31E9923D6A7C6E5DB7F9860F94588B0BE849AC5C7043EF87452666D4AE6B1D634FFD22D474F086F24F458C250982038E7F65986120247A35F57911C638E7428FE3CCC129187BD6397B45D833E6E017E4AB3DBA7CCFE6FFEDB9BC4078572627C9FAEED276B38D898D9E4961A1676C866EB30FBE318B0B59AB6843D3DFAE61169765358882209AD909B30A21D6AD99B6A06231A595B0E89E95CBD506663C87E59FE907269FB21D818C6D9FB40F717EA6176F8ED07DD336466C8AEEA6E338DE1B3A37654E4BE959F22EC54AD92699219CA4B37C56837CC170B63713F310211BB8EEA264165751D17CB2D5DADAA117874184E368A2C626C3FBF5666ED327BFB8A140ADB4F109D31ADB33ADB105D31A779AD658310D237E86B517499B08FC9AFBB1997C736FB636EC601BC3410F2DCED4111B23606418BE24F5D551281326C4603E9EC11B6B1CFE603E7EA01FB6331AF51851E0FF613EF2BCF50929DD976827877527198F1DA5FE2277AD3AABB098B16E6F48B619DDCEFC998EFA50EE5FA7D9CA7668B6B224CFC3257B3ACB266CD2E28ACF978C69B306DFE6B3C99212031E7A18E7599765B961E3DC8BF8CC806BB8D864B0E4F44A4FD141B308A7E9D3C6F0CC2BD6EDB1FA70AF79F91EA14A3A6EDA8E6FF4E2BB9AE796A6446A91E91F782638E222EBA06BEDE0B6A159DEA73F7E598FFB0BAFEE7035A1FDEE3F17EE295973E11E5E6C6A05E615D0456AABE33FF7C2FDE8DEEE8B54F17244DDD8196FBFBF853B2C133FE0DA75F52DF4BBD19E5157BC70EBE69EC5C28D46BBBC37CBE836BF497D142A7EF858193ACF2048306622937D29A4BF2FE50D75A4F1A2ACC660499D0FD194F502D818BF2C1EE33B8A94343966CF2F4BEB101E2BD738CB38E7A3F35FFFA576273BD9C94E76B2939DEC64273BD9C94E1E26FF02F35EC2D1002800000D6578616D706C655F63632E676F000201610003313030016200033230300A0744454641554C54129A072D2D2D2D2D424547494E202D2D2D2D2D0A4D4949436A444343416A4B6741774942416749554245567773537830546D7164627A4E776C654E42427A6F4954307777436759494B6F5A497A6A3045417749770A667A454C4D416B474131554542684D4356564D78457A415242674E5642416754436B4E6862476C6D62334A7561574578466A415542674E564241635444564E680A62694247636D467559326C7A59323878487A416442674E5642416F54466B6C7564475679626D5630494664705A47646C64484D7349456C75597934784444414B0A42674E564241735441316458567A45554D4249474131554541784D4C5A586868625842735A53356A623230774868634E4D5459784D5445784D5463774E7A41770A5768634E4D5463784D5445784D5463774E7A4177576A426A4D517377435159445651514745774A56557A45584D4255474131554543424D4F546D3979644767670A5132467962327870626D45784544414F42674E564241635442314A68624756705A326778477A415A42674E5642416F54456B6835634756796247566B5A3256790A49455A68596E4A70597A454D4D416F474131554543784D44513039514D466B77457759484B6F5A497A6A3043415159494B6F5A497A6A304441516344516741450A4842754B73414F34336873344A4770466669474D6B422F7873494C54734F766D4E32576D77707350485A4E4C36773848576533784350517464472F584A4A765A0A2B433735364B457355424D337977355054666B7538714F42707A43427044414F42674E56485138424166384542414D4342614177485159445652306C424259770A464159494B7759424251554841774547434373474151554642774D434D41774741315564457745422F7751434D414177485159445652304F42425945464F46430A6463555A346573336C746943674156446F794C66567050494D42384741315564497751594D4261414642646E516A32716E6F492F784D55646E3176446D6447310A6E4567514D43554741315564455151654D427943436D31356147397A6443356A62323243446E6433647935746557687663335175593239744D416F47434371470A534D343942414D43413067414D4555434944663948626C34786E337A3445774E4B6D696C4D396C58324671346A5770416152564239374F6D56456579416945410A32356144505148474771324176684B54307776743038635831475447434962666D754C704D774B516A33383D0A2D2D2D2D2D454E44202D2D2D2D2D0A";
        public static readonly string SIGNATURE_HEX = "3045022100BAA3D3DBED52CD5FF2169FE0699E5739983D89A495EE4E5661B0C6ED6AF7914F022009E6D11458E37F44D137BA0F840DC9D7303E569AC9B8F4A2367213F4121C510D";
        public static readonly string PEM_CERT_HEX = "2D2D2D2D2D424547494E2043455254494649434154452D2D2D2D2D0A4D4949434E6A43434164796741774942416749515251692F672B4D79355468732F677536725A494B5844414B42676771686B6A4F50515144416A43426754454C0A4D416B474131554542684D4356564D78457A415242674E5642416754436B4E6862476C6D62334A7561574578466A415542674E564241635444564E68626942470A636D467559326C7A593238784754415842674E5642416F54454739795A7A45755A586868625842735A53356A623230784444414B42674E564241735441304E500A554445634D426F474131554541784D545932457562334A6E4D53356C654746746347786C4C6D4E7662544165467730784E7A45784D5449784D7A51784D5446610A467730794E7A45784D5441784D7A51784D5446614D476B78437A414A42674E5642415954416C56544D524D77455159445651514945777044595778705A6D39790A626D6C684D52597746415944565151484577315459573467526E4A68626D4E7063324E764D517777436759445651514C45774E4454314178487A416442674E560A42414D4D466C567A5A584978514739795A7A45755A586868625842735A53356A623230775754415442676371686B6A4F5051494242676771686B6A4F50514D420A42774E43414152776B7773647A664945753549554F6D6C5A6A4259644755724B566D5841713857757174676E76306375684A4C666F73697277664E38307745740A6B395A637856706C5657703732484A736E5A6A73386C75412B3232756F303077537A414F42674E56485138424166384542414D434234417744415944565230540A4151482F424149774144417242674E5648534D454A4441696743434B6335456947633851566C534665627035594753627372746C7A78486A2F507374626765690A79774F554B7A414B42676771686B6A4F5051514441674E49414442464169454176437773694B374465724A5333647A375A35562B5248644A624D654C625961660A32396234643871467A736F4349483338637A394C7A306B783856615974347453784A4B3550526F695850696A37466C6E794F6248615246330A2D2D2D2D2D454E442043455254494649434154452D2D2D2D2D";
        public static readonly string INVALID_PEM_CERT = "2D2D224547494E202D2D2D2D2D0A4D4949436A444343416A4B6741774942416749554245567773537830546D7164627A4E776C654E42427A6F4954307777436759494B6F5A497A6A3045417749770A667A454C4D416B474131554542684D4356564D78457A415242674E5642416754436B4E6862476C6D62334A7561574578466A415542674E564241635444564E680A62694247636D467559326C7A59323878487A416442674E5642416F54466B6C7564475679626D5630494664705A47646C64484D7349456C75597934784444414B0A42674E564241735441316458567A45554D4249474131554541784D4C5A586868625842735A53356A623230774868634E4D5459784D5445784D5463774E7A41770A5768634E4D5463784D5445784D5463774E7A4177576A426A4D517377435159445651514745774A56557A45584D4255474131554543424D4F546D3979644767670A5132467962327870626D45784544414F42674E564241635442314A68624756705A326778477A415A42674E5642416F54456B6835634756796247566B5A3256790A49455A68596E4A70597A454D4D416F474131554543784D44513039514D466B77457759484B6F5A497A6A3043415159494B6F5A497A6A304441516344516741450A4842754B73414F34336873344A4770466669474D6B422F7873494C54734F766D4E32576D77707350485A4E4C36773848576533784350517464472F584A4A765A0A2B433735364B457355424D337977355054666B7538714F42707A43427044414F42674E56485138424166384542414D4342614177485159445652306C424259770A464159494B7759424251554841774547434373474151554642774D434D41774741315564457745422F7751434D414177485159445652304F42425945464F46430A6463555A346573336C746943674156446F794C66567050494D42384741315564497751594D4261414642646E516A32716E6F492F784D55646E3176446D6447310A6E4567514D43554741315564455151654D427943436D31356147397A6443356A62323243446E6433647935746557687663335175593239744D416F47434371470A534D343942414D43413067414D4555434944663948626C34786E337A3445774E4B6D696C4D396C58324671346A5770416152564239374F6D56456579416945410A32356144505148474771324176684B54307776743038635831475447434962666D754C704D774B516A33383D0A2D2D2D2D2D454E44202D2D2D2D2D0A";
        private static readonly string SIGNING_ALGORITHM = "SHA256withECDSA";

        // File create_key_cert_for_testing.md has info on the other keys and
        // certificates used in this test suite

        private static byte[] plainText, sig, pemCert, invalidPemCert;


        private static CryptoPrimitives crypto;

        private static X509Certificate2 testCACert;

        private static Config config;


        // Create a temp folder to hold temp files for various file I/O operations
        // These are automatically deleted when each test completes

        public readonly string tempFolder = Path.GetTempPath();

        [ClassInitialize]
        public static void SetUpBeforeClass(TestContext context)
        {
            config = Config.Instance;

            plainText = PLAIN_TEXT_HEX.FromHexString();
            sig = SIGNATURE_HEX.FromHexString();
            pemCert = PEM_CERT_HEX.FromHexString();
            invalidPemCert = INVALID_PEM_CERT.FromHexString();

            crypto = new CryptoPrimitives();
            crypto.Init();

            // TODO should do this in @BeforeClass. Need to find out how to get to
            // files from static junit method

            testCACert = new X509Certificate2("Resources/ca.crt".Locate());
            crypto.Store.AddCertificate(testCACert);

            X509Certificate2 cert = GenerateFromCertAndPriv();


            crypto.Store.AddCertificate(cert);
            //        crypto.getTrustStore().setKeyEntry("key", key, "123456".toCharArray(), certificates);
            //        pem.close();
        }

        private static X509Certificate2 GenerateFromCertAndPriv()
        {
            X509CertificateEntry entry = null;
            AsymmetricCipherKeyPair privKey = null;
            using (Stream ms = File.OpenRead("Resources/keypair-signed.crt".Locate()))
            {
                PemReader pemReader = new PemReader(new StreamReader(ms));
                object o;
                while ((o = pemReader.ReadObject()) != null)
                {
                    if (o is X509Certificate)
                    {
                        entry = new X509CertificateEntry((X509Certificate) o);
                    }
                }
            }

            using (Stream ms = File.OpenRead("Resources/keypair-signed.key".Locate()))
            {
                PemReader pemReader = new PemReader(new StreamReader(ms));
                object o;
                while ((o = pemReader.ReadObject()) != null)
                {
                    if (o is AsymmetricCipherKeyPair)
                    {
                        privKey = (AsymmetricCipherKeyPair) o;
                    }
                }
            }

            if (entry != null && privKey != null)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    Pkcs12Store store = new Pkcs12StoreBuilder().Build();
                    store.SetKeyEntry("key2", new AsymmetricKeyEntry(privKey.Private), new[] {entry});
                    store.Save(ms, "123456".ToCharArray(), new SecureRandom());
                    ms.Flush();
                    ms.Position = 0;
                    return new X509Certificate2(ms.ToArray(), "123456", X509KeyStorageFlags.Exportable | X509KeyStorageFlags.EphemeralKeySet);
                }
            }

            return null;
        }

        /*

        // Tests initializing with an invalid certificate format
        [TestMethod]
        [ExpectedException(typeof(CryptoException))]
        public void TestInitInvalidCertFormat()
        {
            string oldVal = null;

            try
            {
                // Set the cert format to something invalid
                oldVal = Config.sdkProperties[Config.CERTIFICATE_FORMAT];
                Config.sdkProperties[Config.CERTIFICATE_FORMAT] = "abc123";
                CryptoPrimitives crypto2 = new CryptoPrimitives();
                crypto2.Init();
            }
            finally
            {
                // Reset the property for subsequent tests
                Config.sdkProperties[Config.CERTIFICATE_FORMAT] = oldVal;
            }
        }
        */
        [TestMethod]
        public void TestDefaultCrypto()
        {
            ICryptoSuite cryptoSuite = Factory.Instance.GetCryptoSuite();
            CryptoPrimitives primitives = (CryptoPrimitives) cryptoSuite;
            Assert.AreEqual("secp256r1", primitives.curveName);
            Assert.AreEqual(256, primitives.securityLevel);
            Assert.AreEqual("SHA2", primitives.hashAlgorithm);
            // Should be exactly same instance as it has the same properties.
            Assert.AreEqual(cryptoSuite, Factory.Instance.GetCryptoSuite());
        }

        [TestMethod]
        public void TestGetSetProperties()
        {
            Properties propsIn = new Properties();
            try
            {
                string expectHash = "SHA3"; // use something different than default!
                propsIn.Set(Config.SECURITY_LEVEL, "384");
                propsIn.Set(Config.HASH_ALGORITHM, expectHash);
                //    testCrypto.setProperties(propsIn);
                //   testCrypto.init();
                ICryptoSuite testCrypto = Factory.Instance.GetCryptoSuite(propsIn);

                //          Assert.AreEqual(BouncyCastleProvider.class, getField(testCrypto, "SECURITY_PROVIDER").getClass());

                string expectedCurve = config.GetSecurityCurveMapping()[384];
                CryptoPrimitives original = (CryptoPrimitives) testCrypto;
                Assert.AreEqual("secp384r1", expectedCurve);
                Assert.AreEqual(expectedCurve, original.curveName);
                Assert.AreEqual(384, original.securityLevel);
                Properties cryptoProps = original.GetProperties();
                Assert.AreEqual(cryptoProps[Config.SECURITY_LEVEL], "384");
                cryptoProps = testCrypto.GetProperties();
                Assert.AreEqual(cryptoProps[Config.HASH_ALGORITHM], expectHash);
                Assert.AreEqual(expectHash, original.hashAlgorithm);
                Assert.AreEqual(cryptoProps[Config.SECURITY_LEVEL], "384");

                // Should be exactly same instance as it has the same properties.
                Assert.AreEqual(testCrypto, Factory.Instance.GetCryptoSuite(propsIn));
            }
            catch (CryptoException e)
            {
                Assert.Fail($"TestGetSetProperties should not throw exception. Error: {e.Message}");
            }
            catch (ArgumentException e)
            {
                Assert.Fail($"TestGetSetProperties should not throw exception. Error: {e.Message}");
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestSecurityLevel()
        {
            CryptoPrimitives testCrypto = new CryptoPrimitives();
            testCrypto.SetSecurityLevel(2001);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestSetHashAlgorithm()
        {
            CryptoPrimitives testCrypto = new CryptoPrimitives();
            testCrypto.SetHashAlgorithm(null);
        }


        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestSetHashAlgorithmBadArg()
        {
            CryptoPrimitives testCrypto = new CryptoPrimitives();
            testCrypto.SetHashAlgorithm("FAKE");
        }

        [TestMethod]
        public void TestGetTrustStore()
        {
            // getTrustStore should have created a KeyStore if setTrustStore hasn't
            // been called
            try
            {
                CryptoPrimitives myCrypto = new CryptoPrimitives();
                Assert.IsNotNull(myCrypto.Store);
            }
            catch (CryptoException e)
            {
                Assert.Fail($"getTrustStore() fails with : {e.Message}");
            }
        }

        [TestMethod]
        public void TestGetTrustStoreEntries()
        {
            // trust store should contain the entries
            try
            {
                Assert.IsNotNull(crypto.Store.Certificates.Select(a => a.X509Certificate2).Contains(testCACert));
                Assert.IsNull(crypto.Store.Certificates.Select(a => a.X509Certificate2).FirstOrDefault(a => a.IssuerName.Name == "testtesttest"));
            }
            catch (System.Exception e)
            {
                Assert.Fail($"TestGetTrustStoreEntries should not have thrown exception. Error: {e.Message}");
            }
        }

        [TestMethod]
        public void TestSetTrustStoreNull()
        {
            try
            {
                CryptoPrimitives myCrypto = new CryptoPrimitives();
                myCrypto.Store = null;
                Assert.Fail("setTrustStore(null) should have thrown exception");
            }
            catch (System.Exception)
            {
                // ignored
            }
        }

        [TestMethod]
        public void TestSetTrustStore()
        {
            try
            {
                CryptoPrimitives myCrypto = new CryptoPrimitives();
                KeyStore keyStore = new KeyStore();
                //     myCrypto.setTrustStore(keyStore);
                myCrypto.Store = keyStore;
                Assert.AreEqual(keyStore, myCrypto.Store);
            }
            catch (System.Exception e)
            {
                Assert.Fail($"testSetTrustStore() should not have thrown Exception. Error: {e.Message}");
            }
        }

        [TestMethod]
        public void TestSetTrustStoreDuplicateCert()
        {
            try
            {
                crypto.Store.AddCertificate(testCACert); //KeyStore overrides existing cert if same alias
            }
            catch (System.Exception e)
            {
                Assert.Fail($"TestSetTrustStoreDuplicateCert should not have thrown Exception. Error: {e.Message}");
            }
        }

        [TestMethod]
        public void TestSetTrustStoreDuplicateCertUsingFile()
        {
            try
            {
                // Read the certificate data
                string certData = File.ReadAllText("Resources/ca.crt".Locate());
                // Write this to a temp file
                string newFile = Path.Combine(Path.GetFullPath(tempFolder), "temp.txt");
                File.WriteAllText(newFile, certData);
                crypto.Store.AddCertificateFromFile(newFile); //KeyStore overrides existing cert if same alias
            }
            catch (System.Exception e)
            {
                Assert.Fail($"TestSetTrustStoreDuplicateCert should not have thrown Exception. Error: {e.Message}");
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestAddCACertificateToTrustStoreNullAlias()
        {
            try
            {
                crypto.Store.AddCertificateFromFile("something");
            }
            catch (CryptoException e)
            {
                Assert.Fail($"TestAddCACertificateToTrustStoreNullAlias should not throw CryptoException. Error: {e.Message}");
            }
        }

        /*
        [TestMethod]
        [ExpectedException(typeof(InvalidArgumentException))]
        public void TestAddCACertificateToTrustStoreBlankAlias()
        {
            try
            {
                crypto.Store.AddCertificateFromFile("something");
            }
            catch (CryptoException e)
            {
                Assert.Fail($"TestAddCACertificateToTrustStoreNoAlias should not throw CryptoException. Error: {e.Message}");
            }
        }
        */
        /*
        [TestMethod]
        [ExpectedExceptionWithMessage(typeof(CryptoException), "Unable to add")]
        public void TestAddCACertificateToTrustStoreBadStore()
        {
            // Create an uninitialized key store
            KeyStore tmpKeyStore = new  KeyStore();
            KeyStore saveKeyStore = crypto.Store;
            crypto.Store = tmpKeyStore;
            try
            {
                crypto.Store.AddCertificate(testCACert);
            }
            finally
            {
                // Ensure we set it back so that subsequent tests will not be affected
                crypto.Store = saveKeyStore;
            }
        }
        */
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestAddCACertificateToTrustStoreNoFile()
        {
            try
            {
                crypto.Store.AddCertificateFromFile("does/not/exist");
            }
            catch (CryptoException e)
            {
                Assert.Fail($"testAddCACertificateToTrustStoreNoFile should not throw CryptoException. Error: {e.Message}");
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestAddCACertificateToTrustStoreInvalidCertFile()
        {
            try
            {
                crypto.Store.AddCertificateFromFile("/bad-ca1.crt");
            }
            catch (CryptoException e)
            {
                Assert.Fail($"testAddCACertificateToTrustStoreInvalidCertFile should not throw CryptoException. Error: {e.Message}");
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestAddCACertificateToTrustStoreNoCert()
        {
            try
            {
                crypto.Store.AddCertificate((X509Certificate2) null);
            }
            catch (CryptoException e)
            {
                Assert.Fail($"testAddCACertificateToTrustStoreNoCert should not have thrown CryptoException. Error {e.Message}");
            }
        }

        /*
        // Tests addCACertificateToTrustStore passing a certificate and null for alias
        [TestMethod]
        [ExpectedExceptionWithMessage(typeof(InvalidArgumentException), "You must assign an alias")]
        public void TestAddCACertificateToTrustStoreCertNullAlias()
        {
            crypto.Store.AddCertificate(testCACert);
        }

        // Tests addCACertificateToTrustStore passing a certificate and an empty string for alias
        [TestMethod]
        [ExpectedExceptionWithMessage(typeof(InvalidArgumentException), "You must assign an alias")]
        public void TestAddCACertificateToTrustStoreCertEmptyAlias()
        {
            crypto.AddCACertificateToTrustStore(testCACert, "");
        }
        */
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestAddCACertificateToTrustStoreNullFile()
        {
            crypto.Store.AddCertificateFromFile(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestLoadCACertsBadInput()
        {
            crypto.Store.AddCertificate((X509Certificate2) null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestLoadCACertsBytesBadInput()
        {
            crypto.Store.AddCertificate((string) null);
        }

        [TestMethod]
        public void TestValidateNullCertificateByteArray()
        {
            Assert.IsFalse(crypto.Store.Validate((string) null));
        }

        [TestMethod]
        public void TestValidateNullCertificate()
        {
            Assert.IsFalse(crypto.Store.Validate((X509Certificate2) null));
        }

        [TestMethod]
        public void TestValidateCertificateByteArray()
        {
            Assert.IsTrue(crypto.Store.Validate(pemCert.ToUTF8String()));
        }

        // Note:
        // For the validateBADcertificate tests, we use the fact that the trustStore
        // contains the peer CA cert
        // the keypair-signed cert is signed by us so it will not validate.
        [TestMethod]
        public void TestValidateBadCertificateByteArray()
        {
            try
            {
                string certBytes = File.ReadAllText("Resources/notsigned.crt".Locate());

                Assert.IsFalse(crypto.Store.Validate(certBytes));
            }
            catch (IOException)
            {
                Assert.Fail("cannot read cert file");
            }
        }

        [TestMethod]
        [ExpectedException(typeof(CryptoException))]
        public void TestBytesToCertificateInvalidBytes()
        {
            Certificate.Create(INVALID_PEM_CERT.FromHexString().ToUTF8String());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestBytesToCertificateNullBytes()
        {
            Certificate.Create((string) null);
        }

        [TestMethod]
        [ExpectedException(typeof(CryptoException))]
        public void TestBytesToPrivateKeyInvalidBytes()
        {
            KeyPair.Create(INVALID_PEM_CERT.FromHexString().ToUTF8String());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestBytesToPrivateKeyNullBytes()
        {
            KeyPair.Create((string) null);
        }

        [TestMethod]
        public void TestBytesToPrivateKey()
        {
            try
            {
                string bytes = File.ReadAllText("Resources/tls-client.key".Locate());
                KeyPair.Create(bytes);
            }
            catch (System.Exception e)
            {
                Assert.Fail($"failed to parse private key bytes: {e.Message}");
            }
        }

        [TestMethod]
        public void TestBytesToPrivateKeyPKCS8()
        {
            try
            {
                string bytes = File.ReadAllText("Resources/tls-client-pk8.key".Locate());
                KeyPair.Create(bytes);
            }
            catch (System.Exception e)
            {
                Assert.Fail($"failed to parse private key bytes: {e.Message}");
            }
        }

        [TestMethod]
        public void TestValidateNotSignedCertificate()
        {
            bool res = true;
            try
            {
                X509Certificate2 cert = new X509Certificate2("Resources/notsigned.crt".Locate());
                res = crypto.Store.Validate(cert);
            }
            catch (System.Exception)
            {
                Assert.Fail("cannot read cert file");
            }

            Assert.IsFalse(res);
        }

        [TestMethod]
        public void TestValidateInvalidCertificate()
        {
            Assert.IsFalse(crypto.Store.Validate(invalidPemCert.ToUTF8String()));
        }

        [TestMethod]
        public void TestValidateCertificate()
        {
            try
            {
                Assert.IsTrue(crypto.Store.Validate(pemCert.ToUTF8String()));
            }
            catch (System.Exception)
            {
                Assert.Fail("cannot read cert file");
            }
        }

        [TestMethod]
        public void TestVerifyNullInput()
        {
            try
            {
                Assert.IsFalse(crypto.Verify(null, SIGNING_ALGORITHM, null, null));
            }
            catch (CryptoException e)
            {
                Assert.Fail($"testVerifyNullInput should not have thrown exception. Error: {e.Message}");
            }
        } // testVerifyNullInput

        [TestMethod]
        [ExpectedException(typeof(CryptoException))]
        public void TestVerifyBadCert()
        {
            byte[] badCert = new byte[] { 0x00};
            crypto.Verify(badCert, SIGNING_ALGORITHM, sig, plainText);
        } // testVerifyBadCert

        [TestMethod]
        [ExpectedException(typeof(CryptoException))]
        public void TestVerifyBadSig()
        {
            byte[] badSig = new byte[] {0x00};
            bool res = crypto.Verify(pemCert, SIGNING_ALGORITHM, badSig, plainText);
            if (!res)
                throw new CryptoException("Not Verified");
        } // testVerifyBadSign

        [TestMethod]
        public void TestVerifyBadPlaintext()
        {
            byte[] badPlainText = new byte[] {0x00};
            try
            {
                Assert.IsFalse(crypto.Verify(pemCert, SIGNING_ALGORITHM, sig, badPlainText));
            }
            catch (CryptoException e)
            {
                Assert.Fail($"testVerifyBadPlaintext should not have thrown exception. Error: {e.Message}");
            }
        } // testVerifyBadPlainText

        [TestMethod]
        public void TestVerify()
        {
            try
            {
                Assert.IsTrue(crypto.Verify(pemCert, SIGNING_ALGORITHM, sig, plainText));
            }
            catch (CryptoException e)
            {
                Assert.Fail($"testVerify should not have thrown exception. Error: {e.Message}");
            }
        } // testVerify

        [TestMethod]
        public void TestSignNullKey()
        {
            try
            {
                crypto.Sign(null, new byte[] { 0x00});
                Assert.Fail("sign() should have thrown an exception");
            }
            catch (ArgumentException)
            {
            }
        }

        private KeyPair FindRightKey()
        {
            foreach (X509Certificate2 cert in crypto.Store.Certificates.Select(a => a.X509Certificate2))
            {
                if (cert.HasPrivateKey && cert.FriendlyName == "Hyperledger.Fabric")
                {
                    byte[] certidata = cert.Export(X509ContentType.Pkcs12, "123");
                    Pkcs12Store store = new Pkcs12Store();
                    store.Load(new MemoryStream(certidata), "123".ToCharArray());
                    AsymmetricKeyEntry parameter = null;
                    List<string> alias = store.Aliases.Cast<string>().ToList();
                    foreach (string s in alias)
                    {
                        if (store.IsKeyEntry(s))
                            parameter = store.GetKey(s);
                    }

                    if (parameter == null)
                        return null;
                    return KeyPair.Create(null, parameter.Key);
                }
            }

            return null;
        }

        [TestMethod]
        public void TestSignValidData()
        {
            KeyPair key = FindRightKey();

            crypto.Sign(key, plainText);
            Assert.IsTrue(crypto.Sign(key, plainText).Length > 0);
        }

        [TestMethod]
        public void TestSignNullData()
        {
            try
            {
                KeyPair key = FindRightKey();
                crypto.Sign(key, null);
                Assert.Fail("sign() should have thrown an exception");
            }
            catch (ArgumentException)
            {
            }
            catch (System.Exception e)
            {
                Assert.Fail($"Could not create private key. Error: {e.Message}");
            }
        }

        [TestMethod]
        [Ignore]
        // TODO need to regen key now that we're using CryptoSuite
        public void TestSign()
        {
            byte[] pText = "123456".ToBytes();
            byte[] signature;
            try
            {
                KeyPair key = FindRightKey();
                signature = crypto.Sign(key, pText);
                byte[] cert = File.ReadAllBytes("Resources/keypair-signed.crt".Locate());
                Assert.IsTrue(crypto.Verify(cert, SIGNING_ALGORITHM, signature, pText));
            }
            catch (System.Exception e)
            {
                Assert.Fail($"Could not verify signature. Error: {e.Message}");
            }
        }

        [TestMethod]
        public void TestKeyGen()
        {
            Assert.IsNotNull(crypto.KeyGen());
            Assert.AreSame(typeof(KeyPair), crypto.KeyGen().GetType());
        }

        // Try to generate a key without initializing crypto
        [TestMethod]
        [ExpectedExceptionWithMessage(typeof(CryptoException), "Unable to generate")]
        public void TestKeyGenBadCrypto()
        {
            CryptoPrimitives tmpCrypto = new CryptoPrimitives();
            tmpCrypto.KeyGen();
        }

        [TestMethod]
        public void TestGenerateCertificateRequest()
        {
            KeyPair testKeyPair = crypto.KeyGen();
            Assert.AreSame(typeof(string), crypto.GenerateCertificationRequest("common name", testKeyPair).GetType());
        }

        [TestMethod]
        public void TestCertificationRequestToPEM()
        {
            KeyPair testKeyPair = crypto.KeyGen();
            string certRequest = crypto.GenerateCertificationRequest("common name", testKeyPair);
            // Assert.AreSame(String.class, crypto.certificationRequestToPEM(certRequest).getClass());

            Assert.IsTrue(certRequest.Contains("BEGIN CERTIFICATE REQUEST"));
        }

        [TestMethod]
        public void TestCertificateToDER()
        {
            KeyPair testKeyPair = crypto.KeyGen();
            string certRequest = crypto.GenerateCertificationRequest("common name", testKeyPair);
            //  String pemGenCert = crypto.certificationRequestToPEM(certRequest);
            Assert.IsTrue(Certificate.ExtractDER(certRequest).Length > 0);
        }

        [TestMethod]
        public void TestHashSHA2()
        {
            byte[] input = "TheQuickBrownFox".ToBytes();
            string expectedHash = "cd0b1763383f460e94a2e6f0aefc3749bbeec60db11c12d678c682da679207ad";

            crypto.SetHashAlgorithm("SHA2");
            byte[] hash = crypto.Hash(input);
            Assert.AreEqual(expectedHash, hash.ToHexString());
        }

        [TestMethod]
        public void TestHashSHA3()
        {
            byte[] input = "TheQuickBrownFox".ToBytes();
            string expectedHash = "feb69c5c360a15802de6af23a3f5622da9d96aff2be78c8f188cce57a3549db6";

            crypto.SetHashAlgorithm("SHA3");
            byte[] hash = crypto.Hash(input);
            Assert.AreEqual(expectedHash, hash.ToHexString());
        }

        /**
         * Test makes sure we validate a certificate that has non-standard attributes as FabricCA generates.
         *
         * @throws Exception
         */
        [TestMethod]
        public void TestValidationOfCertWithFabicCAattributes()
        {
            ICryptoSuite cryptoSuite = Factory.Instance.GetCryptoSuite();
            Certificate onceFailingPem = Certificate.Create(File.ReadAllText("Fixture/testPems/peerCert.pem".Locate()));
            CryptoPrimitives cryptoPrimitives = (CryptoPrimitives) cryptoSuite;
            cryptoPrimitives.Store.AddCertificateFromFile("fixture/testPems/caBundled.pems".Locate());
            Assert.IsTrue(cryptoPrimitives.Store.Validate(onceFailingPem));
        }
    }
}