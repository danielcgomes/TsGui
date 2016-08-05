﻿using System.Xml.Linq;
using System.Windows.Controls;
using System.Windows;
using System.Diagnostics;
using System.Windows.Data;

namespace TsGui
{
    public class TsCheckBox: TsBaseOption, IGuiOption
    {
        new private CheckBox _control;
        private HorizontalAlignment _hAlignment;
        private string _valTrue;
        private string _valFalse;

        public TsCheckBox(XElement SourceXml) : base()
        {
            this._control = new CheckBox();
            base._control = this._control;

            //setup the bindings
            this._control.DataContext = this;

            this._control.SetBinding(Label.PaddingProperty, new Binding("Padding"));
            this._control.SetBinding(Label.MarginProperty, new Binding("Margin"));

            this._visiblepadding = new Thickness(0,0,0,0);
            this.Padding = this._visiblepadding;

            this._visiblemargin = new Thickness(2, 1, 2, 1);
            this.Margin = this._visiblemargin;

            this._control.VerticalContentAlignment = VerticalAlignment.Center;
            this._hAlignment = HorizontalAlignment.Left;
          
            this._valTrue = "TRUE";
            this._valFalse = "FALSE";
            this.Height = 17;

            this.LoadXml(SourceXml);
            this.Build();
        }

        public TsVariable Variable
        {
            get
            {
                //this.value = this.control.Text;
                if (this._control.IsChecked == true) { return new TsVariable(this.VariableName, this._valTrue); }
                else { return new TsVariable(this.VariableName, this._valFalse); }
            }
        }

        public void LoadXml(XElement InputXml)
        {
            #region
            XElement x;

            //load the xml for the base class stuff
            this.LoadBaseXml(InputXml);

            x = InputXml.Element("Checked");
            if (x != null)
            { this._control.IsChecked = true; }

            x = InputXml.Element("TrueValue");
            if (x != null)
            { this._valTrue = x.Value; }

            x = InputXml.Element("FalseValue");
            if (x != null)
            { this._valFalse = x.Value; }

            GuiFactory.LoadHAlignment(InputXml, ref this._hAlignment);
            GuiFactory.LoadMargins(InputXml, this._margin);

            #endregion
        }

        private void Build()
        {
            Debug.WriteLine("CheckBox HAlignment: " + this._hAlignment.ToString());
            this._control.VerticalAlignment = VerticalAlignment.Center;
            this._control.HorizontalAlignment = this._hAlignment;
        }
    }
}
