using Azure.ServiceBus.Demo.Contracts.Relay;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace Azure.ServiceBus.Demo.Services.Relay
{
    [ServiceBehavior(Name = "ImageManager", Namespace = "http://servicebus/demo/relay/image")]
    public class ImageManager : IImageService, IDisposable
    {
        private const string FileName = "image.jpg";
        private readonly Image _bitmap;

        public ImageManager()
        {
            this._bitmap = Image.FromFile(FileName);
        }

        public void Dispose()
        {
            if (this._bitmap != null)
            {
                this._bitmap.Dispose();
            }
        }

        public Stream GetImage()
        {
            Console.WriteLine();
            Console.WriteLine("##### Echoing: GetImage()");
            Console.WriteLine();
            var stream = new MemoryStream();
            this._bitmap.Save(stream, ImageFormat.Jpeg);

            stream.Position = 0;
            if (WebOperationContext.Current != null)
                WebOperationContext.Current.OutgoingResponse.ContentType = "image/jpeg";

            return stream;
        }
    }
}
