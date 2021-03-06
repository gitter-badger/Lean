﻿/*
 * QUANTCONNECT.COM - Democratizing Finance, Empowering Individuals.
 * Lean Algorithmic Trading Engine v2.0. Copyright 2014 QuantConnect Corporation.
 * 
 * Licensed under the Apache License, Version 2.0 (the "License"); 
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
*/

using System;
using System.Collections.Generic;
using System.Globalization;

namespace QuantConnect.Data.Custom
{
    /******************************************************** 
    * CLASS DEFINITIONS
    *********************************************************/
    /// <summary>
    /// Quandl Data Type - Import generic data from quandl, without needing to define Reader methods. 
    /// This reads the headers of the data imported, and dynamically creates properties for the imported data.
    /// </summary>
    public class Quandl : DynamicData
    {
        /******************************************************** 
        * CLASS PRIVATE VARIABLES
        *********************************************************/
        private bool _isInitialized = false;
        private readonly List<string> _propertyNames = new List<string>();
        private readonly string _valueColumn;
        private static string _authCode = "";

        /// <summary>
        /// Is the auth
        /// </summary>
        public static bool IsAuthCodeSet
        {
            get;
            private set;
        }

        /******************************************************** 
        * CLASS CONSTRUCTOR
        *********************************************************/
        /// <summary>
        /// Default quandl constructor uses Close as its value column
        /// </summary>
        public Quandl()
        {
            base.EndTime = Time + TimeSpan.FromDays(1);
            _valueColumn = "Close";
        }
        
        /// <summary>
        /// Constructor for creating customized quandl instance which doesn't use "Close" as its value item.
        /// </summary>
        /// <param name="valueColumnName"></param>
        protected Quandl(string valueColumnName)
        {
            _valueColumn = valueColumnName;
        }


        /******************************************************** 
        * CLASS METHODS
        *********************************************************/
        /// <summary>
        /// Generic Reader Implementation for Quandl Data.
        /// </summary>
        /// <param name="config">Subscription configuration</param>
        /// <param name="line">CSV line of data from the souce</param>
        /// <param name="date">Date of the requested line</param>
        /// <param name="datafeed">Datafeed type - live or backtest</param>
        /// <returns></returns>
        public override BaseData Reader(SubscriptionDataConfig config, string line, DateTime date, DataFeedEndpoint datafeed)
        {
            var data = new Quandl();
            data.Symbol = config.Symbol;
            var csv = line.Split(',');

            if (!_isInitialized)
            {
                _isInitialized = true;
                foreach (var propertyName in csv)
                {
                    var property = propertyName.TrimStart().TrimEnd();
                    // should we remove property names like Time?
                    // do we need to alias the Time??
                    data.SetProperty(property, 0m);
                    _propertyNames.Add(property);
                }
                return data;
            }

            data.Time = DateTime.ParseExact(csv[0], "yyyy-MM-dd", CultureInfo.InvariantCulture);

            for (var i = 1; i < csv.Length; i++)
            {
                var value = csv[i].ToDecimal();
                data.SetProperty(_propertyNames[i], value);
            }

            // we know that there is a close property, we want to set that to 'Value'
            data.Value = (decimal)data.GetProperty(_valueColumn);

            return data;
        }


        /// <summary>
        /// Quandl Source Locator: Using the Quandl V1 API automatically set the URL for the dataset.
        /// </summary>
        /// <param name="config">Subscription configuration object</param>
        /// <param name="date">Date of the data file we're looking for</param>
        /// <param name="datafeed"></param>
        /// <returns>STRING API Url for Quandl.</returns>
        public override string GetSource(SubscriptionDataConfig config, DateTime date, DataFeedEndpoint datafeed)
        {
            return @"https://www.quandl.com/api/v1/datasets/" + config.Symbol + ".csv?sort_order=asc&exclude_headers=false&auth_token=" + _authCode;
        }

        
        /// <summary>
        /// Set the auth code for the quandl set to the QuantConnect auth code.
        /// </summary>
        /// <param name="authCode"></param>
        public static void SetAuthCode(string authCode)
        {
            _authCode = authCode;
            IsAuthCodeSet = true;
        }
    }

} // End QC Namespace
