﻿using Newtonsoft.Json;
using CCXT.NET.Shared.Coin.Private;

namespace CCXT.NET.Korbit.Private
{
    /// <summary>
    ///
    /// </summary>
    public class KBalanceItem : CCXT.NET.Shared.Coin.Private.BalanceItem, IBalanceItem
    {
        /// <summary>
        /// The amount of funds you can use.
        /// </summary>
        [JsonProperty(PropertyName = "available")]
        public override decimal free
        {
            get;
            set;
        }

        /// <summary>
        /// The amount of funds that are being used in trade.
        /// </summary>
        [JsonProperty(PropertyName = "trade_in_use")]
        public decimal trade_in_use
        {
            get;
            set;
        }

        /// <summary>
        /// The amount of funds that are being processed for withdrawal.
        /// </summary>
        [JsonProperty(PropertyName = "withdrawal_in_use")]
        public decimal withdrawal_in_use
        {
            get;
            set;
        }
    }
}