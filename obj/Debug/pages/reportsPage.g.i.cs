﻿#pragma checksum "..\..\..\pages\reportsPage.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "CF1DC76EE35DA22FBB534A05FDAC47A593D0411FE357C8B743790E5A150E66FD"
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
    /// reportsPage
    /// </summary>
    public partial class reportsPage : System.Windows.Controls.Page, System.Windows.Markup.IComponentConnector, System.Windows.Markup.IStyleConnector {
        
        
        #line 22 "..\..\..\pages\reportsPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ProgressBar loadingDoc;
        
        #line default
        #line hidden
        
        
        #line 28 "..\..\..\pages\reportsPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid reportDGV;
        
        #line default
        #line hidden
        
        
        #line 69 "..\..\..\pages\reportsPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox reportsTypeCb;
        
        #line default
        #line hidden
        
        
        #line 77 "..\..\..\pages\reportsPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DatePicker startDateDt;
        
        #line default
        #line hidden
        
        
        #line 82 "..\..\..\pages\reportsPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DatePicker endDateDt;
        
        #line default
        #line hidden
        
        
        #line 86 "..\..\..\pages\reportsPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox clientCb;
        
        #line default
        #line hidden
        
        
        #line 93 "..\..\..\pages\reportsPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button createReportBtn;
        
        #line default
        #line hidden
        
        
        #line 96 "..\..\..\pages\reportsPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button deleteBtn;
        
        #line default
        #line hidden
        
        
        #line 109 "..\..\..\pages\reportsPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button previousBtn;
        
        #line default
        #line hidden
        
        
        #line 113 "..\..\..\pages\reportsPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button nextBtn;
        
        #line default
        #line hidden
        
        
        #line 119 "..\..\..\pages\reportsPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox pageNumber;
        
        #line default
        #line hidden
        
        
        #line 121 "..\..\..\pages\reportsPage.xaml"
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
            System.Uri resourceLocater = new System.Uri("/IISAutoParts;component/pages/reportspage.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\pages\reportsPage.xaml"
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
            this.loadingDoc = ((System.Windows.Controls.ProgressBar)(target));
            return;
            case 2:
            this.reportDGV = ((System.Windows.Controls.DataGrid)(target));
            
            #line 28 "..\..\..\pages\reportsPage.xaml"
            this.reportDGV.MouseDoubleClick += new System.Windows.Input.MouseButtonEventHandler(this.reportDGV_MouseDoubleClick);
            
            #line default
            #line hidden
            return;
            case 4:
            this.reportsTypeCb = ((System.Windows.Controls.ComboBox)(target));
            
            #line 71 "..\..\..\pages\reportsPage.xaml"
            this.reportsTypeCb.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.reportsTypeCb_SelectionChanged);
            
            #line default
            #line hidden
            return;
            case 5:
            this.startDateDt = ((System.Windows.Controls.DatePicker)(target));
            return;
            case 6:
            this.endDateDt = ((System.Windows.Controls.DatePicker)(target));
            return;
            case 7:
            this.clientCb = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 8:
            this.createReportBtn = ((System.Windows.Controls.Button)(target));
            
            #line 93 "..\..\..\pages\reportsPage.xaml"
            this.createReportBtn.Click += new System.Windows.RoutedEventHandler(this.createReportBtn_Click);
            
            #line default
            #line hidden
            return;
            case 9:
            this.deleteBtn = ((System.Windows.Controls.Button)(target));
            
            #line 96 "..\..\..\pages\reportsPage.xaml"
            this.deleteBtn.Click += new System.Windows.RoutedEventHandler(this.deleteBtn_Click);
            
            #line default
            #line hidden
            return;
            case 10:
            this.previousBtn = ((System.Windows.Controls.Button)(target));
            
            #line 109 "..\..\..\pages\reportsPage.xaml"
            this.previousBtn.Click += new System.Windows.RoutedEventHandler(this.previousBtn_Click);
            
            #line default
            #line hidden
            return;
            case 11:
            this.nextBtn = ((System.Windows.Controls.Button)(target));
            
            #line 113 "..\..\..\pages\reportsPage.xaml"
            this.nextBtn.Click += new System.Windows.RoutedEventHandler(this.nextBtn_Click);
            
            #line default
            #line hidden
            return;
            case 12:
            this.pageNumber = ((System.Windows.Controls.TextBox)(target));
            
            #line 119 "..\..\..\pages\reportsPage.xaml"
            this.pageNumber.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.pageNumber_TextChanged);
            
            #line default
            #line hidden
            
            #line 119 "..\..\..\pages\reportsPage.xaml"
            this.pageNumber.PreviewTextInput += new System.Windows.Input.TextCompositionEventHandler(this.pageNumber_PreviewTextInput);
            
            #line default
            #line hidden
            return;
            case 13:
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
            
            #line 43 "..\..\..\pages\reportsPage.xaml"
            eventSetter.Handler = new System.Windows.RoutedEventHandler(this.CheckBox_Checked);
            
            #line default
            #line hidden
            ((System.Windows.Style)(target)).Setters.Add(eventSetter);
            eventSetter = new System.Windows.EventSetter();
            eventSetter.Event = System.Windows.Controls.Primitives.ToggleButton.UncheckedEvent;
            
            #line 44 "..\..\..\pages\reportsPage.xaml"
            eventSetter.Handler = new System.Windows.RoutedEventHandler(this.CheckBox_Unchecked);
            
            #line default
            #line hidden
            ((System.Windows.Style)(target)).Setters.Add(eventSetter);
            break;
            }
        }
    }
}

