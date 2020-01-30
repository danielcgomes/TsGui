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

// TsPage.cs - view model class for a page in TsGui

using System.Collections.Generic;
using System.Xml.Linq;
using System.Windows;
using System.Windows.Threading;

using TsGui.Events;
using TsGui.Grouping;
using TsGui.View.GuiOptions;
using TsGui.Validation;
using TsGui.Diagnostics.Logging;
using System;

namespace TsGui.View.Layout
{
    public class TsPage: BaseLayoutElement, IRootLayoutElement
    {
        public event ComplianceRetryEventHandler ComplianceRetry;

        private TsPage _previouspage;
        private TsPage _nextpage;
        private TsTable _table;

        //Properties
        #region
        public TsPane LeftPane { get; set; }
        public TsPane RightPane { get; set; }
        public TsPageHeader PageHeader { get; set; }
        public string PageId { get; set; } = string.Empty;
        public TsPage NextActivePage
        {
            get
            {
                if ((this.NextPage == null) || (this.NextPage.IsHidden == false)) { return this.NextPage; }
                else { return this.NextPage.NextActivePage; }
            }
        }
        public TsPage PreviousActivePage
        {
            get
            {
                if ((this.PreviousPage == null) || (this.PreviousPage.IsHidden == false)) { return this.PreviousPage; }
                else { return this.PreviousPage.PreviousActivePage; }
            }
        }

        public TsPage PreviousPage
        {
            get { return this._previouspage; }
            set { this.ConnectPrevPage(value); }
        }
        public TsPage NextPage
        {
            get { return this._nextpage; }
            set { this.ConnectNextPage(value); }
        }        
        public List<IGuiOption> Options { get { return this._table.Options; } }
        public TsPageUI Page { get; private set; }
        public bool IsFirst { get; set; } = false;
        #endregion

        //Events
        #region
        public event TsGuiWindowEventHandler PageWindowLoaded;

        /// <summary>
        /// Method to handle when content has finished rendering on the window
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        public void OnPageLoaded(object o, RoutedEventArgs e)
        {
            this.PageWindowLoaded?.Invoke(o,e);
            foreach (IGuiOption opt in this._table.Options)
            {
                if (opt.IsEnabled && opt.InteractiveControl?.Focusable == true)
                {
                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() => opt.InteractiveControl.Focus()));
                    break;
                }
            }
        }
        #endregion

        //Constructors
        public TsPage(XElement SourceXml, PageDefaults Defaults, IDirector MainController):base (MainController)
        {
            //this._controller = Defaults.RootController;
            LoggerFacade.Info("New page created");
            this.ShowGridLines = MainController.ShowGridLines;
            this.Page = new TsPageUI(this);
            this.PageHeader = Defaults.PageHeader;
            this.LeftPane = Defaults.LeftPane;
            this.RightPane = Defaults.RightPane;

            this.Page.Loaded += this.OnPageLoaded;

            this.GroupingStateChange += this.OnPageHide;
            this.Page.DataContext = this;
            this.Page.ButtonGrid.DataContext = Defaults.Buttons;

            this.LoadXml(SourceXml);
            this.Update();
        }

        //Methods
        public new void LoadXml(XElement InputXml)
        {
            base.LoadXml(InputXml);
            XElement x;

            this.PageId = XmlHandler.GetStringFromXAttribute(InputXml, "PageId", this.PageId);
            this.IsEnabled = XmlHandler.GetBoolFromXElement(InputXml, "Enabled", this.IsEnabled);
            this.IsHidden = XmlHandler.GetBoolFromXElement(InputXml, "Hidden", this.IsHidden);

            x = InputXml.Element("Heading");
            if (x != null) { this.PageHeader = new TsPageHeader(this,this.PageHeader,x,this._director); }

            x = InputXml.Element("LeftPane");
            if (x != null) { this.LeftPane = new TsPane(x, this._director); }

            x = InputXml.Element("RightPane");
            if (x != null) { this.RightPane = new TsPane(x, this._director); }

            //create the table adn bind it to the content
            this._table = new TsTable(InputXml, this, this._director);
            this.Page.MainTablePresenter.Content = this._table.Grid;
            this.Page.LeftPanePresenter.Content = this.LeftPane?.PaneUI;
            this.Page.RightPanePresenter.Content = this.RightPane?.PaneUI;
        }

        public bool OptionsValid()
        {
            if ((ResultValidator.OptionsValid(this._table.ValidationOptions)) && this.PageHeader.OptionsValid()) { return true; }
            else { return false; }
        }

        public void Cancel()
        {
            this._director.Cancel();
        }

        public void MovePrevious()
        {
            foreach (IValidationGuiOption option in this._table.ValidationOptions)
            { option.ClearToolTips(); }

            this.ReleaseThisPage();
            this._director.MovePrevious();
        }

        public void MoveNext()
        {
            if (this.OptionsValid() == true)
            {
                this.ReleaseThisPage();
                this._director.MoveNext();
            }
        }

        public void Finish()
        {
            if (this.OptionsValid() == true)
            {
                this._director.Finish();
            }
        }

        private void UpdatePrevious()
        {
            this.PreviousActivePage?.Update();
        }

        private void ReleaseThisPage()
        {
            this.Page.HeaderPresenter.Content = null;
            this.Page.LeftPanePresenter.Content = null;
            this.Page.RightPanePresenter.Content = null;
        }

        //Update the prev, next, finish buttons according to the current pages 
        //place in the world
        public void Update()
        {
            this.Page.HeaderPresenter.Content = this.PageHeader.UI;
            this.Page.LeftPanePresenter.Content = this.LeftPane?.PaneUI;
            this.Page.RightPanePresenter.Content = this.RightPane?.PaneUI;
            TsButtons.Update(this, this.Page);            
        }

        public void OnPageHide(object o, GroupingEventArgs e)
        {
            if (e.GroupStateChanged == GroupStateChanged.IsHidden) { this._director.CurrentPage.Update(); }
        }

        public void RaiseComplianceRetryEvent()
        {
            this.ComplianceRetry?.Invoke(this, new RoutedEventArgs());
        }

        private void ConnectNextPage(TsPage NewNextPage)
        {
            this._nextpage = NewNextPage;
        }

        private void ConnectPrevPage(TsPage NewPrevPage)
        {
            this._previouspage = NewPrevPage;
        }
    }
}
