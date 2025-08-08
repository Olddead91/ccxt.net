﻿using Newtonsoft.Json.Linq;
using CCXT.NET.Shared.Coin;
using CCXT.NET.Shared.Coin.Private;
using CCXT.NET.Shared.Coin.Types;
using CCXT.NET.Shared.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CCXT.NET.OKEx.Private
{
    /// <summary>
    ///
    /// </summary>
    public class PrivateApi : CCXT.NET.Shared.Coin.Private.PrivateApi, IPrivateApi
    {
        private readonly string __connect_key;
        private readonly string __secret_key;
        private readonly string __user_name;
        private readonly string __user_password;

        /// <summary>
        ///
        /// </summary>
        public PrivateApi(string connect_key, string secret_key, string user_name, string user_password)
        {
            __connect_key = connect_key;
            __secret_key = secret_key;
            __user_name = user_name;
            __user_password = user_password;
        }

        /// <summary>
        ///
        /// </summary>
        public override XApiClient privateClient
        {
            get
            {
                if (base.privateClient == null)
                    base.privateClient = new OKExClient("private", __connect_key, __secret_key, __user_name, __user_password);

                return base.privateClient;
            }
        }

        /// <summary>
        ///
        /// </summary>
        public override CCXT.NET.Shared.Coin.Public.PublicApi publicApi
        {
            get
            {
                if (base.publicApi == null)
                    base.publicApi = new CCXT.NET.OKEx.Public.PublicApi();

                return base.publicApi;
            }
        }

        /// <summary>
        /// Withdraw
        /// </summary>
        /// <param name="currency_name">base coin or quote coin name</param>
        /// <param name="address">coin address for send</param>
        /// <param name="tag">Secondary address identifier for coins like XRP,XMR etc.</param>
        /// <param name="quantity">amount of coin</param>
        /// <param name="args">Add additional attributes for each exchange: [chargefee]</param>
        /// <returns></returns>
        public override async ValueTask<Transfer> CoinWithdrawAsync(string currency_name, string address, string tag, decimal quantity, Dictionary<string, object> args = null)
        {
            var _result = new Transfer();

            var _market = await publicApi.LoadMarketAsync(_result.MakeMarketId(currency_name, "USDT"));
            if (_market.success)
            {
                privateClient.ExchangeInfo.ApiCallWait(TradeType.Private);

                var _params = new Dictionary<string, object>();
                {
                    _params.Add("symbol", _market.result.symbol);
                    _params.Add("withdraw_amount", quantity);
                    _params.Add("withdraw_address", address);
                    _params.Add("target", "address");           // or 'okcn', 'okcom', 'okex'
                    _params.Add("trade_pwd", privateClient.UserPassword);

                    privateClient.MergeParamsAndArgs(_params, args);
                }

                var _json_value = await privateClient.CallApiPost1Async("/withdraw.do", _params);
#if DEBUG
                _result.rawJson = _json_value.Content;
#endif
                var _json_result = privateClient.GetResponseMessage(_json_value.Response);
                if (_json_result.success)
                {
                    var _json_data = privateClient.DeserializeObject<OTransfer>(_json_value.Content);
                    if (_json_data.success)
                    {
                        var _withdraw = new OTransferItem
                        {
                            transferId = _json_data.transferId,
                            transactionId = privateClient.GenerateNonceString(16),
                            timestamp = CUnixTime.NowMilli,

                            transactionType = TransactionType.Withdraw,

                            currency = currency_name,
                            toAddress = address,
                            toTag = tag,

                            amount = quantity,
                            fee = 0,

                            confirmations = 0,
                            isCompleted = _json_data.success
                        };

                        _result.result = _withdraw;
                    }
                    else
                    {
                        _json_result.SetFailure();
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
        /// Withdrawal Cancellation Request
        /// </summary>
        /// <param name="currency_name">base coin or quote coin name</param>
        /// <param name="transferId">The unique id of the withdrawal request specified</param>
        /// <param name="args">Add additional attributes for each exchange</param>
        /// <returns></returns>
        public override async ValueTask<Transfer> CancelCoinWithdrawAsync(string currency_name, string transferId, Dictionary<string, object> args = null)
        {
            var _result = new Transfer();

            var _market = await publicApi.LoadMarketAsync(_result.MakeMarketId(currency_name, "USDT"));
            if (_market.success)
            {
                privateClient.ExchangeInfo.ApiCallWait(TradeType.Private);

                var _params = new Dictionary<string, object>();
                {
                    _params.Add("symbol", _market.result.symbol); // symbol	Pairs like : ltc_btc etc_btc
                    _params.Add("withdraw_id", transferId);

                    privateClient.MergeParamsAndArgs(_params, args);
                }

                var _json_value = await privateClient.CallApiPost1Async("/cancel_withdraw.do", _params);
#if DEBUG
                _result.rawJson = _json_value.Content;
#endif
                var _json_result = privateClient.GetResponseMessage(_json_value.Response);
                if (_json_result.success)
                {
                    var _json_data = privateClient.DeserializeObject<OTransfer>(_json_value.Content);
                    if (_json_data.success)
                    {
                        var _transfer = new OTransferItem
                        {
                            transferId = _json_data.transferId,
                            timestamp = CUnixTime.NowMilli,

                            transactionType = TransactionType.Withdraw,

                            currency = currency_name,
                            toAddress = "",
                            toTag = "",

                            amount = 0.0m,
                            isCompleted = _json_data.success
                        };

                        _result.result = _transfer;
                    }
                    else
                    {
                        _json_result.SetFailure("Unknown reference id");
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
        /// Get User Deposits or Withdraw Record
        /// </summary>
        /// <param name="currency_name">base coin or quote coin name</param>
        /// <param name="timeframe">time frame interval (optional): default "1d"</param>
        /// <param name="since">return committed data since given time (milli-seconds) (optional): default 0</param>
        /// <param name="limits">You can set the maximum number of transactions you want to get with this parameter</param>
        /// <param name="args">Add additional attributes for each exchange</param>
        /// <returns></returns>
        public override async ValueTask<Transfers> FetchTransfersAsync(string currency_name, string timeframe = "1d", long since = 0, int limits = 20, Dictionary<string, object> args = null)
        {
            var _result = new Transfers();

            var _market = await publicApi.LoadMarketAsync(_result.MakeMarketId(currency_name, "USD"));
            if (_market.success)
            {
                var _timestamp = privateClient.ExchangeInfo.GetTimestamp(timeframe);
                var _timeframe = privateClient.ExchangeInfo.GetTimeframe(timeframe);

                var _params = new Dictionary<string, object>();
                {
                    _params.Add("symbol", _market.result.symbol);
                    _params.Add("type", 0); // 0：deposits 1 ：withdraw
                    _params.Add("current_page", 1); // current page number
                    _params.Add("page_length", 50); // data entries number per page, maximum 50

                    privateClient.MergeParamsAndArgs(_params, args);
                }

                // TransactionType.Deposit
                {
                    privateClient.ExchangeInfo.ApiCallWait(TradeType.Private);

                    var _json_value = await privateClient.CallApiPost1Async("/account_records.do", _params);
#if DEBUG
                    _result.rawJson += _json_value.Content;
#endif
                    var _json_result = privateClient.GetResponseMessage(_json_value.Response);
                    if (_json_result.success)
                    {
                        var _json_deposits = privateClient.DeserializeObject<ODeposits>(_json_value.Content);
                        {
                            var _deposits = _json_deposits.result
                                                      .Where(t => t.timestamp >= since)
                                                      .OrderByDescending(t => t.timestamp)
                                                      .Take(limits);

                            foreach (var _d in _deposits)
                            {
                                _d.currency = _json_deposits.symbol;
                                _d.transferId = _d.timestamp.ToString();                  // transferId 없음
                                _d.transactionId = (_d.timestamp * 1000).ToString();      // transactionId 없음
                                _result.result.Add(_d);
                            }
                        }
                    }

                    _result.SetResult(_json_result);
                }

                _params.Remove("type");
                _params.Add("type", 1); // 0：deposits 1 ：withdraw

                // TransactionType.Withdrawal
                if (_result.success)
                {
                    privateClient.ExchangeInfo.ApiCallWait(TradeType.Private);

                    var _json_value = await privateClient.CallApiPost1Async("/account_records.do", _params);
#if DEBUG
                    _result.rawJson += _json_value.Content;
#endif
                    var _json_result = privateClient.GetResponseMessage(_json_value.Response);
                    if (_json_result.success)
                    {
                        var _json_withdraws = privateClient.DeserializeObject<OWithdraws>(_json_value.Content);
                        {
                            var _withdraws = _json_withdraws.result
                                                        .Where(t => t.timestamp >= since)
                                                        .OrderByDescending(t => t.timestamp)
                                                        .Take(limits);

                            foreach (var _w in _withdraws)
                            {
                                _w.currency = _json_withdraws.symbol;
                                _w.transferId = _w.timestamp.ToString();                  // transferId 없음
                                _w.transactionId = (_w.timestamp * 1000).ToString();      // transactionId 없음
                                _result.result.Add(_w);
                            }
                        }
                    }

                    _result.SetResult(_json_result);
                }
            }
            else
            {
                _result.SetResult(_market);
            }

            return _result;
        }

        /// <summary>
        /// Get User Account Info
        /// </summary>
        /// <param name="base_name">The type of trading base-currency of which information you want to query for.</param>
        /// <param name="quote_name">The type of trading quote-currency of which information you want to query for.</param>
        /// <param name="args">Add additional attributes for each exchange</param>
        /// <returns></returns>
        public override async ValueTask<Balance> FetchBalanceAsync(string base_name, string quote_name, Dictionary<string, object> args = null)
        {
            var _result = new Balance();

            var _currency_id = await publicApi.LoadCurrencyIdAsync(base_name);
            if (_currency_id.success)
            {
                privateClient.ExchangeInfo.ApiCallWait(TradeType.Private);

                var _params = privateClient.MergeParamsAndArgs(args);

                var _json_value = await privateClient.CallApiPost1Async("/userinfo.do", _params);
#if DEBUG
                _result.rawJson = _json_value.Content;
#endif
                var _json_result = privateClient.GetResponseMessage(_json_value.Response);
                if (_json_result.success)
                {
                    var _json_data = privateClient.DeserializeObject<JObject>(_json_value.Content);
                    {
                        var _balances = privateClient.DeserializeObject<Dictionary<string, JObject>>(_json_data["info"]["funds"].ToString());
                        if (_balances["free"].ContainsKey(_currency_id.result))
                        {
                            var _balance = new OBalanceItem()
                            {
                                free = _balances["free"][_currency_id.result].Value<decimal>(), // available fund
                                used = _balances["freezed"][_currency_id.result].Value<decimal>() // frozen fund
                            };

                            _balance.currency = base_name;
                            _balance.total = _balance.free + _balance.used;

                            _result.result = _balance;
                        }
                        else
                        {
                            _json_result.SetFailure();
                        }
                    }
                }

                _result.SetResult(_json_result);
            }
            else
            {
                _result.SetResult(_currency_id);
            }

            return _result;
        }

        /// <summary>
        /// Get User Account Info
        /// </summary>
        /// <param name="args">Add additional attributes for each exchange</param>
        /// <returns></returns>
        public override async ValueTask<Balances> FetchBalancesAsync(Dictionary<string, object> args = null)
        {
            var _result = new Balances();

            var _markets = await publicApi.LoadMarketsAsync();
            if (_markets.success)
            {
                privateClient.ExchangeInfo.ApiCallWait(TradeType.Private);

                var _params = privateClient.MergeParamsAndArgs(args);

                var _json_value = await privateClient.CallApiPost1Async("/userinfo.do", _params);
#if DEBUG
                _result.rawJson = _json_value.Content;
#endif
                var _json_result = privateClient.GetResponseMessage(_json_value.Response);
                if (_json_result.success)
                {
                    var _json_data = privateClient.DeserializeObject<JObject>(_json_value.Content);
                    {
                        var _balances = _json_data["info"]["funds"];

                        foreach (var _currency_id in _markets.CurrencyNames)
                        {
                            if (_balances["free"].SelectToken(_currency_id.Key) == null)
                                continue;

                            var _balance = new OBalanceItem
                            {
                                free = _balances["free"][_currency_id.Key].Value<decimal>(),
                                used = _balances["freezed"][_currency_id.Key].Value<decimal>()
                            };

                            _balance.currency = _currency_id.Value;
                            _balance.total = _balance.free + _balance.used;

                            _result.result.Add(_balance);
                        }
                    }
                }

                _result.SetResult(_json_result);
            }
            else
            {
                _result.SetResult(_markets);
            }

            return _result;
        }
    }
}