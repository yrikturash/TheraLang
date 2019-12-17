﻿using Newtonsoft.Json;

namespace TheraLang.DLL.Models
{
    class LiqPayResponseModel
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("payment_id")]
        public int PaymentId { get; set; }

        [JsonProperty("liqpay_order_id")]
        public string LiqpayOrderId { get; set; }

        [JsonProperty("receiver_commission")]
        public decimal ReceiverCommission { get; set; }
    }
}
