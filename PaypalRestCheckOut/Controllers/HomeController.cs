using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace PaypalRestCheckOut.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }


        public ActionResult PaypalPopUpCheckOut()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult PaypalAuthenticate()
        {
            //ViewBag.Message = "Paypal Authenticate.";

            return View();
        }

        [HttpPost]
        public ActionResult PaypalAuthenticate(string username, string password, string PostUrl)
        {

            //  var returnresp= RequestPayPalToken(username, password, PostUrl);
            if (ServicePointManager.SecurityProtocol != SecurityProtocolType.Tls12) ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12; // forced to modern day SSL protocols
            var client = new RestClient(PostUrl) { Encoding = Encoding.UTF8 };
            var authRequest = new RestRequest("", Method.POST) { RequestFormat = DataFormat.Json };
            client.Authenticator = new HttpBasicAuthenticator(username, password);
            authRequest.AddParameter("Content-Type", "application/x-www-form-urlencode");
            authRequest.AddParameter("grant_type", "client_credentials");
            var authResponse = client.Execute(authRequest);

            if (authResponse.IsSuccessful)
            {                
                // You can now deserialise the response to get the token as per the answer from @ryuzaki 
              var payPalTokenModel = JsonConvert.DeserializeObject<PayPalTokenModel>(authResponse.Content);
                ViewBag.ResultMessage = "The credentials are Valid";
                ViewBag.AccessToken = authResponse.Content;
                return View("PaypalAuthenticate", payPalTokenModel);

            }
            else
            {
                ViewBag.ResultMessage = "The credentials are Invalid";
                ViewBag.Error = authResponse.Content;
                return View("PaypalAuthenticate");
            }

         
        }



        //public PayPalTokenModel Approach3(string username, string password, string PostUrl)
        //{
           
        //}




        public async Task<ResponseT> InvokePostAsync<RequestT, ResponseT>(RequestT request, string PostUrl, string username, string password)
        {
            ResponseT result;

            // 'HTTP Basic Auth Post' <http://stackoverflow.com/questions/21066622/how-to-send-a-http-basic-auth-post>
            string clientId = username;
            string secret = password;
            string oAuthCredentials = Convert.ToBase64String(Encoding.Default.GetBytes(clientId + ":" + secret));

            //base uri to PayPAl 'live' or 'stage' based on 'productionMode'
            string uriString = PostUrl;

            HttpClient client = new HttpClient();

            //construct request message
            var h_request = new HttpRequestMessage(HttpMethod.Post, uriString);
            h_request.Headers.Authorization = new AuthenticationHeaderValue("Basic", oAuthCredentials);
            h_request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            h_request.Headers.AcceptLanguage.Add(new StringWithQualityHeaderValue("en_US"));

            h_request.Content = new StringContent("grant_type=client_credentials", UTF8Encoding.UTF8, "application/x-www-form-urlencoded");

            try
            {
                HttpResponseMessage response = await client.SendAsync(h_request);

                //if call failed ErrorResponse created...simple class with response properties
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    //    ErrorResponse errResp = JsonConvert.DeserializeObject<ErrorResponse>(error);
                    //    throw new PayPalException { error_name = errResp.name, details = errResp.details, message = errResp.message };
                }

                var success = await response.Content.ReadAsStringAsync();
                result = JsonConvert.DeserializeObject<ResponseT>(success);
            }
            catch (Exception)
            {
                throw new HttpRequestException("Request to PayPal Service failed.");
            }

            return result;
        }









    

         


    }


    //gets PayPal accessToken
   
    public class PayPalTokenModel
    {
        public string scope { get; set; }
        public string nonce { get; set; }
        public string access_token { get; set; }
        public string token_type { get; set; }
        public string app_id { get; set; }
        public int expires_in { get; set; }
    }









//public Working1() {

//    try
//    {
//        WebRequest req = WebRequest.Create(PostUrl + "?Content-Type=application/x-www-form-urlencoded&grant_type=client_credentials");
//        req.Method = "POST";

//        req.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(username + ":" + password));
//        // req.Credentials = new NetworkCredential(username, password);                            
//        req.UseDefaultCredentials = true;
//        WebResponse wr = req.GetResponse();
//        string objText = string.Empty;
//        if (wr != null)
//        {
//            HttpWebResponse resp = wr as HttpWebResponse;
//            using (var twitpicResponse = resp)
//            {

//                using (var reader = new StreamReader(twitpicResponse.GetResponseStream()))
//                {
//                    JavaScriptSerializer js = new JavaScriptSerializer();
//                    objText += reader.ReadToEnd();

//                }

//            }


//        }
//        else
//        {
//            ViewBag.Message = "No response";
//        }
//        ViewBag.Message = objText;
//    }
//    catch (Exception ex)
//    {
//        ViewBag.Message = "Exception:" + ex.Message;

//    }





//}
#region MyRegion
//public async Task RequestPayPalToken(string username, string password, string PostUrl)
//{
    //        // Discussion about SSL secure channel
    //        // http://stackoverflow.com/questions/32994464/could-not-create-ssl-tls-secure-channel-despite-setting-servercertificatevalida
    //        ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
    //            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

    //            try
    //            {
    //                // ClientId of your Paypal app API
    //                string APIClientId = username;// "**_[your_API_Client_Id]_**";

    //        // secret key of you Paypal app API
    //        string APISecret = password;// "**_[your_API_secret]_**";

    //                using (var client = new System.Net.Http.HttpClient())
    //                {
    //                    var byteArray = Encoding.UTF8.GetBytes(APIClientId + ":" + APISecret);
    //    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

    //                    var url = new Uri(PostUrl, UriKind.Absolute);

    //    client.DefaultRequestHeaders.IfModifiedSince = DateTime.UtcNow;

    //                    var requestParams = new List<KeyValuePair<string, string>>
    //                            {
    //                                new KeyValuePair<string, string>("grant_type", "client_credentials")
    //                            };

    //    var content = new FormUrlEncodedContent(requestParams);
    //    var webresponse = await client.PostAsync(url, content);
    //    var jsonString = await webresponse.Content.ReadAsStringAsync();

    //    // response will deserialized using Jsonconver
    //    var payPalTokenModel = JsonConvert.DeserializeObject<PayPalTokenModel>(jsonString);
    //}
    //            }
    //            catch (System.Exception ex)
    //            {
    //                //TODO: Log connection error
    //            }
    //        }
    //}
    #endregion

}
