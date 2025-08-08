﻿using Newtonsoft.Json;
using CCXT.NET.Shared.Coin.Public;
using System.Collections.Generic;

namespace CCXT.NET.Zb.Public
{
    /// <summary>
    ///
    /// </summary>
    public class ZMarkets : CCXT.NET.Shared.Coin.Public.Markets, IMarkets
    {
        /// <summary>
        ///
        /// </summary>
        public ZMarkets()
        {
            this.result = new List<ZMarketItem>();
        }

        /// <summary>
        ///
        /// </summary>
        public new List<ZMarketItem> result
        {
            get;
            set;
        }
    }

    /// <summary>
    ///
    /// </summary>
    public class ZMarketItem : CCXT.NET.Shared.Coin.Public.MarketItem, IMarketItem
    {
        /// <summary>
        ///
        /// </summary>
        public ZMarketItem()
        {
            this.precision = new MarketPrecision();
            this.limits = new MarketLimits
            {
                price = new MarketMinMax(),
                quantity = new MarketMinMax(),
                amount = new MarketMinMax()
            };
        }

        /// <summary>
        ///
        /// </summary>
        [JsonProperty(PropertyName = "priceScale")]
        private int priceScale
        {
            set
            {
                precision.price = value;
            }
        }

        /// <summary>
        ///
        /// </summary>
        [JsonProperty(PropertyName = "amountScale")]
        private int amountScale
        {
            set
            {
                precision.quantity = value;
            }
        }
    }
}