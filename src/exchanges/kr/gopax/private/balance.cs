﻿using Newtonsoft.Json;
using CCXT.NET.Shared.Coin.Private;

namespace CCXT.NET.GOPAX.Private
{
    /// <summary>
    /// 거래소 회원 지갑 정보
    /// </summary>
    public class GBalanceItem : CCXT.NET.Shared.Coin.Private.BalanceItem, IBalanceItem
    {
        /// <summary>
        ///
        /// </summary>
        [JsonProperty(PropertyName = "asset")]
        public override string currency
        {
            get;
            set;
        }

        /// <summary>
        ///
        /// </summary>
        [JsonProperty(PropertyName = "avail")]
        public override decimal free
        {
            get;
            set;
        }

        /// <summary>
        ///
        /// </summary>
        [JsonProperty(PropertyName = "used")]
        public override decimal used
        {
            get
            {
                base.used = hold + pendingWithdrawal;
                return base.used;
            }
            set => base.used = value;
        }

        /// <summary>
        /// 미체결 금액
        /// </summary>
        public decimal hold
        {
            get;
            set;
        }

        /// <summary>
        /// 출금 중
        /// </summary>
        public decimal pendingWithdrawal
        {
            get;
            set;
        }
    }
}