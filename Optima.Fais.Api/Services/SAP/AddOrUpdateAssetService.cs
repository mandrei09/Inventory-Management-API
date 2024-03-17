//using System.Collections.Generic;
//using System.Linq;
//using System.Net;
//using System.Net.Http;
//using System.Text;
//using System.Text.Json;
//using System.Threading.Tasks;

//namespace Optima.Fais.Api.Services.SAP
//{
//    public class AddOrUpdateAssetService
//    {
//        private const string BaseUrl = @_BASEURL;

//        private static readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
//        {
//            IgnoreNullValues = true,
//            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
//        };

//        private readonly HttpClient _httpClient;

//        public AddOrUpdateAssetService(HttpClient httpClient)
//        {
//            _httpClient = httpClient;
//        }

//        //public async Task<IEnumerable<AddOrUpdateSAPModel>> GetItemsAsync()
//        //{
//        //    using var httpResponse = await _httpClient.GetAsync("/api/TodoItems");

//        //    httpResponse.EnsureSuccessStatusCode();

//        //    using var httpResponseStream = await httpResponse.Content.ReadAsStreamAsync();

//        //    return await JsonSerializer.DeserializeAsync<List<AddOrUpdateSAPModel>>(httpResponseStream, _jsonSerializerOptions);
//        //}

//        //public async Task<AddOrUpdateSAPModel> GetItemAsync(long itemId)
//        //{
//        //    using var httpResponse = await _httpClient.GetAsync($"/api/TodoItems/{itemId}");

//        //    if (httpResponse.StatusCode == HttpStatusCode.NotFound)
//        //        return null;

//        //    httpResponse.EnsureSuccessStatusCode();

//        //    using var httpResponseStream = await httpResponse.Content.ReadAsStreamAsync();

//        //    return await JsonSerializer.DeserializeAsync<AddOrUpdateSAPModel>(httpResponseStream, _jsonSerializerOptions);
//        //}


//        public async Task CreateItemAsync(AddOrUpdateSAPModel todoItem)
//        {
//            var todoItemJson = new StringContent(
//                JsonSerializer.Serialize(todoItem, _jsonSerializerOptions),
//                Encoding.UTF8,
//                "application/json");

//            using var httpResponse =
//                await _httpClient.PostAsync(BaseUrl, todoItemJson);

//            httpResponse.EnsureSuccessStatusCode();
//        }


//        //public async Task SaveItemAsync(AddOrUpdateSAPModel todoItem)
//        //{
//        //    var todoItemJson = new StringContent(
//        //        JsonSerializer.Serialize(todoItem),
//        //        Encoding.UTF8,
//        //        "application/json");

//        //    using var httpResponse =
//        //        await _httpClient.PutAsync($"/api/TodoItems/{todoItem.Id}", todoItemJson);

//        //    httpResponse.EnsureSuccessStatusCode();
//        //}

//        public async Task DeleteItemAsync(long itemId)
//        {
//            using var httpResponse =
//                await _httpClient.DeleteAsync($"/api/TodoItems/{itemId}");

//            httpResponse.EnsureSuccessStatusCode();
//        }
//    }
//}


//public class AddOrUpdateSAPModel
//{
//    public string Sap_function { get; set; }
//	public Data Data { get; set; }
//	public string Options { get; set; }
//    public string Remote_host_name { get; set; }
//    public string GroupDimension { get; set; }
//    public string IsSuspended { get; set; }
//    public string Description { get; set; }
//    public string Owner { get; set; }
//}

//public class Data
//{
//    public IInput[] SAPData { get; set; }
//}

//public class IInput
//{
//    public string XSUBNO { get; set; }
//    public string COMPANYCODE { get; set; }
//    public string ASSET { get; set; }
//    public string SUBNUMBER { get; set; }
//    public string ASSETCLASS { get; set; }
//    public string POSTCAP { get; set; }
//    public string DESCRIPT { get; set; }

//    public string DESCRIPT2 { get; set; }
//    public string INVENT_NO { get; set; }
//    public string SERIAL_NO { get; set; }
//    public int QUANTITY { get; set; }
//    public string BASE_UOM { get; set; }
//    public string LAST_INVENTORY_DATE { get; set; }
//    public string LAST_INVENTORY_DOCNO { get; set; }

//    public string CAP_DATE { get; set; }
//    public string COSTCENTER { get; set; }
//    public string INTERN_ORD { get; set; }
//    public string PLANT { get; set; }
//    public string LOCATION { get; set; }
//    public string ROOM { get; set; }
//    public string PERSON_NO { get; set; }

//    public string PLATE_NO { get; set; }
//    public string ZZCLAS { get; set; }
//    public string IN_CONSERVATION { get; set; }
//    public string PROP_IND { get; set; }
//    public string OPTIMA_ASSET_NO { get; set; }
//    public string OPTIMA_ASSET_PARENT_NO { get; set; }
//    public string TESTRUN { get; set; }
//}

//public class SAPResponse
//{
//    public SAPResponseMeta Meta { get; set; }
//    public SAPResponseData Data { get; set; }
//}


//public class SAPResponseData
//{
//    public string ASSET { get; set; }
//    public string OPTIMA_ASSET_NO { get; set; }
//    public string OPTIMA_ASSET_PARENT_NO { get; set; }
//    public string RETURN_CODE { get; set; }
//    public string RETURN_MESSAGE { get; set; }
//    public string SUBNUMBER { get; set; }
//}

//public class SAPResponseMeta
//{
//    public string Code { get; set; }
//}