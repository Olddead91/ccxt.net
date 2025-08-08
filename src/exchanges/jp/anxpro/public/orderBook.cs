﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using CCXT.NET.Shared.Coin.Public;
using System.Collections.Generic;

namespace CCXT.NET.Anxpro.Public
{
    /// <summary>
    ///
    /// </summary>
    public class AOrderBooks : CCXT.NET.Shared.Coin.Public.OrderBooks, IOrderBooks
    {
        /// <summary>
        ///
        /// </summary>
        public AOrderBooks()
        {
            this.result = new AOrderBook();
        }

        /// <summary>
        ///
        /// </summary>
        [JsonProperty(PropertyName = "data")]
        public new AOrderBook result
        {
            get;
            set;
        }
    }

    /// <summary>
    ///
    /// </summary>
    public class AOrderBook : CCXT.NET.Shared.Coin.Public.OrderBook, IOrderBook
    {
        /// <summary>
        ///
        /// </summary>
        [JsonProperty(PropertyName = "bids")]
        private List<JToken> bidsValue
        {
            set
            {
                this.bids = new List<OrderBookItem>();

                foreach (var _bid in value)
                {
                    var _b = new OrderBookItem
                    {
                        price = _bid["price"].Value<decimal>(),
                        quantity = _bid["amount"].Value<decimal>(),
                        count = 1
                    };

                    _b.amount = _b.quantity * _b.price;
                    this.bids.Add(_b);
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        [JsonProperty(PropertyName = "asks")]
        private List<JToken> asksValue
        {
            set
            {
                this.asks = new List<OrderBookItem>();

                foreach (var _ask in value)
                {
                    var _a = new OrderBookItem
                    {
                        price = _ask["price"].Value<decimal>(),
                        quantity = _ask["amount"].Value<decimal>(),
                        count = 1
                    };

                    _a.amount = _a.quantity * _a.price;
                    this.asks.Add(_a);
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        [JsonProperty(PropertyName = "originBids")]
        public override List<OrderBookItem> bids
        {
            get;
            set;
        }

        /// <summary>
        ///
        /// </summary>
        [JsonProperty(PropertyName = "originAsks")]
        public override List<OrderBookItem> asks
        {
            get;
            set;
        }

        /// <summary>
        ///
        /// </summary>
        public long now
        {
            get;
            set;
        }

        /// <summary>
        ///
        /// </summary>
        public long dataUpdateTime
        {
            get;
            set;
        }
    }
}