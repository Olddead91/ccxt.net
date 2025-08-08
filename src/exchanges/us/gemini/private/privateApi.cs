﻿using CCXT.NET.Shared.Coin;
using CCXT.NET.Shared.Coin.Private;
using CCXT.NET.Shared.Coin.Types;
using CCXT.NET.Shared.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CCXT.NET.Gemini.Private
{
    /// <summary>
    ///
    /// </summary>
    public class PrivateApi : CCXT.NET.Shared.Coin.Private.PrivateApi, IPrivateApi
    {
        private readonly string __connect_key;
        private readonly string __secret_key;

        /// <summary>
        ///
        /// </summary>
        public PrivateApi(string connect_key, string secret_key)
        {
            __connect_key = connect_key;
            __secret_key = secret_key;
        }

        /// <summary>
        ///
        /// </summary>
        public override XApiClient privateClient
        {
            get
            {
                if (base.privateClient == null)
                    base.privateClient = new GeminiClient("private", __connect_key, __secret_key);

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
                    base.publicApi = new CCXT.NET.Gemini.Public.PublicApi();

                return base.publicApi;
            }
        }

        /// <summary>
        /// New Deposit Address
        /// </summary>
        /// <param name="currency_name">base coin or quote coin name</param>
        /// <param name="args">Add additional attributes for each exchange</param>
        /// <returns></returns>
        public override async ValueTask<Address> CreateAddressAsync(string currency_name, Dictionary<string, object> args = null)
        {
            var _result = new Address();

            var _currency_id = await publicApi.LoadCurrencyIdAsync(currency_name);
            if (_currency_id.success)
            {
                privateClient.ExchangeInfo.ApiCallWait(TradeType.Private);

                var _params = privateClient.MergeParamsAndArgs(args);

                var _json_value = await privateClient.CallApiPost1Async($"/v1/deposit/{_currency_id.result}/newAddress", _params);
#if DEBUG
                _result.rawJson = _json_value.Content;
#endif
                var _json_result = privateClient.GetResponseMessage(_json_value.Response);
                if (_json_result.success)
                {
                    var _address = privateClient.DeserializeObject<GAddressItem>(_json_value.Content);
                    {
                        _address.currency = currency_name;

                        _result.result = _address;

                        if (_address.success == false)
                            _json_result.SetFailure("could not create address", ErrorCode.TooManyAddress);
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
        /// Withdraw Crypto Funds To Whitelisted Address
        /// </summary>
        /// <param name="currency_name">base coin or quote coin name</param>
        /// <param name="address">coin address for send</param>
        /// <param name="tag">Secondary address identifier for coins like XRP,XMR etc.</param>
        /// <param name="quantity">amount of coin</param>
        /// <param name="args">Add additional attributes for each exchange</param>
        /// <returns></returns>
        public override async ValueTask<Transfer> CoinWithdrawAsync(string currency_name, string address, string tag, decimal quantity, Dictionary<string, object> args = null)
        {
            var _result = new Transfer();

            var _currency_id = await publicApi.LoadCurrencyIdAsync(currency_name);
            if (_currency_id.success)
            {
                privateClient.ExchangeInfo.ApiCallWait(TradeType.Private);

                var _params = new Dictionary<string, object>();
                {
                    _params.Add("amount", quantity);
                    _params.Add("address", address);

                    privateClient.MergeParamsAndArgs(_params, args);
                }

                var _json_value = await privateClient.CallApiPost1Async($"/v1/withdraw/{currency_name}", _params);
#if DEBUG
                _result.rawJson = _json_value.Content;
#endif
                var _json_result = privateClient.GetResponseMessage(_json_value.Response);
                if (_json_result.success)
                {
                    var _json_data = privateClient.DeserializeObject<GWithdrawItem>(_json_value.Content);
                    if (String.IsNullOrEmpty(_json_data.transactionId) == false)
                    {
                        var _withdraw = new GWithdrawItem
                        {
                            transferId = privateClient.GenerateNonceString(13),
                            transactionId = _json_data.transactionId,
                            timestamp = CUnixTime.NowMilli,

                            transactionType = TransactionType.Withdraw,

                            currency = currency_name,
                            toAddress = _json_data.toAddress,
                            toTag = tag,

                            amount = _json_data.amount,
                            fee = 0,

                            confirmations = 0,
                            isCompleted = true
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
                _result.SetResult(_currency_id);
            }

            return _result;
        }

        /// <summary>
        /// This endpoint shows deposits and withdrawals in the supported currencies.
        /// When deposits show as Advanced or Complete they are available for trading.
        /// This endpoint does not currently show cancelled advances, returned outgoing wires or ACH transactions, admin credits and debits, or other exceptional transaction circumstances.
        /// </summary>
        /// <param name="timeframe">time frame interval (optional): default "1d"</param>
        /// <param name="since">return committed data since given time (milli-seconds) (optional): default 0</param>
        /// <param name="limits">You can set the maximum number of transactions you want to get with this parameter</param>
        /// <param name="args">Add additional attributes for each exchange</param>
        /// <returns></returns>
        public override async ValueTask<Transfers> FetchAllTransfersAsync(string timeframe = "1d", long since = 0, int limits = 20, Dictionary<string, object> args = null)
        {
            var _result = new Transfers();

            var _markets = await publicApi.LoadMarketsAsync();
            if (_markets.success)
            {
                privateClient.ExchangeInfo.ApiCallWait(TradeType.Private);

                var _params = privateClient.MergeParamsAndArgs(args);

                // This endpoint shows deposits and withdrawals in the supported currencies.
                // When deposits show as Advanced or Complete they are available for trading.
                // This endpoint does not currently show cancelled advances, returned outgoing wires or ACH transactions,
                // admin credits and debits, or other exceptional transaction circumstances.
                var _json_value = await privateClient.CallApiPost1Async($"/v1/transfers", _params);
#if DEBUG
                _result.rawJson = _json_value.Content;
#endif
                var _json_result = privateClient.GetResponseMessage(_json_value.Response);
                if (_json_result.success)
                {
                    var _json_data = privateClient.DeserializeObject<List<GTransferItem>>(_json_value.Content);
                    {
                        var _transfers = _json_data
                                                .Where(t => t.timestamp >= since)
                                                .OrderByDescending(t => t.timestamp)
                                                .Take(limits);

                        foreach (var _t in _transfers)
                        {
                            if (String.IsNullOrEmpty(_t.transactionId))
                                _t.transactionId = (_t.timestamp * 1000).ToString();

                            if (_t.transactionType == TransactionType.Deposit)
                                _t.toAddress = "undefined";

                            _result.result.Add(_t);
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

        /// <summary>
        /// This will show the available balances in the supported currencies
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

                var _json_value = await privateClient.CallApiPost1Async("/v1/balances", _params);
#if DEBUG
                _result.rawJson = _json_value.Content;
#endif
                var _json_result = privateClient.GetResponseMessage(_json_value.Response);
                if (_json_result.success)
                {
                    var _balances = privateClient.DeserializeObject<List<GBalanceItem>>(_json_value.Content);
                    {
                        foreach (var _balance in _balances)
                        {
                            if (_balance.currency.ToLower() != _currency_id.result.ToLower())
                                continue;

                            _balance.currency = base_name;
                            _balance.used = _balance.total - _balance.free;

                            _result.result = _balance;
                            break;
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
        /// This will show the available balances in the supported currencies
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

                var _json_value = await privateClient.CallApiPost1Async("/v1/balances", _params);
#if DEBUG
                _result.rawJson = _json_value.Content;
#endif
                var _json_result = privateClient.GetResponseMessage(_json_value.Response);
                if (_json_result.success)
                {
                    var _json_data = privateClient.DeserializeObject<List<GBalanceItem>>(_json_value.Content);
                    {
                        foreach (var _currency_id in _markets.CurrencyNames)
                        {
                            var _balances = _json_data.Where(b => b.currency.ToLower() == _currency_id.Key);
                            foreach (var _balance in _balances)
                            {
                                _balance.currency = _currency_id.Value;
                                _balance.used = _balance.total - _balance.free;

                                _result.result.Add(_balance);
                            }
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