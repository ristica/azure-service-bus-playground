using System.IO;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace Azure.ServiceBus.Demo.Contracts.Relay
{
    [ServiceContract(Name = "IImageService", Namespace = "http://servicebus/demo/relay/image")]
    public interface IImageService
    {
        [OperationContract, WebGet]
        Stream GetImage();
    }

    public interface IImageChannel : IImageService, IClientChannel { }
}
