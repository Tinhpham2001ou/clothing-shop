﻿using ClothingShop.WEB.DTOs.Requests;
using ClothingShop.WEB.DTOs.Responses;
using ClothingShop.WEB.Utils.E_Payment;
using ClothingShop.WEB.Utils.E_Payment.MomoPayment;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace ClothingShop.WEB.Controllers
{
    [Route("payment/momo")]
    public class MomoPaymentController : Controller
    {
        public MomoPaymentController() { }

        [HttpPost]
        public bool Payment([FromBody] PaymentRequest req)
        {
            try
            {
                //request params need to request to MoMo system
                string endpoint = "https://test-payment.momo.vn/gw_payment/transactionProcessor";
                string partnerCode = "MOMOOJOI20210710";
                string accessKey = "iPXneGmrJH0G8FOP";
                string serectkey = "sFcbSGRSJjwGxwhhcEktCHWYUuTuPNDB";
                string orderInfo = "5s Fashion";
                string returnUrl = "https://localhost:44394/Home/ConfirmPaymentClient";
                string notifyurl = "https://4c8d-2001-ee0-5045-50-58c1-b2ec-3123-740d.ap.ngrok.io/Home/SavePayment"; //lưu ý: notifyurl không được sử dụng localhost, có thể sử dụng ngrok để public localhost trong quá trình test

                string amount = req.TotalMoney.ToString();
                string orderid = DateTime.Now.Ticks.ToString();
                string requestId = DateTime.Now.Ticks.ToString();
                string extraData = "";

                //Before sign HMAC SHA256 signature
                string rawHash = "partnerCode=" +
                    partnerCode + "&accessKey=" +
                    accessKey + "&requestId=" +
                    requestId + "&amount=" +
                    amount + "&orderId=" +
                    orderid + "&orderInfo=" +
                    orderInfo + "&returnUrl=" +
                    returnUrl + "&notifyUrl=" +
                    notifyurl + "&extraData=" +
                    extraData;

                MoMoSecurity crypto = new MoMoSecurity();
                //sign signature SHA256
                string signature = crypto.signSHA256(rawHash, serectkey);

                //build body json request
                JObject message = new JObject
            {
                { "partnerCode", partnerCode },
                { "accessKey", accessKey },
                { "requestId", requestId },
                { "amount", amount },
                { "orderId", orderid },
                { "orderInfo", orderInfo },
                { "returnUrl", returnUrl },
                { "notifyUrl", notifyurl },
                { "extraData", extraData },
                { "requestType", "captureMoMoWallet" },
                { "signature", signature }
            };

                string responseFromMomo = Momo.sendPaymentRequest(endpoint, message.ToString());

                JObject jmessage = JObject.Parse(responseFromMomo);
                var url = jmessage.GetValue("payUrl").ToString();
                HttpContext.Response.Cookies.Append("URL", url, new CookieOptions
                {
                    Path = "/",
                });
                return true;
            }catch (Exception e)
            {
                return false;
            }
        }
    }
}
