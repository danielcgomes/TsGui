﻿//    Copyright (C) 2016 Mike Pohatu

//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation; version 2 of the License.

//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.

//    You should have received a copy of the GNU General Public License along
//    with this program; if not, write to the Free Software Foundation, Inc.,
//    51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.

// TsBaseOption.cs - base class for the rest of the gui options to inherit

using System.Xml.Linq;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using System.Windows.Data;
using System.Collections.Generic;

namespace TsGui
{
    public abstract class TsBaseOption: INotifyPropertyChanged, IGroupChild
    {

        protected List<Group> _groups = new List<Group>();
        protected int _hiddenParents = 0;
        protected int _disabledParents = 0;
        protected bool _isenabled = true;
        protected bool _ishidden = false;
        protected MainController _controller;
        protected string _value;
        protected string _help;
        protected int _height;
        protected int _visibleHeight;
        protected ToolTip _tooltip;
        protected Label _labelcontrol;
        protected Control _control;
        protected string _labeltext;
        protected Thickness _visiblemargin;
        protected Thickness _visiblelabelmargin;
        protected Thickness _visiblepadding;
        protected Thickness _visiblelabelpadding;
        protected Thickness _margin;
        protected Thickness _labelmargin;
        protected Thickness _padding;
        protected Thickness _labelpadding;

        //properties
        #region
        public List<Group> Groups { get { return this._groups; } }
        public int GroupCount { get { return this._groups.Count; } }
        public int DisabledParentCount { get; set; }
        public int HiddenParentCount { get; set; }
        public bool PurgeInactive { get; set; }
        public string VariableName { get; set; }
        public Label Label { get { return this._labelcontrol; } }
        public Control Control { get { return this._control; } }
        public string Value
        {
            get { return this._value; }
            set
            {
                this._value = value;
                this.OnPropertyChanged(this, "Value");
            }
        }
        public string InactiveValue { get; set; }
        public string LabelText
        {
            get { return this._labeltext; }
            set
            {
                this._labeltext = value;
                this.OnPropertyChanged(this, "LabelText");
            }
        }
        public Thickness Margin
        {
            get { return this._margin; }
            set
            {
                this._margin = value;
                this.OnPropertyChanged(this, "Margin");
            }
        }
        public Thickness LabelMargin
        {
            get { return this._labelmargin; }
            set
            {
                this._labelmargin = value;
                this.OnPropertyChanged(this, "LabelMargin");
            }
        }
        public Thickness Padding
        {
            get { return this._padding; }
            set
            {
                this._padding = value;
                this.OnPropertyChanged(this, "Padding");
            }
        }
        public Thickness LabelPadding
        {
            get { return this._labelpadding; }
            set
            {
                this._labelpadding = value;
                this.OnPropertyChanged(this, "LabelPadding");
            }
        }
        public int Height
        {
            get { return this._height; }
            set
            {
                this._height = value;
                this.OnPropertyChanged(this, "Height");
            }
        }
        public string HelpText
        {
            get { return this._help; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    this._help = value;
                    this._labelcontrol.ToolTip = this._tooltip;
                    this._control.ToolTip = this._tooltip;
                    OnPropertyChanged(this, "HelpText");
                }
            }
        }
        public ToolTip ToolTip
        {
            get { return this._tooltip; }
            set
            {
                this._tooltip = value;
                OnPropertyChanged(this, "ToolTip");
            }
        }
        public bool IsEnabled
        {
            get { return this._isenabled; }
            set
            {
                this._isenabled = value;
                OnPropertyChanged(this, "IsEnabled");
            }
        }
        public bool IsHidden
        {
            get { return this._ishidden; }
            set
            {
                this.HideUnhide(value);
                OnPropertyChanged(this, "IsHidden");
            }
        }
        public bool IsActive
        {
            get
            {
                if ((this.IsEnabled == true) && (this.IsHidden == false))
                { return true; }
                else { return false; }
            }
        }
        #endregion

        //Event handling
        #region
        //Setup the INotifyPropertyChanged interface 
        public event PropertyChangedEventHandler PropertyChanged;

        // OnPropertyChanged method to raise the event
        protected void OnPropertyChanged(object sender, string name)
        {
            PropertyChanged?.Invoke(sender, new PropertyChangedEventArgs(name));
        }

        public void OnGroupStateChange()
        {
            GroupingLogic.EvaluateGroups(this);
        }

        public void OnParentHide(bool Hide)
        {
            GroupingLogic.OnParentHide(this, Hide);
        }

        public void OnParentEnable(bool Enable)
        {
            GroupingLogic.OnParentEnable(this, Enable);
        }
        #endregion

        //constructor
        protected TsBaseOption()
        {
            //this._parentColumn = ParentColumn;
            this._labelcontrol = new Label();
            this._labelcontrol.DataContext = this;
            this._labelcontrol.SetBinding(Label.IsEnabledProperty, new Binding("IsEnabled"));
            this._labelcontrol.SetBinding(Label.ContentProperty, new Binding("LabelText"));
            this._labelcontrol.SetBinding(Label.HeightProperty, new Binding("Height"));
            this._labelcontrol.SetBinding(Label.PaddingProperty, new Binding("LabelPadding"));
            this._labelcontrol.SetBinding(Label.MarginProperty, new Binding("LabelMargin"));

            this._labelcontrol.VerticalAlignment = VerticalAlignment.Bottom;
            this._visiblelabelpadding = new Thickness(3, 0, 0, 0);
            this.LabelPadding = this._visiblelabelpadding;

            this._visiblelabelmargin = new Thickness(2,2,2,2);
            this.LabelMargin = this._visiblelabelmargin;

            //Setup the tooltips
            this._tooltip = new ToolTip();

            //setup the binding
            TextBlock tb = new TextBlock();
            this._tooltip.Content = tb;
            tb.SetBinding(TextBlock.TextProperty, new Binding("HelpText"));

            //Set defaults
            this.PurgeInactive = false;
            this.DisabledParentCount = 0;
            this.HiddenParentCount = 0;

            this.InactiveValue = "TSGUI_INACTIVE";
            this._visibleHeight = 20;
            this.Height = _visibleHeight;
            this._labelcontrol.HorizontalAlignment = HorizontalAlignment.Left;
        }

        protected void LoadBaseXml(XElement InputXml)
        {
            //Load the XML
            #region
            this.PurgeInactive = XmlHandler.GetBoolFromXAttribute(InputXml, "PurgeInactive", this.PurgeInactive);

            this.VariableName = XmlHandler.GetStringFromXElement(InputXml, "Variable", this.VariableName);
            this.InactiveValue = XmlHandler.GetStringFromXElement(InputXml, "InactiveValue", this.InactiveValue);
            this.HelpText = XmlHandler.GetStringFromXElement(InputXml, "HelpText", this.HelpText);
            this.LabelText = XmlHandler.GetStringFromXElement(InputXml, "Label", this.LabelText);

            this._visibleHeight = XmlHandler.GetIntFromXElement(InputXml, "Height", this.Height);
            this.Height = this._visibleHeight;

            int padInt = XmlHandler.GetIntFromXElement(InputXml, "LabelPadding", 0);
            this._visiblelabelpadding = new System.Windows.Thickness(padInt, padInt, padInt, padInt);
            this.LabelPadding = this._visiblelabelpadding;

            this.IsEnabled = XmlHandler.GetBoolFromXElement(InputXml, "Enabled", this.IsEnabled);
            this.IsHidden = XmlHandler.GetBoolFromXElement(InputXml, "Hidden", this.IsHidden);

            IEnumerable<XElement> xGroups = InputXml.Elements("Group");
            if (xGroups != null)
            {
                foreach (XElement xGroup in xGroups)
                { this._groups.Add(this._controller.AddToGroup(xGroup.Value, this)); }
            }
            #endregion
        }

        protected void HideUnhide(bool Hidden)
        {
            this._ishidden = Hidden;
            if (Hidden == true)
            {
                Thickness zerothick = new Thickness(0);
                this._control.Visibility = Visibility.Collapsed;
                this._labelcontrol.Visibility = Visibility.Collapsed;
                //this.Height = 0;
                this.Margin = zerothick;
                this.LabelMargin = zerothick;
                this.Padding = zerothick;
                this.LabelPadding = zerothick;
            }
            else
            {
                this._control.Visibility = Visibility.Visible;
                this._labelcontrol.Visibility = Visibility.Visible;
                //this.Height = this._visibleHeight;
                this.Padding = this._visiblepadding;
                this.LabelPadding = this._visiblelabelpadding;
                this.Margin = this._visiblemargin;
                this.LabelMargin = this._visiblelabelmargin;
            }
        }
    }
}
