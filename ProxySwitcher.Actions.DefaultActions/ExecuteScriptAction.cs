using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProxySwitcher.Common;
using System.Windows.Controls;
using System.Diagnostics;
using System.IO;

namespace ProxySwitcher.Actions.DefaultActions
{
    [SwitcherActionAddIn]
    public class ExecuteScriptAction : SwitcherActionBase
    {
        public override string Name
        {
            get { return DefaultResources.ExecuteScript_Name; }
        }

        public override string Group
        {
            get { return "Script"; }
        }

        public override string Description
        {
            get { return string.Empty; }
        }

        public override Stream IconResourceStream
        {
            get { return null; }
        }

        public override Guid Id
        {
            get { return new Guid("38F54865-A9E8-4787-87BF-DCB60A26F863"); }
        }

        public IList<KeyValuePair<int, string>> GetWindowStyleItems()
        {
            return new List<KeyValuePair<int, string>> {
                new KeyValuePair<int, string>((int)ProcessWindowStyle.Normal, DefaultResources.ExecuteScript_Window_Normal),
                new KeyValuePair<int, string>((int)ProcessWindowStyle.Minimized, DefaultResources.ExecuteScript_Window_Minimized),
                new KeyValuePair<int, string>((int)ProcessWindowStyle.Maximized, DefaultResources.ExecuteScript_Window_Maximized),
                new KeyValuePair<int, string>((int)ProcessWindowStyle.Hidden, DefaultResources.ExecuteScript_Window_Hidden),
            };
        }

        public KeyValuePair<int, string> GetWindowStyleItem(int windowStyle)
        {
            foreach (KeyValuePair<int, string> item in this.GetWindowStyleItems())
            {
                if (item.Key == windowStyle)
                    return item;
            }

            return this.GetDefaultWindowStyleItem();
        }

        public KeyValuePair<int, string> GetActiveWindowStyleItem(Guid networkId)
        {
            int windowStyle = (int)ProcessWindowStyle.Normal;
            int.TryParse(this.Settings[networkId.ToString() + "_ScriptWindowStyle"], out windowStyle);

            return this.GetWindowStyleItem(windowStyle);
        }

        public KeyValuePair<int, string> GetDefaultWindowStyleItem()
        {
            return this.GetWindowStyleItems().First();
        }


        public override UserControl GetWindowControl(Guid networkId, string networkName)
        {
            string script = this.Settings[networkId.ToString() + "_ScriptPath"];
            if (String.IsNullOrEmpty(script))
            {
                return new ExecuteScript.ExecuteScriptSetup(this, networkId, networkName);
            }
            else
            {
                bool withParameter = false;
                bool.TryParse(this.Settings[networkId.ToString() + "_ScriptWithParameter"], out withParameter);

                bool withParameterNameInsteadOfId = false;
                bool.TryParse(this.Settings[networkId.ToString() + "_ScriptWithParameterName"], out withParameterNameInsteadOfId);

                bool asAdmin = false;
                bool.TryParse(this.Settings[networkId.ToString() + "_ScriptAsAdmin"], out asAdmin);

                int windowStyle = (int)ProcessWindowStyle.Normal;
                int.TryParse(this.Settings[networkId.ToString() + "_ScriptWindowStyle"], out windowStyle);

                return new ExecuteScript.ExecuteScriptSetup(this, networkId, networkName, script, withParameter, withParameterNameInsteadOfId, asAdmin, windowStyle);
            }
        }

        internal void Save(Guid networkId, string script, bool withParameter, bool withParameterNameInsteadOfId, bool runAsAdmin, int windowStyle)
        {
            this.Settings[networkId.ToString() + "_ScriptPath"] = script;
            this.Settings[networkId.ToString() + "_ScriptWithParameter"] = withParameter.ToString();
            this.Settings[networkId.ToString() + "_ScriptWithParameterName"] = withParameterNameInsteadOfId.ToString();
            this.Settings[networkId.ToString() + "_ScriptAsAdmin"] = runAsAdmin.ToString();
            this.Settings[networkId.ToString() + "_ScriptWindowStyle"] = windowStyle.ToString();

            OnSettingsChanged();

            if (HostApplication != null)
                HostApplication.SetStatusText(this, DefaultResources.Saved_Status);
        }

        public override void Activate(Guid networkId, string networkName)
        {
            string script = this.Settings[networkId.ToString() + "_ScriptPath"];
            if (String.IsNullOrEmpty(script))
                return;

            bool withParameter = false;
            bool.TryParse(this.Settings[networkId.ToString() + "_ScriptWithParameter"], out withParameter);

            bool asAdmin = false;
            bool.TryParse(this.Settings[networkId.ToString() + "_ScriptAsAdmin"], out asAdmin);

            int windowStyle = (int)ProcessWindowStyle.Normal;
            int.TryParse(this.Settings[networkId.ToString() + "_ScriptWindowStyle"], out windowStyle);

            ProcessStartInfo startInfo = new ProcessStartInfo(Environment.ExpandEnvironmentVariables(script));

            if (withParameter)
            {
                bool withNetworkNameInsteadofId = false;
                bool.TryParse(this.Settings[networkId.ToString() + "_ScriptWithParameterName"], out withNetworkNameInsteadofId);

                if (withNetworkNameInsteadofId)
                    startInfo.Arguments = networkName;
                else
                    startInfo.Arguments = networkId.ToString();
            }

            if (asAdmin)
                startInfo.Verb = "runas";

            startInfo.WindowStyle = (ProcessWindowStyle)windowStyle;

            Process.Start(startInfo);
        }
    }
}
