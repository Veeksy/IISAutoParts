﻿#pragma checksum "..\..\..\pages\providersPage.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "E221E57679C2A9676C4010892421D678226FC10F82A4D8D47CC0FA6BA5F5A94A"
//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.42000
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

using IISAutoParts.pages;
using MaterialDesignThemes.MahApps;
using MaterialDesignThemes.Wpf;
using MaterialDesignThemes.Wpf.Converters;
using MaterialDesignThemes.Wpf.Transitions;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace IISAutoParts.pages {
    
    
    /// <summary>
    /// providersPage
    /// </summary>
    public partial class providersPage : System.Windows.Controls.Page, System.Windows.Markup.IComponentConnector, System.Windows.Markup.IStyleConnector {
        
        
        #line 13 "..\..\..\pages\providersPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid carPage;
        
        #line default
        #line hidden
        
        
        #line 22 "..\..\..\pages\providersPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid providersDGV;
        
        #line default
        #line hidden
        
        
        #line 64 "..\..\..\pages\providersPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox customerNameTb;
        
        #line default
        #line hidden
        
        
        #line 66 "..\..\..\pages\providersPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox addressTb;
        
        #line default
        #line hidden
        
        
        #line 72 "..\..\..\pages\providersPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button searchBtn;
        
        #line default
        #line hidden
        
        
        #line 76 "..\..\..\pages\providersPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button AddnewOrder;
        
        #line default
        #line hidden
        
        
        #line 80 "..\..\..\pages\providersPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button deleteBtn;
        
        #line default
        #line hidden
        
        
        #line 93 "..\..\..\pages\providersPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button previousBtn;
        
        #line default
        #line hidden
        
        
        #line 97 "..\..\..\pages\providersPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button nextBtn;
        
        #line default
        #line hidden
        
        
        #line 103 "..\..\..\pages\providersPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox pageNumber;
        
        #line default
        #line hidden
        
        
        #line 106 "..\..\..\pages\providersPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label countPage;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/IISAutoParts;component/pages/providerspage.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\pages\providersPage.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.carPage = ((System.Windows.Controls.Grid)(target));
            return;
            case 2:
            this.providersDGV = ((System.Windows.Controls.DataGrid)(target));
            
            #line 23 "..\..\..\pages\providersPage.xaml"
            this.providersDGV.MouseDoubleClick += new System.Windows.Input.MouseButtonEventHandler(this.providersDGV_MouseDoubleClick);
            
            #line default
            #line hidden
            return;
            case 4:
            this.customerNameTb = ((System.Windows.Controls.TextBox)(target));
            return;
            case 5:
            this.addressTb = ((System.Windows.Controls.TextBox)(target));
            return;
            case 6:
            this.searchBtn = ((System.Windows.Controls.Button)(target));
            
            #line 72 "..\..\..\pages\providersPage.xaml"
            this.searchBtn.Click += new System.Windows.RoutedEventHandler(this.searchBtn_Click);
            
            #line default
            #line hidden
            return;
            case 7:
            this.AddnewOrder = ((System.Windows.Controls.Button)(target));
            
            #line 77 "..\..\..\pages\providersPage.xaml"
            this.AddnewOrder.Click += new System.Windows.RoutedEventHandler(this.AddnewOrder_Click);
            
            #line default
            #line hidden
            return;
            case 8:
            this.deleteBtn = ((System.Windows.Controls.Button)(target));
            
            #line 81 "..\..\..\pages\providersPage.xaml"
            this.deleteBtn.Click += new System.Windows.RoutedEventHandler(this.deleteBtn_Click);
            
            #line default
            #line hidden
            return;
            case 9:
            this.previousBtn = ((System.Windows.Controls.Button)(target));
            
            #line 93 "..\..\..\pages\providersPage.xaml"
            this.previousBtn.Click += new System.Windows.RoutedEventHandler(this.previousBtn_Click);
            
            #line default
            #line hidden
            return;
            case 10:
            this.nextBtn = ((System.Windows.Controls.Button)(target));
            
            #line 97 "..\..\..\pages\providersPage.xaml"
            this.nextBtn.Click += new System.Windows.RoutedEventHandler(this.nextBtn_Click);
            
            #line default
            #line hidden
            return;
            case 11:
            this.pageNumber = ((System.Windows.Controls.TextBox)(target));
            
            #line 103 "..\..\..\pages\providersPage.xaml"
            this.pageNumber.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.pageNumber_TextChanged);
            
            #line default
            #line hidden
            
            #line 104 "..\..\..\pages\providersPage.xaml"
            this.pageNumber.PreviewTextInput += new System.Windows.Input.TextCompositionEventHandler(this.pageNumber_PreviewTextInput);
            
            #line default
            #line hidden
            return;
            case 12:
            this.countPage = ((System.Windows.Controls.Label)(target));
            return;
            }
            this._contentLoaded = true;
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        void System.Windows.Markup.IStyleConnector.Connect(int connectionId, object target) {
            System.Windows.EventSetter eventSetter;
            switch (connectionId)
            {
            case 3:
            eventSetter = new System.Windows.EventSetter();
            eventSetter.Event = System.Windows.Controls.Primitives.ToggleButton.CheckedEvent;
            
            #line 38 "..\..\..\pages\providersPage.xaml"
            eventSetter.Handler = new System.Windows.RoutedEventHandler(this.CheckBox_Checked);
            
            #line default
            #line hidden
            ((System.Windows.Style)(target)).Setters.Add(eventSetter);
            eventSetter = new System.Windows.EventSetter();
            eventSetter.Event = System.Windows.Controls.Primitives.ToggleButton.UncheckedEvent;
            
            #line 39 "..\..\..\pages\providersPage.xaml"
            eventSetter.Handler = new System.Windows.RoutedEventHandler(this.CheckBox_Unchecked);
            
            #line default
            #line hidden
            ((System.Windows.Style)(target)).Setters.Add(eventSetter);
            break;
            }
        }
    }
}

