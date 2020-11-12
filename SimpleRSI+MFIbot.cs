using System;
using System.Linq;
using cAlgo.API;
using cAlgo.API.Indicators;
using cAlgo.API.Internals;
using cAlgo.Indicators;

namespace cAlgo.Robots
{
    [Robot(TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class SimpleRSIcBot : Robot
    {
        [Parameter("source")]
        public DataSeries source { get; set; }

        [Parameter("period", DefaultValue = 14)]
        public int period { get; set; }

        [Parameter("lots", DefaultValue = 1)]
        public int lots { get; set; }

        [Parameter("takeProfit", DefaultValue = 150)]
        public int takeProfit { get; set; }

        [Parameter("stopLoss", DefaultValue = 0)]
        public int stopLoss { get; set; }

        private RelativeStrengthIndex rsi1;
        private MoneyFlowIndex mfi1;

        protected override void OnStart()
        {
            // Put your initialization logic here
            rsi1 = Indicators.RelativeStrengthIndex(source, period);
            mfi1 = Indicators.MoneyFlowIndex(period);
        }

        protected override void OnTick()
        {
            // Put your core logic here
            if (rsi1.Result.LastValue > 80 && mfi1.Result.LastValue > 80)
            {
                //open sell position & close buy positions
                openPosition(TradeType.Sell);
                //closePosition(TradeType.Buy);
            }
            else if (rsi1.Result.LastValue < 20 && mfi1.Result.LastValue < 20)
            {
                // open buy position & close sell positions
                openPosition(TradeType.Buy);
                //closePosition(TradeType.Sell);
            }
        }

        protected override void OnStop()
        {
            // Put your deinitialization logic here
        }

        //create the open position function
        private void openPosition(TradeType tradeType)
        {
            var positionInfo = Positions.Find("SimpleRSI-MFIcBot", Symbol, tradeType);
            //label of my bot is that name : SimpleRSIcBot
            if (positionInfo == null)
            {
                ExecuteMarketOrder(tradeType, Symbol, lots, "SimpleRSI-MFIcBot", stopLoss, takeProfit);
            }
        }

        //create the close position function
        private void closePosition(TradeType tradeType)
        {
            foreach (var positionInfo in Positions.FindAll("SimpleRSI-MFIcBot", Symbol, tradeType))
            {
                ClosePosition(positionInfo);
            }
        }
    }
}
