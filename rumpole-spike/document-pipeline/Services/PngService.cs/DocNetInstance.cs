using Docnet.Core;

namespace Services.PngService
{
  public class DocNetInstance
  {
    private IDocLib _docNet { get; }

    public DocNetInstance()
    {
      _docNet = DocLib.Instance;
    }

    public IDocLib GetDocNet()
    {
      return _docNet;
    }

    public void Dispose()
    {
      _docNet.Dispose();
    }
  }

}