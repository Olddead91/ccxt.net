﻿using Newtonsoft.Json;
using CCXT.NET.Shared.Coin.Trade;
using CCXT.NET.Shared.Coin.Types;
using CCXT.NET.Shared.Configuration;
using System.Collections.Generic;

namespace CCXT.NET.Zb.Trade
{
    /// <summary>
    ///
    /// </summary>
    public class ZMyTrades : CCXT.NET.Shared.Coin.Trade.MyTrades, IMyTrades
    {
        /// <summary>
        ///
        /// </summary>
        public ZMyTrades()
        {
            this.result = new List<ZMyTradeItem>();
        }

        /// <summary>
        ///
        /// </summary>
        [JsonProperty(PropertyName = "result")]
        public new List<ZMyTradeItem> result
        {
            get;
            set;
        }
    }

    /// <summary>
    ///
    /// </summary>
    public class ZMyTradeItem : CCXT.NET.Shared.Coin.Trade.MyTradeItem, IMyTradeItem
    {
        /// <summary>
        ///
        /// </summary>
        public override string tradeId
        {
            get;
            set;
        }

        /// <summary>
        ///
        /// </summary>
        public override string symbol
        {
            get;
            set;
        }

        /// <summary>
        ///
        /// </summary>
        public override SideType sideType
        {
            get;
            set;
        }

        /// <summary>
        ///
        /// </summary>
        public override OrderType orderType
        {
            get;
            set;
        }

        /// <summary>
        ///
        /// </summary>
        public override long timestamp
        {
            get;
            set;
        }

        /// <summary>
        /// ISO 8601 datetime string with milliseconds
        /// </summary>
        public override string datetime
        {
            get
            {
                return CUnixTime.ConvertToUtcTimeMilli(timestamp).ToString("o");
            }
        }

        /// <summary>
        ///
        /// </summary>
        public override decimal quantity
        {
            get;
            set;
        }

        /// <summary>
        ///
        /// </summary>
        public override decimal price
        {
            get;
            set;
        }

        /// <summary>
        ///
        /// </summary>
        public override decimal amount
        {
            get;
            set;
        }

        /// <summary>
        ///
        /// </summary>
        public override decimal fee
        {
            get;
            set;
        }

        /// <summary>
        ///
        /// </summary>
        public override string orderId
        {
            get;
            set;
        }
    }
}