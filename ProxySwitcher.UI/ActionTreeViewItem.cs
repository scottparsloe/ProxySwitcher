using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using ProxySwitcher.Common;
using System.Windows.Media.Imaging;
using System.IO;
using System.Windows.Documents;

namespace ProxySwitcher.UI
{
    public class ActionTreeViewItem : TreeViewItem
    {
        public ActionTreeViewItem()
            : base()
        {
        }

        public ActionTreeViewItem(SwitcherActionBase action, NetworkTreeViewItem parentNetworkItem)
            : base()
        {
            if (action == null)
                throw new ArgumentNullException("action", "Action cannot be null");

            this.Action = action;
            this.ActionId = action.Id;
            this.Header = CreateHeader();
            this.ParentNetworkItem = parentNetworkItem;
        }


        public SwitcherActionBase Action { get; set; }

        public Guid ActionId { get; set; }

        public NetworkTreeViewItem ParentNetworkItem { get; set; }

        public Guid NetworkId
        {
            get { return ParentNetworkItem.NetworkConfiguration.Id; }
        }

        public string NetworkName
        {
            get { return ParentNetworkItem.NetworkConfiguration.Name; }
        }

        private object CreateHeader()
        {
            StackPanel pan = new StackPanel();

            string blank = string.Empty;
            pan.Orientation = Orientation.Horizontal;
            Image image = new Image();
            image.Height = 16;
            if (this.Action.IconResourceStream != null)
            {
                image.Source = GetBitmapImageFromStream(this.Action.IconResourceStream);
            }
            else
            {
                image.Source = new BitmapImage(new Uri(@"pack://application:,,,/Images/action_nologo.png", UriKind.RelativeOrAbsolute));
            }
            pan.Children.Add(image);
            blank = "  ";

            pan.Children.Add(new TextBlock(new Run(blank + this.Action.Name)));
            return pan;
        }

        private BitmapImage GetBitmapImageFromStream(Stream stream)
        {
            var img = new BitmapImage();
            img.BeginInit();
            img.StreamSource = stream;
            img.CacheOption = BitmapCacheOption.OnLoad;
            img.EndInit();
            return img;
        }

    }
}
