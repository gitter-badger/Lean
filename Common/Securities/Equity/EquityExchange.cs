/*
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

namespace QuantConnect.Securities.Equity
{
    /// <summary>
    /// Equity exchange information 
    /// </summary>
    /// <seealso cref="SecurityExchange"/>
    public class EquityExchange : SecurityExchange
    {
        private TimeSpan _marketOpen = new TimeSpan(9, 30, 0);
        private TimeSpan _marketClose = new TimeSpan(12 + 4, 0, 0);
        private TimeSpan _extendedMarketOpen = new TimeSpan(4, 0, 0);
        private TimeSpan _extendedMarketClose = new TimeSpan(12 + 8, 0, 0);

        /// <summary>
        /// Boolean flag indicating the equities exchange markets are open
        /// </summary>
        public override bool ExchangeOpen
        {
            get { return DateTimeIsOpen(Time); }
        }

        /// <summary>
        /// Number of trading days in an equity calendar year - 252
        /// </summary>
        public override int TradingDaysPerYear
        {
            get { return 252; }
        }

        /// <summary>
        /// Equity markets open time/hour of day.
        /// </summary>
        public override TimeSpan MarketOpen
        {
            get { return _marketOpen; }
            set { _marketOpen = value; }
        }

        /// <summary>
        /// Equity markets closing time/hour of day.
        /// </summary>
        public override TimeSpan MarketClose
        {
            get { return _marketClose; }
            set { _marketClose = value; }
        }

        /// <summary>
        /// Equity market extended opens at 4:00am
        /// </summary>
        public override TimeSpan ExtendedMarketOpen
        {
            get { return _extendedMarketOpen; }
            set { _extendedMarketOpen = value; }
        }

        /// <summary>
        /// Equity market extended closes at 8:00pm
        /// </summary>
        public override TimeSpan ExtendedMarketClose
        {
            get { return _extendedMarketClose; }
            set { _extendedMarketClose = value; }
        }

        /// <summary>
        /// Check if the datetime specified is open.
        /// </summary>
        /// <param name="dateToCheck">Time to check</param>
        /// <remarks>Ignores early market closing times</remarks>
        /// <returns>True if open</returns>
        public override bool DateTimeIsOpen(DateTime dateToCheck)
        {
            //Market not open yet:
            if (dateToCheck.TimeOfDay < MarketOpen ||
                dateToCheck.TimeOfDay >= MarketClose ||
                dateToCheck.DayOfWeek == DayOfWeek.Saturday ||
                dateToCheck.DayOfWeek == DayOfWeek.Sunday)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Set the incoming datetime object date to the market open time.
        /// </summary>
        /// <param name="time">Date we want to set</param>
        /// <returns>DateTime adjusted to market open</returns>
        public override DateTime TimeOfDayOpen(DateTime time)
        {
            //Set open time to 9:30am for US equities.
            return time.Date.Add(MarketOpen);
        }

        /// <summary>
        /// Set the datetime object to the time of day closed.
        /// </summary>
        /// <param name="time"></param>
        /// <returns>Return datetime date to market close</returns>
        public override DateTime TimeOfDayClosed(DateTime time)
        {
            //Set close time to 4pm for US equities.
            return time.Date.Add(MarketClose);
        }

        /// <summary>
        /// Check if the US Equity markets are open on today's *date*. Check the calendar holidays as well.
        /// </summary>
        /// <param name="dateToCheck">Datetime to check</param>
        /// <returns>True if open</returns>
        public override bool DateIsOpen(DateTime dateToCheck)
        {
            if (dateToCheck.DayOfWeek == DayOfWeek.Saturday || dateToCheck.DayOfWeek == DayOfWeek.Sunday)
            {
                return false;
            }

            //Check the date first.
            if (USHoliday.Dates.Contains(dateToCheck.Date))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Check if this datetime is open, including extended market hours:
        /// </summary>
        /// <param name="time">Time to check</param>
        /// <returns>Bool true if in normal+extended market hours.</returns>
        public override bool DateTimeIsExtendedOpen(DateTime time)
        {
            if (time.DayOfWeek == DayOfWeek.Saturday || time.DayOfWeek == DayOfWeek.Sunday)
            {
                return false;
            }

            if (time.TimeOfDay < ExtendedMarketOpen || time.TimeOfDay >= ExtendedMarketClose)
            {
                return false;
            }

            return true;
        }
    }
}