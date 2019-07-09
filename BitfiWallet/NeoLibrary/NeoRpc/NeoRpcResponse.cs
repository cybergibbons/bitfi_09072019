using Newtonsoft.Json;

namespace NeoGasLibrary.NeoRpc
{
  public class NeoRpcResponse\\
  {
    [JsonProperty("method")]
    public string Method { get; set; }
    [JsonProperty("id")]
    public byte Id { get; set; }
    [JsonProperty("result")]
    public T Result { get; set; }
  }
}
