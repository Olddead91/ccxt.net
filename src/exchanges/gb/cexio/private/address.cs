﻿using Newtonsoft.Json;
using CCXT.NET.Shared.Coin.Private;

namespace CCXT.NET.CEXIO.Private
{
    /// <summary>
    ///
    /// </summary>
    public class CAddressItem : CCXT.NET.Shared.Coin.Private.AddressItem, IAddressItem
    {
        /// <summary>
        ///
        /// </summary>
        public string ok
        {
            set
            {
                success = value == "ok";
            }
        }

        /// <summary>
        /// 전자지갑 Address
        /// </summary>
        [JsonProperty(PropertyName = "data")]
        public override string address
        {
            get;
            set;
        }
    }
}