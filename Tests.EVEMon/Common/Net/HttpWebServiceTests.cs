using System;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Xml;
using EVEMon.Common.Net;
using NUnit.Framework;
using TypeMock;

namespace Tests.EVEMon.Common.Net
{
    [TestFixture]
    [VerifyMocks]
    public class HttpWebServiceTests
    {
        [Test]
        public void IsValidURLTests()
        {
            string errorMsg;
            Assert.IsFalse(HttpWebService.IsValidURL(null, out errorMsg), "Null URL string is not valid");
            Assert.IsFalse(HttpWebService.IsValidURL(string.Empty, out errorMsg), "Empty string is not valid");
            Assert.IsFalse(HttpWebService.IsValidURL("This is not a URL", out errorMsg), "Simple text is not valid");
            Assert.IsFalse(HttpWebService.IsValidURL("ftp://FTPIsNotSupported", out errorMsg), "Incorrect scheme");
            Assert.IsTrue(HttpWebService.IsValidURL("http://battleclinic.com", out errorMsg), "URL is valid");
        }

        [Test]
        public void UrlExceptionTests()
        {
            HttpWebService service = new HttpWebService();
            try
            {
                service.DownloadString(null);
                Assert.Fail("Expected ArgumentException for null url");
            }
            catch(Exception ex)
            {
                Assert.IsInstanceOfType(typeof(ArgumentException), ex, "null url");
            }
            try
            {
                service.DownloadString(string.Empty);
                Assert.Fail("Expected ArgumentException for empty string");
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(typeof(ArgumentException), ex, "empty url");
            }
            try
            {
                service.DownloadString("not a url");
                Assert.Fail("Expected ArgumentException for invalid url");
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(typeof(ArgumentException), ex, "invalid url");
            }

        }

        [Test]
        public void StringDownloadTest()
        {
            UTF8Encoding encoding = new UTF8Encoding();
            byte[] contentAsBytes = encoding.GetBytes(TestResources.CharacterSheet);
            MemoryStream sourceResponseStream = new MemoryStream();
            sourceResponseStream.Write(contentAsBytes, 0, contentAsBytes.Length);
            sourceResponseStream.Position = 0;
            Mock<HttpWebRequest> mockRequest = MockManager.Mock<HttpWebRequest>(Constructor.NotMocked);
            MockObject<HttpWebResponse> mockResponse = MockManager.MockObject<HttpWebResponse>(Constructor.Mocked);
            mockResponse.ExpectAndReturn("GetResponseStream", sourceResponseStream);
            mockRequest.ExpectAndReturn("GetResponse", mockResponse.Object);

            HttpWebService service = new HttpWebService();
            string result = service.DownloadString("http://www.battleclinic.com");
            Assert.AreEqual(TestResources.CharacterSheet, result);

            sourceResponseStream.Close();
            
        }

        private AutoResetEvent _stringAsyncCompletedTrigger;
        private DownloadStringAsyncResult _stringAsyncDownloadResult = null;

        [Test]
        public void StringAsyncDownloadTest()
        {
            UTF8Encoding encoding = new UTF8Encoding();
            byte[] contentAsBytes = encoding.GetBytes(TestResources.CharacterSheet);
            MemoryStream sourceResponseStream = new MemoryStream();
            sourceResponseStream.Write(contentAsBytes, 0, contentAsBytes.Length);
            sourceResponseStream.Position = 0;
            Mock<HttpWebRequest> mockRequest = MockManager.Mock<HttpWebRequest>(Constructor.NotMocked);
            MockObject<HttpWebResponse> mockResponse = MockManager.MockObject<HttpWebResponse>(Constructor.Mocked);
            mockResponse.ExpectAndReturn("GetResponseStream", sourceResponseStream);
            mockRequest.ExpectAndReturn("GetResponse", mockResponse.Object);

            _stringAsyncCompletedTrigger = new AutoResetEvent(false);
            HttpWebService service = new HttpWebService();
            service.DownloadStringAsync("http://www.battleclinic.com", StringAysncDownloadTestCompleted, null);
            _stringAsyncCompletedTrigger.WaitOne();
            if (_stringAsyncDownloadResult.Error != null)
                Assert.Fail(_stringAsyncDownloadResult.Error.Message);
            Assert.AreEqual(TestResources.CharacterSheet, _stringAsyncDownloadResult.Result);
            sourceResponseStream.Close();
            _stringAsyncCompletedTrigger = null;
            _stringAsyncDownloadResult = null;
        }

        private void StringAysncDownloadTestCompleted(DownloadStringAsyncResult e, object state)
        {
            _stringAsyncDownloadResult = e;
            _stringAsyncCompletedTrigger.Set();
        }

        [Test]
        public void XmlDownloadTest()
        {
            UTF8Encoding encoding = new UTF8Encoding();
            byte[] contentAsBytes = encoding.GetBytes(TestResources.CharacterSheet);
            MemoryStream sourceResponseStream = new MemoryStream();
            sourceResponseStream.Write(contentAsBytes, 0, contentAsBytes.Length);
            sourceResponseStream.Position = 0;
            XmlDocument contentAsXml = new XmlDocument();
            contentAsXml.Load(new StringReader(TestResources.CharacterSheet));

            Mock<HttpWebRequest> mockRequest = MockManager.Mock<HttpWebRequest>(Constructor.NotMocked);
            MockObject<HttpWebResponse> mockResponse = MockManager.MockObject<HttpWebResponse>(Constructor.Mocked);
            mockResponse.ExpectAndReturn("GetResponseStream", sourceResponseStream);
            mockRequest.ExpectAndReturn("GetResponse", mockResponse.Object);

            HttpWebService service = new HttpWebService();
            XmlDocument result = service.DownloadXml("http://www.battleclinic.com");
            StringAssert.AreEqualIgnoringCase(contentAsXml.ToString(), result.ToString());

            sourceResponseStream.Close();
        }

        private AutoResetEvent _xmlAsyncCompletedTrigger;
        private DownloadXmlAsyncResult _xmlAsyncDownloadResult = null;

        [Test]
        public void XmlAsyncDownloadTest()
        {
            UTF8Encoding encoding = new UTF8Encoding();
            byte[] contentAsBytes = encoding.GetBytes(TestResources.CharacterSheet);
            MemoryStream sourceResponseStream = new MemoryStream();
            sourceResponseStream.Write(contentAsBytes, 0, contentAsBytes.Length);
            sourceResponseStream.Position = 0;
            XmlDocument contentAsXml = new XmlDocument();
            contentAsXml.Load(new StringReader(TestResources.CharacterSheet));

            Mock<HttpWebRequest> mockRequest = MockManager.Mock<HttpWebRequest>(Constructor.NotMocked);
            MockObject<HttpWebResponse> mockResponse = MockManager.MockObject<HttpWebResponse>(Constructor.Mocked);
            mockResponse.ExpectAndReturn("GetResponseStream", sourceResponseStream);
            mockRequest.ExpectAndReturn("GetResponse", mockResponse.Object);

            _xmlAsyncCompletedTrigger = new AutoResetEvent(false);
            HttpWebService service = new HttpWebService();
            service.DownloadXmlAsync("http://www.battleclinic.com", null, XmlAysncDownloadTestCompleted, null);
            _xmlAsyncCompletedTrigger.WaitOne();
            if (_xmlAsyncDownloadResult.Error != null)
                Assert.Fail(_xmlAsyncDownloadResult.Error.Message);
            StringAssert.AreEqualIgnoringCase(contentAsXml.ToString(), _xmlAsyncDownloadResult.Result.ToString());
            sourceResponseStream.Close();
            _xmlAsyncCompletedTrigger = null;
            _xmlAsyncDownloadResult = null;
        }

        private void XmlAysncDownloadTestCompleted(DownloadXmlAsyncResult e, object state)
        {
            _xmlAsyncDownloadResult = e;
            _xmlAsyncCompletedTrigger.Set();
        }

        [Test]
        public void XmlDownloadExceptionTest()
        {
            UTF8Encoding encoding = new UTF8Encoding();
            byte[] contentAsBytes = encoding.GetBytes("This is not XML data");
            MemoryStream sourceResponseStream = new MemoryStream();
            sourceResponseStream.Write(contentAsBytes, 0, contentAsBytes.Length);
            sourceResponseStream.Position = 0;

            Mock<HttpWebRequest> mockRequest = MockManager.Mock<HttpWebRequest>(Constructor.NotMocked);
            MockObject<HttpWebResponse> mockResponse = MockManager.MockObject<HttpWebResponse>(Constructor.Mocked);
            mockResponse.ExpectAndReturn("GetResponseStream", sourceResponseStream);
            mockRequest.ExpectAndReturn("GetResponse", mockResponse.Object);

            HttpWebService service = new HttpWebService();
            try
            {
                service.DownloadXml("http://www.battleclinic.com");
                Assert.Fail("Expected exception was not thrown");
            }
            catch (HttpWebServiceException ex)
            {
                Assert.AreEqual(HttpWebServiceExceptionStatus.XmlException, ex.Status);
            }
            finally
            {
                sourceResponseStream.Close();
            }
        }

        [Test]
        public void ImageDownloadTest()
        {
            MemoryStream sourceResponseStream = new MemoryStream();
            TestResources.testimage.Save(sourceResponseStream, ImageFormat.Png);
            sourceResponseStream.Position = 0;

            Mock<HttpWebRequest> mockRequest = MockManager.Mock<HttpWebRequest>(Constructor.NotMocked);
            MockObject<HttpWebResponse> mockResponse = MockManager.MockObject<HttpWebResponse>(Constructor.Mocked);
            mockResponse.ExpectAndReturn("GetResponseStream", sourceResponseStream);
            mockRequest.ExpectAndReturn("GetResponse", mockResponse.Object);

            HttpWebService service = new HttpWebService();
            try
            {
                service.DownloadImage("http://www.battleclinic.com");
            }
            finally
            {
                sourceResponseStream.Close();
            }
        }

        private AutoResetEvent _imageAsyncCompletedTrigger;
        private DownloadImageAsyncResult _imageAsyncDownloadResult = null;

        [Test]
        public void ImageAsyncDownloadTest()
        {
            MemoryStream sourceResponseStream = new MemoryStream();
            TestResources.testimage.Save(sourceResponseStream, ImageFormat.Png);
            sourceResponseStream.Position = 0;

            Mock<HttpWebRequest> mockRequest = MockManager.Mock<HttpWebRequest>(Constructor.NotMocked);
            MockObject<HttpWebResponse> mockResponse = MockManager.MockObject<HttpWebResponse>(Constructor.Mocked);
            mockResponse.ExpectAndReturn("GetResponseStream", sourceResponseStream);
            mockRequest.ExpectAndReturn("GetResponse", mockResponse.Object);

            _imageAsyncCompletedTrigger = new AutoResetEvent(false);
            HttpWebService service = new HttpWebService();
            service.DownloadImageAsync("http://www.battleclinic.com", ImageAysncDownloadTestCompleted, null);
            _imageAsyncCompletedTrigger.WaitOne();
            if (_imageAsyncDownloadResult.Error != null)
                Assert.Fail(_imageAsyncDownloadResult.Error.Message);
            Assert.IsNotNull(_imageAsyncDownloadResult.Result, "No image retrieved");
            _imageAsyncCompletedTrigger = null;
            _imageAsyncDownloadResult = null;
        }

        private void ImageAysncDownloadTestCompleted(DownloadImageAsyncResult e, object state)
        {
            _imageAsyncDownloadResult = e;
            _imageAsyncCompletedTrigger.Set();
        }


        [Test]
        public void ImageDownloadExceptionTest()
        {
            UTF8Encoding encoding = new UTF8Encoding();
            byte[] contentAsBytes = encoding.GetBytes("This is not image data");
            MemoryStream sourceResponseStream = new MemoryStream();
            sourceResponseStream.Write(contentAsBytes, 0, contentAsBytes.Length);
            sourceResponseStream.Position = 0;

            Mock<HttpWebRequest> mockRequest = MockManager.Mock<HttpWebRequest>(Constructor.NotMocked);
            MockObject<HttpWebResponse> mockResponse = MockManager.MockObject<HttpWebResponse>(Constructor.Mocked);
            mockResponse.ExpectAndReturn("GetResponseStream", sourceResponseStream);
            mockRequest.ExpectAndReturn("GetResponse", mockResponse.Object);

            HttpWebService service = new HttpWebService();
            try
            {
                service.DownloadImage("http://www.battleclinic.com");
                Assert.Fail("Expected exception was not thrown");
            }
            catch (HttpWebServiceException ex)
            {
                Assert.AreEqual(HttpWebServiceExceptionStatus.ImageException, ex.Status);
            }
            finally
            {
                sourceResponseStream.Close();
            }
        }
    }
}
