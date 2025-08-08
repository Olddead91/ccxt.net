﻿using Newtonsoft.Json;
using CCXT.NET.Shared.Coin.Trade;

namespace CCXT.NET.GateIO.Trade
{
    /// <summary>
    ///
    /// </summary>
    public class GPlaceOrderItem : CCXT.NET.Shared.Coin.Trade.MyOrderItem, IMyOrderItem
    {
        /// <summary>
        ///
        /// </summary>
        [JsonProperty(PropertyName = "result")]
        public bool success
        {
            get;
            set;
        }

        /// <summary>
        ///
        /// </summary>
        [JsonProperty(PropertyName = "msg")]
        public string message
        {
            get;
            set;
        }

        /// <summary>
        ///
        /// </summary>
        [JsonProperty(PropertyName = "orderNumber")]
        public override string orderId
        {
            get;
            set;
        }

        /// <summary>
        ///
        /// </summary>
        [JsonProperty(PropertyName = "rate")]
        public override decimal price
        {
            get;
            set;
        }

        /// <summary>
        ///
        /// </summary>
        [JsonProperty(PropertyName = "leftAmount")]
        public decimal remaining_volume
        {
            get;
            set;
        }

        /// <summary>
        ///
        /// </summary>
        [JsonProperty(PropertyName = "filledAmount")]
        public override decimal filled
        {
            get;
            set;
        }

        /// <summary>
        ///
        /// </summary>
        [JsonProperty(PropertyName = "filledRate")]
        public decimal filledRate
        {
            get;
            set;
        }
    }

    /// <summary>
    ///
    /// </summary>
    public class GCancelOrder : CCXT.NET.Shared.Coin.Trade.MyOrderItem, IMyOrderItem
    {
        /// <summary>
        ///
        /// </summary>
        [JsonProperty(PropertyName = "result")]
        public bool success
        {
            get;
            set;
        }

        /// <summary>
        ///
        /// </summary>
        [JsonProperty(PropertyName = "msg")]
        public string message
        {
            get;
            set;
        }
    }

    /// <summary>
    ///
    /// </summary>
    public class GCancelOrders : CCXT.NET.Shared.Coin.Trade.MyOrderItem, IMyOrderItem
    {
        /// <summary>
        ///
        /// </summary>
        [JsonProperty(PropertyName = "result")]
        public bool success
        {
            get;
            set;
        }

        /// <summary>
        ///
        /// </summary>
        [JsonProperty(PropertyName = "message")]
        public string message
        {
            get;
            set;
        }
    }
}