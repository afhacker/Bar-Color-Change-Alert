using cAlgo.API;
using cAlgo.API.Alert;
using cAlgo.API.Alert.Enums;
using cAlgo.API.Alert.Models;

namespace cAlgo
{
    [Indicator(IsOverlay = true, TimeZone = TimeZones.UTC, AccessRights = AccessRights.FullAccess)]
    public class BarColorChangeAlert : Indicator
    {
        #region Fields

        private int _lastAlertBarIndex;

        #endregion Fields

        #region Parameters

        [Parameter("Type", DefaultValue = AlertType.Popup, Group = "Alert")]
        public AlertType AlertType { get; set; }

        [Parameter("Show Window", DefaultValue = false, Group = "Alert")]
        public bool ShowWindow { get; set; }

        #endregion Parameters

        #region Overridden methods

        protected override void Initialize()
        {
            _lastAlertBarIndex = Bars.ClosePrices.Count - 1;

            if (ShowWindow)
            {
                Notifications.ShowPopup();
            }
        }

        public override void Calculate(int index)
        {
            if (Bars.ClosePrices[index] > Bars.OpenPrices[index] && Bars.ClosePrices[index - 1] < Bars.OpenPrices[index - 1])
            {
                TriggerAlert(index, "Bullish");
            }
            else if (Bars.ClosePrices[index] < Bars.OpenPrices[index] && Bars.ClosePrices[index - 1] > Bars.OpenPrices[index - 1])
            {
                TriggerAlert(index, "Bearish");
            }
        }

        #endregion Overridden methods

        #region Other methods

        private void TriggerAlert(int index, string type)
        {
            if (index == _lastAlertBarIndex || !IsLastBar || AlertType == AlertType.None)
            {
                return;
            }

            _lastAlertBarIndex = index;

            var alertModel = new AlertModel
            {
                TimeFrame = TimeFrame.ToString(),
                Price = Bars.ClosePrices[index],
                Symbol = Symbol.Name,
                TriggeredBy = "Bar Color Change",
                Time = Server.Time,
                Type = type
            };

            if (AlertType == AlertType.Popup)
            {
                Notifications.ShowPopup(alertModel);
            }
            else if (AlertType == AlertType.Triggers)
            {
                Notifications.TriggerAlert(alertModel);
            }
        }

        #endregion Other methods
    }
}