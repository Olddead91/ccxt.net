﻿using Newtonsoft.Json;
using CCXT.NET.Shared.Coin.Trade;
using CCXT.NET.Shared.Coin.Types;
using System.Collections.Generic;

namespace CCXT.NET.Quoinex.Trade
{
    /// <summary>
    ///
    /// </summary>
    public class QMyTrades : CCXT.NET.Shared.Coin.Trade.MyTrades, IMyTrades
    {
        /// <summary>
        ///
        /// </summary>
        [JsonProperty(PropertyName = "models")]
        public new List<QMyTradeItem> result
        {
            get;
            set;
        }

        /// <summary>
        ///
        /// </summary>
        [JsonProperty(PropertyName = "current_page")]
        public int current_page
        {
            get;
            set;
        }

        /// <summary>
        ///
        /// </summary>
        [JsonProperty(PropertyName = "total_pages")]
        public int total_pages
        {
            get;
            set;
        }
    }

    /// <summary>
    ///
    /// </summary>
    public class QMyTradeItem : CCXT.NET.Shared.Coin.Trade.MyTradeItem, IMyTradeItem
    {
        /// <summary>
        ///
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public override string tradeId
        {
            get;
            set;
        }

        /// <summary>
        ///
        /// </summary>
        [JsonProperty(PropertyName = "quantity")]
        public override decimal quantity
        {
            get;
            set;
        }

        /// <summary>
        ///
        /// </summary>
        [JsonProperty(PropertyName = "price")]
        public override decimal price
        {
            get;
            set;
        }

        /// <summary>
        ///
        /// </summary>
        [JsonProperty(PropertyName = "taker_side")]
        public string taker_side
        {
            get;
            set;
        }

        /// <summary>
        ///
        /// </summary>
        [JsonProperty(PropertyName = "my_side")]
        private string sideValue
        {
            set
            {
                sideType = SideTypeConverter.FromString(value);
            }
        }

        /// <summary>
        ///
        /// </summary>
        [JsonProperty(PropertyName = "created_at")]
        private long timeValue
        {
            set
            {
                timestamp = value * 1000;
            }
        }
    }
}