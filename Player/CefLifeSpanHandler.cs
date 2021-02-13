using CefSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Player
{
    public class CefLifeSpanHandler : ILifeSpanHandler
    {
        public static CefLifeSpanHandler Instance = new CefLifeSpanHandler();

        public bool DoClose(IWebBrowser chromiumWebBrowser, IBrowser browser)
        {
            if (browser.IsDisposed || browser.IsPopup)
                return false;
            return true;
        }

        public void OnAfterCreated(IWebBrowser chromiumWebBrowser, IBrowser browser)
        {
        }

        public void OnBeforeClose(IWebBrowser chromiumWebBrowser, IBrowser browser)
        {
        }

        public bool OnBeforePopup(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, string targetUrl, string targetFrameName, WindowOpenDisposition targetDisposition, bool userGesture, IPopupFeatures popupFeatures, IWindowInfo windowInfo, IBrowserSettings browserSettings, ref bool noJavascriptAccess, out IWebBrowser newBrowser)
        {
            newBrowser = null;
            switch (targetDisposition)
            {
                case WindowOpenDisposition.NewForegroundTab:
                case WindowOpenDisposition.NewBackgroundTab:
                case WindowOpenDisposition.NewPopup:
                case WindowOpenDisposition.NewWindow:
                    browser.MainFrame.LoadUrl(targetUrl);
                    return true;
            }
            return false;
        }
    }
}
