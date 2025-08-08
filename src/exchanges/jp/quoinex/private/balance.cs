﻿using Newtonsoft.Json;
using CCXT.NET.Shared.Coin.Private;

namespace CCXT.NET.Quoinex.Private
{
    /// <summary>
    /// 거래소 회원 지갑 정보
    /// </summary>
    public class QBalanceItem : CCXT.NET.Shared.Coin.Private.BalanceItem, IBalanceItem
    {
        /// <summary>
        ///
        /// </summary>
        [JsonProperty(PropertyName = "currency")]
        public override string currency
        {
            get;
            set;
        }

        /// <summary>
        ///
        /// </summary>
        [JsonProperty(PropertyName = "balance")]
        public override decimal free
        {
            get;
            set;
        }
    }
}