﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using CCXT.NET.Shared.Coin;
using CCXT.NET.Shared.Coin.Private;
using System.Collections.Generic;
using System.Linq;

namespace CCXT.NET.Huobi.Private
{
    /// <summary>
    ///
    /// </summary>
    public class HBalances : CCXT.NET.Shared.Coin.Private.Balances, IBalances
    {
        /// <summary>
        ///
        /// </summary>
        public HBalances()
        {
            this.result = new List<HBalanceItem>();
        }

        /// <summary>
        ///
        /// </summary>
        public new List<HBalanceItem> result
        {
            get;
            set;
        }

        /// <summary>
        ///
        /// </summary>
        [JsonProperty(PropertyName = "status")]
        private string status
        {
            set
            {
                success = value == "ok" ? true : false;
            }
        }

        /// <summary>
        ///
        /// </summary>
        [JsonProperty(PropertyName = "data")]
        private JObject data
        {
            set
            {
                var _items = value["list"].ToObject<List<HBalanceItem>>();
                result = _items.GroupBy(i => i.currency)
                               .Select(g => new HBalanceItem
                               {
                                   currency = g.First().currency,
                                   free = g.Sum(f => f.free),
                                   used = g.Sum(u => u.used)
                               })
                               .ToList();
            }
        }
    }

    /// <summary>
    ///
    /// </summary>
    public class HBalanceItem : CCXT.NET.Shared.Coin.Private.BalanceItem, IBalanceItem
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
        [JsonProperty(PropertyName = "type")]
        private string type
        {
            get;
            set;
        }

        /// <summary>
        ///
        /// </summary>
        [JsonProperty(PropertyName = "balance")]
        private decimal balance
        {
            set
            {
                if (type == "trade")
                    free = value;
                else if (type == "frozen")
                    used = value;
            }
        }
    }

    /// <summary>
    ///
    /// </summary>
    public class HAccounts : ApiResult<List<HAccountItem>>
    {
        /// <summary>
        ///
        /// </summary>
        public HAccounts()
        {
            this.result = new List<HAccountItem>();
        }

        /// <summary>
        ///
        /// </summary>
        [JsonProperty(PropertyName = "data")]
        public new List<HAccountItem> result
        {
            get;
            set;
        }

        /// <summary>
        ///
        /// </summary>
        [JsonProperty(PropertyName = "status")]
        private string status
        {
            set
            {
                success = value == "ok" ? true : false;
            }
        }
    }

    /// <summary>
    ///
    /// </summary>
    public class HAccountItem
    {
        /// <summary>
        ///
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public string id
        {
            get;
            set;
        }

        /// <summary>
        ///
        /// </summary>
        [JsonProperty(PropertyName = "type")]
        public string type
        {
            get;
            set;
        }

        /// <summary>
        ///
        /// </summary>
        [JsonProperty(PropertyName = "state")]
        public string state
        {
            get;
            set;
        }

        /// <summary>
        ///
        /// </summary>
        [JsonProperty(PropertyName = "user-id")]
        public string userId
        {
            get;
            set;
        }
    }
}