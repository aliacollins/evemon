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
    public class EVEMonWebClientTests
    {
        [Test]
        public void IsValidURLTests()
        {
            string errorMsg;
            Assert.IsFalse(EVEMonWebClient.IsValidURL(null, out errorMsg), "Null URL string is not valid");
            Assert.IsFalse(EVEMonWebClient.IsValidURL(string.Empty, out errorMsg), "Empty string is not valid");
            Assert.IsFalse(EVEMonWebClient.IsValidURL("This is not a URL", out errorMsg), "Simple text is not valid");
            Assert.IsFalse(EVEMonWebClient.IsValidURL("ftp://FTPIsNotSupported", out errorMsg), "Incorrect scheme");
            Assert.IsTrue(EVEMonWebClient.IsValidURL("http://battleclinic.com", out errorMsg), "URL is valid");
        }

        [Test]
        public void UrlExceptionTests()
        {
            EVEMonWebClient client = new EVEMonWebClient();
            try
            {
                client.DownloadString(null);
                Assert.Fail("Expected ArgumentException for null url");
            }
            catch(Exception ex)
            {
                Assert.IsInstanceOfType(typeof(ArgumentException), ex, "null url");
            }
            try
            {
                client.DownloadString(string.Empty);
                Assert.Fail("Expected ArgumentException for empty string");
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(typeof(ArgumentException), ex, "empty url");
            }
            try
            {
                client.DownloadString("not a url");
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

            EVEMonWebClient client = new EVEMonWebClient();
            string result = client.DownloadString("http://www.battleclinic.com");
            Assert.AreEqual(TestResources.CharacterSheet, result);

            sourceResponseStream.Close();
            
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

            EVEMonWebClient client = new EVEMonWebClient();
            XmlDocument result = client.DownloadXml("http://www.battleclinic.com");
            StringAssert.AreEqualIgnoringCase(contentAsXml.ToString(), result.ToString());

            sourceResponseStream.Close();
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

            EVEMonWebClient client = new EVEMonWebClient();
            try
            {
                client.DownloadXml("http://www.battleclinic.com");
                Assert.Fail("Expected exception was not thrown");
            }
            catch (EVEMonWebException ex)
            {
                Assert.AreEqual(EVEMonWebExceptionStatus.XmlException, ex.Status);
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

            EVEMonWebClient client = new EVEMonWebClient();
            try
            {
                client.DownloadImage("http://www.battleclinic.com");
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
            EVEMonWebClient client = new EVEMonWebClient();
            client.DownloadImageAsync("http://www.battleclinic.com", ImageAysncDownloadTestCompleted, null);
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

            EVEMonWebClient client = new EVEMonWebClient();
            try
            {
                client.DownloadImage("http://www.battleclinic.com");
                Assert.Fail("Expected exception was not thrown");
            }
            catch (EVEMonWebException ex)
            {
                Assert.AreEqual(EVEMonWebExceptionStatus.ImageException, ex.Status);
            }
            finally
            {
                sourceResponseStream.Close();
            }
        }
    }
}
