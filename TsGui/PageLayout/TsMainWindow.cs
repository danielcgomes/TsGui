﻿using System;
using System.Xml.Linq;
using System.Windows;
using System.Windows.Media;
using System.Globalization;
using System.ComponentModel;

namespace TsGui
{
    class TsMainWindow : INotifyPropertyChanged
    {
        private double _height;        //default page height for the window
        private double _width;         //default page width for the window
        private string _headingTitle;
        private string _headingText;
        private int _headingHeight;
        private Thickness _pageMargin = new Thickness(0, 0, 0, 0);
        private string _footerText;
        private double _footerHeight;
        private HorizontalAlignment _footerHAlignment;
        private bool _gridlines = false;

        

        //Properties
        public string HeadingText { get { return this._headingText; } }
        public int HeadingHeight { get { return this._headingHeight; } }
        public string HeadingTitle
        {
            get { return this._headingTitle; }
            set
            {
                this._headingTitle = value;
                this.OnPropertyChanged(this, "HeadingTitle");
            }
        }
        public double Height
        {
            get { return this._height; }
            set
            {
                this._height = value;
                this.OnPropertyChanged(this, "Height");
            }
        }
        public double Width
        {
            get { return this._width; }
            set
            {
                this._width = value;
                this.OnPropertyChanged(this, "Width");
            }
        }
        public Thickness PageMargin
        {
            get { return this._pageMargin; }
            set
            {
                this._pageMargin = value;
                this.OnPropertyChanged(this, "PageMargin");
            }
        }
        public SolidColorBrush HeadingBgColor { get; set; }
        public SolidColorBrush HeadingTextColor { get; set; }

        public double FooterHeight
        {
            get { return this._footerHeight; }
            set
            {
                this._footerHeight = value;
                this.OnPropertyChanged(this, "FooterHeight");
            }
        }
        public string FooterText
        {
            get { return this._footerText; }
            set
            {
                this._footerText = value;
                this.OnPropertyChanged(this, "FooterText");
            }
        }
        public HorizontalAlignment FooterHAlignment
        {
            get { return this._footerHAlignment; }
            set
            {
                this._footerHAlignment = value;
                this.OnPropertyChanged(this, "FooterHAlignment");
            }
        }
        public bool ShowGridLines
        {
            get { return this._gridlines; }
            set
            {
                this._gridlines = value;
                this.OnPropertyChanged(this, "ShowGridLines");
            }
        }

        //Constructors
        public TsMainWindow()
        {
            //set default values
            this._width = Double.NaN;
            this._height = Double.NaN;
            this._headingHeight = 50;
            this.HeadingTextColor = new SolidColorBrush(Colors.White);
            this.HeadingBgColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF006AD4"));
            this.FooterText = "Powered by TsGui - www.20road.com";
            this.FooterHeight = 15;
            this.FooterHAlignment = HorizontalAlignment.Right;
        }

        //Setup the INotifyPropertyChanged interface 
        public event PropertyChangedEventHandler PropertyChanged;

        // OnPropertyChanged method to raise the event
        protected void OnPropertyChanged(object sender, string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(sender, new PropertyChangedEventArgs(name));
            }
        }

        public void LoadXml(XElement SourceXml)
        {

            XElement x;

            if (SourceXml != null)
            {

                XElement headingX = SourceXml.Element("Heading");
                if (headingX != null)
                {
                    x = headingX.Element("Title");
                    if (x != null) { this._headingTitle = x.Value; }

                    x = headingX.Element("Text");
                    if (x != null) { this._headingText = x.Value; }

                    x = headingX.Element("Height");
                    if (x != null) { this._headingHeight = Convert.ToInt32(x.Value); }

                    x = headingX.Element("BgColor");
                    if (x != null)
                    {
                        this.HeadingBgColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(x.Value));
                    }

                    x = headingX.Element("TextColor");
                    if (x != null)
                    {
                        this.HeadingTextColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(x.Value));
                    }

                }

                XElement footerX = SourceXml.Element("Footer");
                if (footerX != null)
                {
                    x = footerX.Element("Text");
                    if (x != null) { this.FooterText = x.Value; }

                    x = footerX.Element("Height");
                    if (x != null) { this.FooterHeight = Convert.ToInt32(x.Value); }

                    GuiFactory.LoadHAlignment(footerX, ref this._footerHAlignment);
                }

                x = SourceXml.Element("Width");
                if (x != null)
                { this.Width = Convert.ToInt32(x.Value); }

                x = SourceXml.Element("Height");
                if (x != null)
                { this.Height = Convert.ToInt32(x.Value); }

                GuiFactory.LoadMargins(SourceXml, this._pageMargin);

                //Set show grid lines after pages and columns have been created.
                x = SourceXml.Element("ShowGridLines");
                if (x != null)
                { this.ShowGridLines = true; }
            }
        }
    }
}
