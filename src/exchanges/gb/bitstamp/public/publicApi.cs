﻿using Newtonsoft.Json.Linq;
using CCXT.NET.Shared.Coin;
using CCXT.NET.Shared.Coin.Public;
using CCXT.NET.Shared.Coin.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CCXT.NET.Bitstamp.Public
{
    /// <summary>
    /// exchange's public API implement class
    /// </summary>
    public class PublicApi : CCXT.NET.Shared.Coin.Public.PublicApi, IPublicApi
    {
        /// <summary>
        ///
        /// </summary>
        public PublicApi()
        {
        }

        /// <summary>
        ///
        /// </summary>
        public override XApiClient publicClient
        {
            get
            {
                if (base.publicClient == null)
                    base.publicClient = new BitstampClient("public");

                return base.publicClient;
            }
        }

        /// <summary>
        /// Fetch symbols, market ids and exchanger's information
        /// </summary>
        /// <param name="args">Add additional attributes for each exchange</param>
        /// <returns></returns>
        public override async ValueTask<Markets> FetchMarketsAsync(Dictionary<string, object> args = null)
        {
            var _result = new Markets();

            publicClient.ExchangeInfo.ApiCallWait(TradeType.Public);
            {
                var _params = publicClient.MergeParamsAndArgs(args);

                var _json_value = await publicClient.CallApiGet1Async("/v2/trading-pairs-info/", _params);
#if DEBUG
                _result.rawJson = _json_value.Content;
#endif
                var _json_result = publicClient.GetResponseMessage(_json_value.Response);
                if (_json_result.success)
                {
                    var _pairs_info = publicClient.DeserializeObject<List<JObject>>(_json_value.Content);
                    foreach (var _market in _pairs_info)
                    {
                        var _symbol = _market["url_symbol"].ToString();

                        var _market_name = _market["name"].ToString().Split('/');
                        var _base_id = _market_name[0].ToLower();
                        var _quote_id = _market_name[1].ToLower();

                        var _base_name = publicClient.ExchangeInfo.GetCommonCurrencyName(_base_id);
                        var _quote_name = publicClient.ExchangeInfo.GetCommonCurrencyName(_quote_id);

                        var _market_id = _base_name + "/" + _quote_name;
                        if (_result.result.ContainsKey(_market_id))
                            continue;

                        var _precision = new MarketPrecision
                        {
                            quantity = _market["base_decimals"].Value<int>(),
                            price = _market["counter_decimals"].Value<int>(),
                            amount = _market["counter_decimals"].Value<int>()
                        };

                        var _lot = (decimal)Math.Pow(10.0, -(double)_precision.quantity);
                        var _active = _market["trading"].ToString().ToLower() == "enabled";

                        var _limits = new MarketLimits
                        {
                            quantity = new MarketMinMax
                            {
                                min = _lot,
                                max = decimal.MaxValue
                            },
                            price = new MarketMinMax
                            {
                                min = (decimal)Math.Pow(10.0, -(double)_precision.price),
                                max = decimal.MaxValue
                            },
                            amount = new MarketMinMax
                            {
                                min = Convert.ToDecimal(_market["minimum_order"].Value<string>().Split(' ')[0]),
                                max = decimal.MaxValue
                            }
                        };

                        var _entry = new MarketItem
                        {
                            marketId = _market_id,

                            symbol = _symbol,

                            baseId = _base_id,
                            quoteId = _quote_id,

                            baseName = _base_name,
                            quoteName = _quote_name,

                            lot = _lot,
                            active = _active,

                            precision = _precision,
                            limits = _limits
                        };

                        _result.result.Add(_entry.marketId, _entry);
                    }
                }

                _result.SetResult(_json_result);
            }

            return _result;
        }

        /// <summary>
        /// Fetch current best bid and ask, as well as the last trade price.
        /// </summary>
        /// <param name="base_name">The type of trading base-currency of which information you want to query for.</param>
        /// <param name="quote_name">The type of trading quote-currency of which information you want to query for.</param>
        /// <param name="args">Add additional attributes for each exchange</param>
        /// <returns></returns>
        public override async ValueTask<Ticker> FetchTickerAsync(string base_name, string quote_name, Dictionary<string, object> args = null)
        {
            var _result = new Ticker(base_name, quote_name);

            var _market = await this.LoadMarketAsync(_result.marketId);
            if (_market.success)
            {
                publicClient.ExchangeInfo.ApiCallWait(TradeType.Public);

                var _params = publicClient.MergeParamsAndArgs(args);

                var _json_value = await publicClient.CallApiGet1Async($"/v2/ticker/{_market.result.symbol}", _params);
#if DEBUG
                _result.rawJson = _json_value.Content;
#endif
                var _json_result = publicClient.GetResponseMessage(_json_value.Response);
                if (_json_result.success)
                {
                    var _ticker = publicClient.DeserializeObject<BTickerItem>(_json_value.Content);
                    {
                        _ticker.symbol = _market.result.symbol;
                        _result.result = _ticker;
                    }
                }

                _result.SetResult(_json_result);
            }
            else
            {
                _result.SetResult(_market);
            }

            return _result;
        }

        /// <summary>
        /// Fetch pending or registered order details
        /// </summary>
        /// <param name="base_name">The type of trading base-currency of which information you want to query for.</param>
        /// <param name="quote_name">The type of trading quote-currency of which information you want to query for.</param>
        /// <param name="limits">maximum number of items (optional): default 20</param>
        /// <param name="args">Add additional attributes for each exchange</param>
        /// <returns></returns>
        public override async ValueTask<OrderBooks> FetchOrderBooksAsync(string base_name, string quote_name, int limits = 20, Dictionary<string, object> args = null)
        {
            var _result = new OrderBooks(base_name, quote_name);

            var _market = await this.LoadMarketAsync(_result.marketId);
            if (_market.success)
            {
                publicClient.ExchangeInfo.ApiCallWait(TradeType.Public);

                var _params = publicClient.MergeParamsAndArgs(args);

                var _json_value = await publicClient.CallApiGet1Async($"/v2/order_book/{_market.result.symbol}", _params);
#if DEBUG
                _result.rawJson = _json_value.Content;
#endif
                var _json_result = publicClient.GetResponseMessage(_json_value.Response);
                if (_json_result.success)
                {
                    var _orderbook = publicClient.DeserializeObject<BOrderBook>(_json_value.Content);
                    {
                        _result.result.asks = _orderbook.asks.OrderBy(o => o.price).Take(limits).ToList();
                        _result.result.bids = _orderbook.bids.OrderByDescending(o => o.price).Take(limits).ToList();

                        _result.result.symbol = _market.result.symbol;
                        _result.result.timestamp = _orderbook.timestamp * 1000;
                        _result.result.nonce = _orderbook.timestamp;
                    }
                }

                _result.SetResult(_json_result);
            }
            else
            {
                _result.SetResult(_market);
            }

            return _result;
        }

        /// <summary>
        /// Fetch array of recent trades data
        /// </summary>
        /// <param name="base_name">The type of trading base-currency of which information you want to query for.</param>
        /// <param name="quote_name">The type of trading quote-currency of which information you want to query for.</param>
        /// <param name="timeframe">time frame interval (optional): default "1d"</param>
        /// <param name="since">return committed data since given time (milli-seconds) (optional): default 0</param>
        /// <param name="limits">maximum number of items (optional): default 20</param>
        /// <param name="args">Add additional attributes for each exchange</param>
        /// <returns></returns>
        public override async ValueTask<CompleteOrders> FetchCompleteOrdersAsync(string base_name, string quote_name, string timeframe = "1d", long since = 0, int limits = 20, Dictionary<string, object> args = null)
        {
            var _result = new CompleteOrders(base_name, quote_name);

            var _market = await this.LoadMarketAsync(_result.marketId);
            if (_market.success)
            {
                publicClient.ExchangeInfo.ApiCallWait(TradeType.Public);

                var _timeframe = publicClient.ExchangeInfo.GetTimeframe(timeframe);
                var _timestamp = publicClient.ExchangeInfo.GetTimestamp(timeframe);

                var _params = publicClient.MergeParamsAndArgs(args);

                var _json_value = await publicClient.CallApiGet1Async($"/v2/transactions/{_market.result.symbol}", _params);
#if DEBUG
                _result.rawJson = _json_value.Content;
#endif
                var _json_result = publicClient.GetResponseMessage(_json_value.Response);
                if (_json_result.success)
                {
                    var _json_data = publicClient.DeserializeObject<List<BCompleteOrderItem>>(_json_value.Content);
                    {
                        var _orders = _json_data
                                                .Where(t => t.timestamp >= since)
                                                .OrderByDescending(t => t.timestamp)
                                                .Take(limits);

                        foreach (var _o in _orders)
                        {
                            _o.orderType = OrderType.Limit;
                            _o.fillType = FillType.Fill;

                            _o.amount = _o.quantity * _o.price;
                            _result.result.Add(_o);
                        }
                    }
                }

                _result.SetResult(_json_result);
            }
            else
            {
                _result.SetResult(_market);
            }

            return _result;
        }
    }
}