﻿// ***********************************************************************
// Assembly         : MVVMSidekick_Wp8
// Author           : waywa
// Created          : 05-17-2014
//
// Last Modified By : waywa
// Last Modified On : 01-04-2015
// ***********************************************************************
// <copyright file="Views.cs" company="">
//     Copyright ©  2012
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.Reflection;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Input;
using MVVMSidekick.ViewModels;
using MVVMSidekick.Commands;
using System.Runtime.CompilerServices;
using MVVMSidekick.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive;
using MVVMSidekick.EventRouting;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Collections;
using System.Windows;



#if NETFX_CORE
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Controls;
using System.Collections.Concurrent;
using Windows.UI.Xaml.Navigation;

using Windows.UI.Xaml.Controls.Primitives;

#elif WPF
using System.Windows.Controls;


using System.Collections.Concurrent;
using System.Windows.Navigation;

using MVVMSidekick.Views;
using System.Windows.Controls.Primitives;
using MVVMSidekick.Utilities;

#elif SILVERLIGHT_5||SILVERLIGHT_4
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Navigation;
using System.Windows.Controls.Primitives;
#elif WINDOWS_PHONE_8||WINDOWS_PHONE_7
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;
using System.Windows.Data;
using System.Windows.Navigation;
using System.Windows.Controls.Primitives;
#endif

#if (! WPF) && (! NETFX_CORE)
/// <summary>
/// The Windows namespace.
/// </summary>
namespace System.Windows
{



}



#endif

/// <summary>
/// The MVVMSidekick namespace.
/// </summary>
namespace MVVMSidekick
{

	/// <summary>
	/// The Views namespace.
	/// </summary>
	namespace Views
	{


		/// <summary>
		/// Class ViewHelper.
		/// </summary>
		public static class ViewHelper
		{
			/// <summary>
			/// The defaul t_ v m_ name
			/// </summary>
			public static readonly string DEFAULT_VM_NAME = "DesignVM";
			/// <summary>
			/// Gets the default designing view model.
			/// </summary>
			/// <param name="view">The view.</param>
			/// <returns>System.Object.</returns>
			public static object GetDefaultDesigningViewModel(this IView view)
			{
				var f = view as FrameworkElement;
				object rval = null;
#if NETFX_CORE
				if (!f.Resources.ContainsKey(DEFAULT_VM_NAME))
#else
				if (!f.Resources.Contains(DEFAULT_VM_NAME))
#endif
				{
					return null;
				}
				else
				{
					rval = f.Resources[DEFAULT_VM_NAME];
				}
				return rval;
			}

			/// <summary>
			/// The view unload call back
			/// </summary>
			internal static RoutedEventHandler ViewUnloadCallBack
				= async (o, e) =>
				{
					IView v = o as IView;
					if (v != null)
					{
						var m = v.ViewModel as IViewModelLifetime;
						if (m != null)
						{
							await m.OnBindedViewUnload(v);
						}
					}
				};
			/// <summary>
			/// The designing view model changed call back
			/// </summary>
			internal static PropertyChangedCallback DesigningViewModelChangedCallBack
				= (o, e) =>
					  {
						  var oiview = o as IView;
						  if (Utilities.Runtime.IsInDesignMode)
						  {
							  oiview.ViewModel = e.NewValue as IViewModel;
						  }
					  };



			/// <summary>
			/// The view model changed callback
			/// </summary>
			internal static PropertyChangedCallback ViewModelChangedCallback
				= (o, e) =>
				{
					dynamic item = o;
					var oiview = o as IView;
					var fele = (oiview.ContentObject as FrameworkElement);
					if (fele == null)
					{
						return;
					}
					if (object.ReferenceEquals(fele.DataContext, e.NewValue))
					{
						return;
					}
					(oiview.ContentObject as FrameworkElement).DataContext = e.NewValue;
					var nv = e.NewValue as IViewModel;
					var ov = e.OldValue as IViewModel;
					if (ov != null)
					{
						ov.OnUnbindedFromView(oiview, nv);
					}
					if (nv != null)
					{
						nv.OnBindedToView(oiview, ov);
					}

				};

			/// <summary>
			/// Gets the content and create if null.
			/// </summary>
			/// <param name="control">The control.</param>
			/// <returns>FrameworkElement.</returns>
			internal static FrameworkElement GetContentAndCreateIfNull(this IView control)
			{
				var c = (control.ContentObject as FrameworkElement);
				if (c == null)
				{
					control.ContentObject = c = new Grid();
				}
				return c;
			}

			/// <summary>
			/// Selfs the close.
			/// </summary>
			/// <param name="view">The view.</param>
			public static void SelfClose(this IView view)
			{

				if (view is UserControl || view is Page)
				{
					var viewElement = view as FrameworkElement;
					var parent = viewElement.Parent;
					if (parent is Panel)
					{
						(parent as Panel).Children.Remove(viewElement);
					}
					else if (parent is Frame)
					{
						var f = (parent as Frame);
						if (f.CanGoBack)
						{
							f.GoBack();
						}
						else
						{
							f.Content = null;
						}
					}
					else if (parent is ContentControl)
					{
						(parent as ContentControl).Content = null;
					}
					else if (parent is Page)
					{
						(parent as Page).Content = null;
					}
					else if (parent is UserControl)
					{
						(parent as UserControl).Content = null;
					}

				}
#if WPF
				else if (view is Window)
				{
					(view as Window).Close();
				}
#endif


			}

		}

#if WPF
		public class MVVMWindow : Window, IView
		{

			public MVVMWindow()
				: this(null)
			{
			}



			public bool IsAutoOwnerSetNeeded
			{
				get { return (bool)GetValue(IsAutoOwnerSetNeededProperty); }
				set { SetValue(IsAutoOwnerSetNeededProperty, value); }
			}

			// Using a DependencyProperty as the backing store for IsAutoOwnerSetNeeded.  This enables animation, styling, binding, etc...
			public static readonly DependencyProperty IsAutoOwnerSetNeededProperty =
				DependencyProperty.Register("IsAutoOwnerSetNeeded", typeof(bool), typeof(MVVMWindow), new PropertyMetadata(true));



			public MVVMWindow(IViewModel viewModel)
			{

				Unloaded += ViewHelper.ViewUnloadCallBack;
				Loaded += async (_1, _2) =>
				{

					//    if (viewModel != null)
					//    {
					//        //   this.Resources[ViewHelper.DEFAULT_VM_NAME] = viewModel;
					//        if (!object.ReferenceEquals(ViewModel, viewModel))
					//        {
					//            ViewModel = viewModel;
					//        }
					//    }
					//    else
					//    {
					//        var solveV = this.GetDefaultViewModel();
					//        if (solveV != null)
					//        {
					//            ViewModel = solveV;
					//        }

					//    }
					//    ////ViewModel = ViewModel ?? new DefaultViewModel();

					await ViewModel.OnBindedViewLoad(this);
				};
			}

			public object ContentObject
			{
				get
				{
					return Content;
				}
				set
				{
					Content = value;
				}
			}




			//public IViewModel DesigningViewModel
			//{
			//	get { return (IViewModel)GetValue(DesigningViewModelProperty); }
			//	set { SetValue(DesigningViewModelProperty, value); }
			//}

			//// Using a DependencyProperty as the backing store for DesigningViewModel.  This enables animation, styling, binding, etc...
			//public static readonly DependencyProperty DesigningViewModelProperty =
			//	DependencyProperty.Register("DesigningViewModel", typeof(IViewModel), typeof(MVVMWindow), new PropertyMetadata(null, ViewHelper.DesigningViewModelChangedCallBack));



			public IViewModel ViewModel
			{
				get
				{
					var rval = GetValue(ViewModelProperty) as IViewModel;
					var c = this.GetContentAndCreateIfNull();
					if (rval == null)
					{

						rval = c.DataContext as IViewModel;
						SetValue(ViewModelProperty, rval);

					}
					else
					{

						if (!Object.ReferenceEquals(c.DataContext, rval))
						{
							c.DataContext = rval;
						}
					}
					return rval;
				}
				set
				{
					SetValue(ViewModelProperty, value);
					var c = this.GetContentAndCreateIfNull();
					if (!Object.ReferenceEquals(c.DataContext, value))
					{
						c.DataContext = value;
					}

				}
			}



			// Using a DependencyProperty as the backing store for ViewModel.  This enables animation, styling, binding, etc...
			public static readonly DependencyProperty ViewModelProperty =
				DependencyProperty.Register("ViewModel", typeof(IViewModel), typeof(MVVMWindow), new PropertyMetadata(null, ViewHelper.ViewModelChangedCallback));


			public ViewType ViewType
			{
				get { return ViewType.Window; }
			}




		}




#endif

#if WINDOWS_PHONE_7||WINDOWS_PHONE_8
		/// <summary>
		/// Class MVVMPage.
		/// </summary>
        public partial class MVVMPage : PhoneApplicationPage, IView
#else
		public class MVVMPage : Page, IView
#endif
		{



			/// <summary>
			/// Initializes a new instance of the <see cref="MVVMPage"/> class.
			/// </summary>
			public MVVMPage()
				: this(null)
			{

			}



#if WPF


			public Frame Frame
			{
				get { return (Frame)GetValue(FrameProperty); }
				set { SetValue(FrameProperty, value); }
			}

			// Using a DependencyProperty as the backing store for Frame.  This enables animation, styling, binding, etc...
			public static readonly DependencyProperty FrameProperty =
				DependencyProperty.Register("Frame", typeof(Frame), typeof(MVVMPage), new PropertyMetadata(null));


			DependencyObject IView.Parent
			{
				get
				{
					return Frame;

				}
			}
#endif

			/// <summary>
			/// Initializes a new instance of the <see cref="MVVMPage"/> class.
			/// </summary>
			/// <param name="viewModel">The view model.</param>
			public MVVMPage(IViewModel viewModel)
			{
				ViewModel = viewModel;
				Unloaded += ViewHelper.ViewUnloadCallBack;
#if WPF
				Loaded += async (o, e) =>
					{
						await ViewModel.OnBindedViewLoad(this);

					};
#endif

			}




#if ! WPF
			//WPF Pages' Content are objects but others are FE .
			/// <summary>
			/// Gets or sets the content object.
			/// </summary>
			/// <value>The content object.</value>
			public object ContentObject
			{
				get { return Content; }
				set { Content = value as FrameworkElement; }

			}

			/// <summary>
			/// The is loaded
			/// </summary>
			bool IsLoaded = false;

			//WPF navigates page instances but other navgates with parameters
			/// <summary>
			/// Handles the <see cref="E:NavigatedTo" /> event.
			/// </summary>
			/// <param name="e">The <see cref="NavigationEventArgs"/> instance containing the event data.</param>
			protected override void OnNavigatedTo(NavigationEventArgs e)
			{

				base.OnNavigatedTo(e);
				RoutedEventHandler loadEvent = null;

				loadEvent = async (_1, _2) =>
				{


					EventRouting.EventRouter.Instance.RaiseEvent(this, e);

					if (ViewModel != null)
					{
						await ViewModel.OnBindedViewLoad(this);
					}

					IsLoaded = true;
					this.Loaded -= loadEvent;





				};
				this.Loaded += loadEvent;

			}




			/// <summary>
			/// Handles the <see cref="E:NavigatedFrom" /> event.
			/// </summary>
			/// <param name="e">The <see cref="NavigationEventArgs"/> instance containing the event data.</param>
			protected override void OnNavigatedFrom(NavigationEventArgs e)
			{
				base.OnNavigatedFrom(e);

#if SILVERLIGHT_5
                if (ViewModel.StageManager.DefaultStage.NavigateRequestContexts.ContainsKey(e.Uri.ToString()))
#else
				if (e.NavigationMode == NavigationMode.Back)
#endif

				{

					if (ViewModel != null)
					{

						ViewModel.Dispose();
					}


				}

			}
#else
			public object ContentObject
			{
				get
				{
					return Content;
				}
				set
				{
					Content = value;
				}
			}
#endif



			//public IViewModel DesigningViewModel
			//{
			//	get { return (IViewModel)GetValue(DesigningViewModelProperty); }
			//	set { SetValue(DesigningViewModelProperty, value); }
			//}

			//// Using a DependencyProperty as the backing store for DesigningViewModel.  This enables animation, styling, binding, etc...
			//public static readonly DependencyProperty DesigningViewModelProperty =
			//	DependencyProperty.Register("DesigningViewModel", typeof(IViewModel), typeof(MVVMPage), new PropertyMetadata(null, ViewHelper.DesigningViewModelChangedCallBack));


			/// <summary>
			/// Gets or sets the view model.
			/// </summary>
			/// <value>The view model.</value>
			public IViewModel ViewModel
			{
				get
				{
					var rval = GetValue(ViewModelProperty) as IViewModel;
					var c = this.GetContentAndCreateIfNull();
					if (rval == null)
					{

						rval = c.DataContext as IViewModel;
						SetValue(ViewModelProperty, rval);

					}
					else
					{

						if (!Object.ReferenceEquals(c.DataContext, rval))
						{
							c.DataContext = rval;
						}
					}
					return rval;
				}
				set
				{

					SetValue(ViewModelProperty, value);
					var c = this.GetContentAndCreateIfNull();
					if (!Object.ReferenceEquals(c.DataContext, value))
					{
						c.DataContext = value;
					}

				}
			}

			// Using a DependencyProperty as the backing store for ViewModel.  This enables animation, styling, binding, etc...
			/// <summary>
			/// The view model property
			/// </summary>
			public static readonly DependencyProperty ViewModelProperty =
				DependencyProperty.Register("ViewModel", typeof(IViewModel), typeof(MVVMPage), new PropertyMetadata(null,
					(o, e) =>
					{
						var p = o as MVVMPage;
#if !WPF
						if (p.IsLoaded)
						{
							ViewHelper.ViewModelChangedCallback(o, e);
						}
#else
						ViewHelper.ViewModelChangedCallback(o, e);
#endif
					}

					));


#if NETFX_CORE

			/// <summary>
			/// Populates the page with content passed during navigation.  Any saved state is also
			/// provided when recreating a page from a prior session.
			/// </summary>
			/// <param name="navigationParameter">The parameter value passed to
			/// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
			/// </param>
			/// <param name="pageState">A dictionary of state preserved by this page during an earlier
			/// session.  This will be null the first time a page is visited.</param>
			protected virtual void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
			{
				if (ViewModel != null)
				{
					ViewModel.LoadState(navigationParameter, pageState);
				}
			}

			/// <summary>
			/// Preserves state associated with this page in case the application is suspended or the
			/// page is discarded from the navigation cache.  Values must conform to the serialization
			/// requirements of <see cref="SuspensionManager.SessionState"/>.
			/// </summary>
			/// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
			protected virtual void SaveState(Dictionary<String, Object> pageState)
			{
				if (ViewModel != null)
				{
					ViewModel.SaveState(pageState);

				}
			}
#endif


			/// <summary>
			/// Gets the type of the view.
			/// </summary>
			/// <value>The type of the view.</value>
			public ViewType ViewType
			{
				get { return ViewType.Page; }
			}




		}



		/// <summary>
		/// Class MVVMControl.
		/// </summary>
		public class MVVMControl : UserControl, IView
		{

			/// <summary>
			/// Initializes a new instance of the <see cref="MVVMControl"/> class.
			/// </summary>
			public MVVMControl()
				: this(null)
			{

			}


			/// <summary>
			/// Initializes a new instance of the <see cref="MVVMControl"/> class.
			/// </summary>
			/// <param name="viewModel">The view model.</param>
			public MVVMControl(IViewModel viewModel)
			{

				Unloaded += ViewHelper.ViewUnloadCallBack;
				////////// Unloaded += (_1, _2) => ViewModel = null;
				Loaded += async (_1, _2) =>
				{

					//if (viewModel != null)
					//{
					//    //this.Resources[ViewHelper.DEFAULT_VM_NAME] = viewModel;
					//    if (!object.ReferenceEquals(ViewModel, viewModel))
					//    {
					//        ViewModel = viewModel;

					//    }
					//}
					//else
					//{
					//    var solveV = this.GetDefaultViewModel();
					//    if (solveV != null)
					//    {
					//        ViewModel = solveV;
					//    }

					//}
					//ViewModel = ViewModel ?? new DefaultViewModel();


					await ViewModel.OnBindedViewLoad(this);
				};
			}
#if !WPF
			/// <summary>
			/// Gets or sets the content object.
			/// </summary>
			/// <value>The content object.</value>
			public object ContentObject
			{
				get { return Content; }
				set { Content = value as FrameworkElement; }

			}
#else
			public object ContentObject
			{
				get
				{
					return Content;
				}
				set
				{
					Content = value;
				}
			}
#endif
#if NETFX_CORE

			/// <summary>
			/// Populates the page with content passed during navigation.  Any saved state is also
			/// provided when recreating a page from a prior session.
			/// </summary>
			/// <param name="navigationParameter">The parameter value passed to
			/// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
			/// </param>
			/// <param name="pageState">A dictionary of state preserved by this page during an earlier
			/// session.  This will be null the first time a page is visited.</param>
			protected virtual void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
			{
				if (ViewModel != null)
				{
					ViewModel.LoadState(navigationParameter, pageState);
				}
			}

			/// <summary>
			/// Preserves state associated with this page in case the application is suspended or the
			/// page is discarded from the navigation cache.  Values must conform to the serialization
			/// requirements of <see cref="SuspensionManager.SessionState"/>.
			/// </summary>
			/// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
			protected virtual void SaveState(Dictionary<String, Object> pageState)
			{
				if (ViewModel != null)
				{
					ViewModel.SaveState(pageState);
				}
			}
#endif

			//public IViewModel DesigningViewModel
			//{
			//	get { return (IViewModel)GetValue(DesigningViewModelProperty); }
			//	set { SetValue(DesigningViewModelProperty, value); }
			//}

			//// Using a DependencyProperty as the backing store for DesigningViewModel.  This enables animation, styling, binding, etc...
			//public static readonly DependencyProperty DesigningViewModelProperty =
			//	DependencyProperty.Register("DesigningViewModel", typeof(IViewModel), typeof(MVVMControl), new PropertyMetadata(null, ViewHelper.DesigningViewModelChangedCallBack));
			/// <summary>
			/// Gets or sets the view model.
			/// </summary>
			/// <value>The view model.</value>
			public IViewModel ViewModel
			{
				get
				{
					var vm = GetValue(ViewModelProperty) as IViewModel;
					var content = this.GetContentAndCreateIfNull();
					if (vm == null)
					{

						vm = content.DataContext as IViewModel;
						SetValue(ViewModelProperty, vm);

					}
					else
					{
						IView view = this;


						if (!Object.ReferenceEquals(content.DataContext, vm))
						{

							content.DataContext = vm;
						}
					}
					return vm;
				}
				set
				{
					SetValue(ViewModelProperty, value);
					var c = this.GetContentAndCreateIfNull();
					if (!Object.ReferenceEquals(c.DataContext, value))
					{
						c.DataContext = value;
					}

				}
			}

			// Using a DependencyProperty as the backing store for ViewModel.  This enables animation, styling, binding, etc...
			/// <summary>
			/// The view model property
			/// </summary>
			public static readonly DependencyProperty ViewModelProperty =
				DependencyProperty.Register("ViewModel", typeof(IViewModel), typeof(MVVMControl), new PropertyMetadata(null, ViewHelper.ViewModelChangedCallback));


			/// <summary>
			/// Gets the type of the view.
			/// </summary>
			/// <value>The type of the view.</value>
			public ViewType ViewType
			{
				get { return ViewType.Control; }
			}

		}
		public enum ViewType
		{
			/// <summary>
			/// The page
			/// </summary>
			Page,
			/// <summary>
			/// The window
			/// </summary>
			Window,
			/// <summary>
			/// The control
			/// </summary>
			Control
		}

		/// <summary>
		/// Interface IView
		/// </summary>
		public interface IView
		{
			/// <summary>
			/// Gets or sets the view model.
			/// </summary>
			/// <value>The view model.</value>
			IViewModel ViewModel { get; set; }

			/// <summary>
			/// Gets the type of the view.
			/// </summary>
			/// <value>The type of the view.</value>
			ViewType ViewType { get; }

			/// <summary>
			/// Gets or sets the content object.
			/// </summary>
			/// <value>The content object.</value>
			Object ContentObject { get; set; }

			/// <summary>
			/// Gets the parent.
			/// </summary>
			/// <value>The parent.</value>
			DependencyObject Parent { get; }

		}


		/// <summary>
		/// Interface IView
		/// </summary>
		/// <typeparam name="TViewModel">The type of the t view model.</typeparam>
		public interface IView<TViewModel> : IView, IDisposable where TViewModel : IViewModel
		{
			/// <summary>
			/// Gets or sets the specific typed view model.
			/// </summary>
			/// <value>The specific typed view model.</value>
			TViewModel SpecificTypedViewModel { get; set; }
		}

		/// <summary>
		/// Struct ViewModelToViewMapper
		/// </summary>
		/// <typeparam name="TModel">The type of the t model.</typeparam>
		public struct ViewModelToViewMapper<TModel>
			where TModel : IViewModel
		{

			/// <summary>
			/// Maps the view to view model.
			/// </summary>
			/// <typeparam name="TView">The type of the t view.</typeparam>
			public static void MapViewToViewModel<TView>()
			{
				Func<IViewModel> func;
				if (!ViewModelToViewMapperHelper.ViewToVMMapping.TryGetValue(typeof(TView), out func))
				{
					ViewModelToViewMapperHelper.ViewToVMMapping.Add(typeof(TView), () => (ViewModelLocator<TModel>.Instance.Resolve()));
				}


			}
#if WPF
			public ViewModelToViewMapper<TModel> MapToDefault<TView>(TView instance) where TView : class,IView
			{
				MapViewToViewModel<TView>();
				ViewModelToViewMapperServiceLocator<TModel>.Instance.Register(instance);
				return this;
			}

			public ViewModelToViewMapper<TModel> MapTo<TView>(string viewMappingKey, TView instance) where TView : class,IView
			{
				MapViewToViewModel<TView>();
				ViewModelToViewMapperServiceLocator<TModel>.Instance.Register(viewMappingKey, instance);
				return this;
			}


			public ViewModelToViewMapper<TModel> MapToDefault<TView>(bool alwaysNew = true) where TView : class,IView
			{
				MapViewToViewModel<TView>();
				ViewModelToViewMapperServiceLocator<TModel>.Instance.Register(null, d => (TView)Activator.CreateInstance(typeof(TView), d as object), alwaysNew);
				return this;
			}
			public ViewModelToViewMapper<TModel> MapTo<TView>(string viewMappingKey, bool alwaysNew = true) where TView : class,IView
			{
				MapViewToViewModel<TView>();
				ViewModelToViewMapperServiceLocator<TModel>.Instance.Register(viewMappingKey, d => (TView)Activator.CreateInstance(typeof(TView), d as object), alwaysNew);
				return this;
			}

			public ViewModelToViewMapper<TModel> MapToDefault<TView>(Func<TModel, TView> factory, bool alwaysNew = true) where TView : class,IView
			{
				MapViewToViewModel<TView>();
				ViewModelToViewMapperServiceLocator<TModel>.Instance.Register(null, d => factory((TModel)d), alwaysNew);
				return this;
			}

			public ViewModelToViewMapper<TModel> MapTo<TView>(string viewMappingKey, Func<TModel, TView> factory, bool alwaysNew = true) where TView : class,IView
			{
				MapViewToViewModel<TView>();
				ViewModelToViewMapperServiceLocator<TModel>.Instance.Register(viewMappingKey, d => factory((TModel)d), alwaysNew);
				return this;
			}
#else
			public ViewModelToViewMapper<TModel> MapToDefaultControl<TControl>(TControl instance) where TControl : MVVMControl
			{
				MapViewToViewModel<TControl>();
				ViewModelToViewMapperServiceLocator<TModel>.Instance.Register(instance);
				return this;
			}

			public ViewModelToViewMapper<TModel> MapToControl<TControl>(string viewMappingKey, TControl instance) where TControl : MVVMControl
			{
				MapViewToViewModel<TControl>();
				ViewModelToViewMapperServiceLocator<TModel>.Instance.Register(viewMappingKey, instance);
				return this;
			}


			public ViewModelToViewMapper<TModel> MapToDefaultControl<TControl>(bool alwaysNew = true) where TControl : MVVMControl
			{
				MapViewToViewModel<TControl>();
				ViewModelToViewMapperServiceLocator<TModel>.Instance.Register(null, d => (TControl)Activator.CreateInstance(typeof(TControl), d as object), alwaysNew);
				return this;
			}
			public ViewModelToViewMapper<TModel> MapToControl<TControl>(string viewMappingKey, bool alwaysNew = true) where TControl : MVVMControl
			{
				MapViewToViewModel<TControl>();
				ViewModelToViewMapperServiceLocator<TModel>.Instance.Register(viewMappingKey, d => (TControl)Activator.CreateInstance(typeof(TControl), d as object), alwaysNew);
				return this;
			}

			public ViewModelToViewMapper<TModel> MapToDefaultControl<TControl>(Func<TModel, TControl> factory, bool alwaysNew = true) where TControl : MVVMControl
			{
				MapViewToViewModel<TControl>();
				ViewModelToViewMapperServiceLocator<TModel>.Instance.Register(null, d => factory((TModel)d), alwaysNew);
				return this;
			}

			public ViewModelToViewMapper<TModel> MapToControl<TControl>(string viewMappingKey, Func<TModel, TControl> factory, bool alwaysNew = true) where TControl : MVVMControl
			{
				MapViewToViewModel<TControl>();
				ViewModelToViewMapperServiceLocator<TModel>.Instance.Register(viewMappingKey, d => factory((TModel)d), alwaysNew);
				return this;
			}
#endif

#if WINDOWS_PHONE_8||WINDOWS_PHONE_7||SILVERLIGHT_5
            private static Uri GuessViewUri<TPage>(Uri baseUri) where TPage : MVVMPage
            {
                MapViewToViewModel<TPage>();

                baseUri = baseUri ?? new Uri("/", UriKind.Relative);


                if (baseUri.IsAbsoluteUri)
                {
                    var path = Path.Combine(baseUri.LocalPath, typeof(TPage).Name + ".xaml");
                    UriBuilder ub = new UriBuilder(baseUri);
                    ub.Path = path;
                    return ub.Uri;
                }
                else
                {
                    var path = Path.Combine(baseUri.OriginalString, typeof(TPage).Name + ".xaml");
                    var pageUri = new Uri(path, UriKind.Relative);
                    return pageUri;
                }
            }

            public ViewModelToViewMapper<TModel> MapToDefault<TPage>(Uri baseUri = null) where TPage : MVVMPage
            {
				/// <summary>
				/// Enum ViewType
				/// </summary>
                MapViewToViewModel<TPage>();
                var pageUri = GuessViewUri<TPage>(baseUri);
                ViewModelToViewMapperServiceLocator<TModel>.Instance.Register(
                    Tuple.Create<Uri, Func<IView>>(pageUri,
                    () => Activator.CreateInstance(typeof(TPage)) as IView
					/// <summary>
					/// The page
					/// </summary>
                    ));
                return this;
            }

			/// <summary>
			/// The window
			/// </summary>



            public ViewModelToViewMapper<TModel> MapTo<TPage>(string viewMappingKey, Uri baseUri = null) where TPage : MVVMPage
			/// <summary>
			/// The control
			/// </summary>
            {
                MapViewToViewModel<TPage>();
                var pageUri = GuessViewUri<TPage>(baseUri);
                ViewModelToViewMapperServiceLocator<TModel>.Instance.Register(Tuple.Create<Uri, Func<IView>>(pageUri,
                        () => Activator.CreateInstance(typeof(TPage)) as IView
                        ));
                return this;
            }

            //public ViewModelToViewMapper<TModel> MapToDefault(Uri pageUri)
            //{
			/// <summary>
			/// The page
			/// </summary>
            //    ViewModelToViewMapperServiceLocator<TModel>.Instance.Register(pageUri);
            //    return this;
            //}

			/// <summary>
			/// The window
			/// </summary>
            //public ViewModelToViewMapper<TModel> MapTo(string viewMappingKey, Uri pageUri)
            //{
            //    ViewModelToViewMapperServiceLocator<TModel>.Instance.Register(viewMappingKey, pageUri);
            //    return this;
			/// <summary>
			/// The control
			/// </summary>
            //}
#endif
#if NETFX_CORE



			public ViewModelToViewMapper<TModel> MapToDefault<TPage>() where TPage : MVVMPage
			{

				MapViewToViewModel<TPage>();
				/// <summary>
				/// The page
				/// </summary>
				ViewModelToViewMapperServiceLocator<TModel>.Instance.Register(typeof(TPage));
				return this;
			}


			public ViewModelToViewMapper<TModel> MapToDefault<TPage>(string viewMappingKey) where TPage : MVVMPage
			{
				/// <summary>
				/// The window
				/// </summary>
				MapViewToViewModel<TPage>();
				ViewModelToViewMapperServiceLocator<TModel>.Instance.Register(viewMappingKey, typeof(TPage));
				return this;
			}



/// <summary>
/// The control
/// </summary>
#endif


		}

		public static class ViewModelToViewMapperHelper
		{

			internal static Dictionary<Type, Func<IViewModel>> ViewToVMMapping = new Dictionary<Type, Func<IViewModel>>();

			public static IViewModel GetDefaultViewModel(this IView view)
			{
				Func<IViewModel> func;
				var vt = view.GetType();
				if (ViewModelToViewMapperHelper.ViewToVMMapping.TryGetValue(vt, out func))
				{
					/// <summary>
					/// The page
					/// </summary>
					return func();
				}
				return null;
			}

			public static ViewModelToViewMapper<TViewModel> GetViewMapper<TViewModel>(this MVVMSidekick.Services.ServiceLocatorEntryStruct<TViewModel> vmRegisterEntry)
				  where TViewModel : IViewModel
			{
				return new ViewModelToViewMapper<TViewModel>();
			}
			/// <summary>
			/// The window
			/// </summary>

		}
		public class ViewModelToViewMapperServiceLocator<TViewModel> : MVVMSidekick.Services.TypeSpecifiedServiceLocatorBase<ViewModelToViewMapperServiceLocator<TViewModel>, object>
		{
			static ViewModelToViewMapperServiceLocator()
			{
				Instance = new ViewModelToViewMapperServiceLocator<TViewModel>();
			}
			public static ViewModelToViewMapperServiceLocator<TViewModel> Instance { get; set; }

			/// <summary>
			/// The control
			/// </summary>

		}
		public class ViewModelLocator<TViewModel> : MVVMSidekick.Services.TypeSpecifiedServiceLocatorBase<ViewModelLocator<TViewModel>, TViewModel>
			where TViewModel : IViewModel
		{
			static ViewModelLocator()
			{
				Instance = new ViewModelLocator<TViewModel>();
			}
			public static ViewModelLocator<TViewModel> Instance { get; set; }

		}


		public class Stage : DependencyObject
		{
			public Stage(FrameworkElement target, string beaconKey, StageManager navigator)
			{
				Target = target;
				_navigator = navigator;
				BeaconKey = beaconKey;
				//SetupNavigateFrame();
			}

			StageManager _navigator;
			FrameworkElement _target;




			public Frame Frame
			{
				get { return (Frame)GetValue(FrameProperty); }
				private set { SetValue(FrameProperty, value); }
			}

			// Using a DependencyProperty as the backing store for Frame.  This enables animation, styling, binding, etc...
			public static readonly DependencyProperty FrameProperty =
				DependencyProperty.Register("Frame", typeof(Frame), typeof(Stage), new PropertyMetadata(null));



			public FrameworkElement Target
			{
				get { return _target; }
				private set
				{
					_target = value;
					Frame = _target as Frame;


				}
			}
#if WPF
			public bool IsGoForwardSupported
			{
				get
				{
					return Frame != null;
				}
			}

			public bool CanGoForward
			{
				get
				{
					return IsGoForwardSupported ? Frame.CanGoForward : false;
				}

			}
#else
			public bool IsGoForwardSupported
			{
				get
				{
					return false;
				}
			}

			public bool CanGoForward
			{
				get
				{
					return false;
				}

			}
#endif
			public bool IsGoBackSupported
			{
				get
				{
					return Frame != null;
				}
			}


			public bool CanGoBack
			{
				get
				{
					return IsGoBackSupported ? Frame.CanGoBack : false;
				}

			}




			public string BeaconKey
			{
				get { return (string)GetValue(BeaconKeyProperty); }
				private set { SetValue(BeaconKeyProperty, value); }
			}

			// Using a DependencyProperty as the backing store for BeaconKey.  This enables animation, styling, binding, etc...
			public static readonly DependencyProperty BeaconKeyProperty =
				DependencyProperty.Register("BeaconKey", typeof(string), typeof(Stage), new PropertyMetadata(""));


#if WPF
			private static IView InternalLocateViewIfNotSet<TTarget>(TTarget targetViewModel, string viewMappingKey, IView view) where TTarget : class, IViewModel
			{
				if (targetViewModel != null && targetViewModel.StageManager != null)
				{
					view = targetViewModel.StageManager.CurrentBindingView as IView;

				}
				view = view ?? ViewModelToViewMapperServiceLocator<TTarget>.Instance.Resolve(viewMappingKey, targetViewModel) as IView;
				return view;
			}





			public async Task Show<TTarget>(TTarget targetViewModel = null, string viewMappingKey = null)
				 where TTarget : class,IViewModel
			{
				IView view = null;
				view = InternalLocateViewIfNotSet<TTarget>(targetViewModel, viewMappingKey, view);
				targetViewModel = targetViewModel ?? view.ViewModel as TTarget;
				if (view.ViewType == ViewType.Page)
				{
					var mvpg = view as MVVMPage;
					mvpg.Frame = Frame;
				}

				SetVMAfterLoad(targetViewModel, view);
				InternalShowView(view, Target, _navigator.CurrentBindingView.ViewModel);
				await targetViewModel.WaitForClose();
			}


			public async Task<TResult> Show<TTarget, TResult>(TTarget targetViewModel = null, string viewMappingKey = null)
				where TTarget : class,IViewModel<TResult>
			{
				IView view = null;
				view = InternalLocateViewIfNotSet<TTarget>(targetViewModel, viewMappingKey, view);
				targetViewModel = targetViewModel ?? view.ViewModel as TTarget;

				if (view.ViewType == ViewType.Page)
				{
					var mvpg = view as MVVMPage;
					mvpg.Frame = Frame;
				}

				SetVMAfterLoad(targetViewModel, view);

				InternalShowView(view, Target, _navigator.CurrentBindingView.ViewModel);
				return await targetViewModel.WaitForCloseWithResult();
			}



			public async Task<ShowAwaitableResult<TTarget>> ShowAndGetViewModel<TTarget>(TTarget targetViewModel = null, string viewMappingKey = null)
				where TTarget : class,IViewModel
			{
				IView view = null;
				view = InternalLocateViewIfNotSet<TTarget>(targetViewModel, viewMappingKey, view);

				targetViewModel = targetViewModel ?? view.ViewModel as TTarget;
				if (view.ViewType == ViewType.Page)
				{
					var mvpg = view as MVVMPage;
					mvpg.Frame = Frame;
				}

				SetVMAfterLoad(targetViewModel, view);
				InternalShowView(view, Target, _navigator.CurrentBindingView.ViewModel);


				return await TaskExHelper.FromResult(new ShowAwaitableResult<TTarget> { Closing = targetViewModel.WaitForClose(), ViewModel = targetViewModel });
			}
#endif
#if SILVERLIGHT_5||WINDOWS_PHONE_7||WINDOWS_PHONE_8

            internal Dictionary<string, IViewModel> NavigateRequestContexts
            {
                get
                {

                    return GetNavigateRequestContexts(Frame);
                }

                set
                {
                    SetNavigateRequestContexts(Frame, value);
                }

            }

            public static Dictionary<string, IViewModel> GetNavigateRequestContexts(DependencyObject obj)
            {
                return (Dictionary<string, IViewModel>)obj.GetValue(NavigateRequestContextsProperty);
            }

            public static void SetNavigateRequestContexts(DependencyObject obj, Dictionary<string, IViewModel> value)
            {
                obj.SetValue(NavigateRequestContextsProperty, value);
            }

            // Using a DependencyProperty as the backing store for NavigateRequestContexts.  This enables animation, styling, binding, etc...
            public static readonly DependencyProperty NavigateRequestContextsProperty =
                DependencyProperty.RegisterAttached("NavigateRequestContexts", typeof(Dictionary<string, IViewModel>), typeof(Stage), new PropertyMetadata(new Dictionary<string, IViewModel>()));

            public async Task Show<TTarget>(TTarget targetViewModel = null, string viewMappingKey = null)
                 where TTarget : class,IViewModel
            {

                var item = ViewModelToViewMapperServiceLocator<TTarget>.Instance.Resolve(viewMappingKey, targetViewModel);



                Tuple<Uri, Func<IView>> uriData;
                uriData = item as Tuple<Uri, Func<IView>>;
                if (uriData != null) //only sl like page Can be registered as uri
                {
                    Frame frame;
                    if ((frame = Target as Frame) != null)
                    {
                        targetViewModel = await FrameNavigate<TTarget>(targetViewModel, uriData, frame);
                        await targetViewModel.WaitForClose();
                        return;
                    }
                    else
                    {
                        item = uriData.Item2();
                    }
                }

                IView view = item as IView;
                targetViewModel = targetViewModel ?? view.ViewModel as TTarget;
                SetVMAfterLoad(targetViewModel, view);
                InternalShowView(view, Target, _navigator.CurrentBindingView.ViewModel);
                await targetViewModel.WaitForClose();
            }


			//public async Task<TResult> Show<TTarget, TResult>(TTarget targetViewModel = null, string viewMappingKey = null)
			//	where TTarget : class,IViewModel<TResult>
			//{

			//	var item = ViewModelToViewMapperServiceLocator<TTarget>.Instance.Resolve(viewMappingKey, targetViewModel);

			//	Tuple<Uri, Func<IView>> uriData;
			//	if ((uriData = item as Tuple<Uri, Func<IView>>) != null) //only sl like page Can be registered as uri
			//	{
			//		Frame frame;
			//		if ((frame = Target as Frame) != null)
			//		{
			//			targetViewModel = await FrameNavigate<TTarget>(targetViewModel, uriData, frame);

			//			return await targetViewModel.WaitForCloseWithResult();
			//		}
			//		else
			//		{
			//			item = uriData.Item2();
			//		}
			//	}
			//	IView view = item as IView;
			//	targetViewModel = targetViewModel ?? view.ViewModel as TTarget;		
			//	SetVMAfterLoad(targetViewModel, view);								
			//	InternalShowView(view, Target, _navigator.CurrentBindingView.ViewModel);
			//	return await targetViewModel.WaitForCloseWithResult();
			//}

            private async Task<TTarget> FrameNavigate<TTarget>(TTarget targetViewModel, Tuple<Uri, Func<IView>> uriData, Frame frame) where TTarget : class,IViewModel
            {
                var t = new TaskCompletionSource<object>();
                var guid = Guid.NewGuid();
                var newUriWithParameter = new Uri(uriData.Item1.ToString() + "?CallBackGuid=" + guid.ToString(), UriKind.Relative);

                var dis = EventRouting.EventRouter.Instance.GetEventObject<System.Windows.Navigation.NavigationEventArgs>()

                    .Where(e =>
                            e.EventArgs.Uri == newUriWithParameter)
                    .Subscribe(e =>
                    {
                        var key = newUriWithParameter.ToString();

                        lock (NavigateRequestContexts)
                        {
                            IViewModel vm = null;

                            if (NavigateRequestContexts.TryGetValue(key, out vm))
                            {
                                targetViewModel = vm as TTarget;
                            }

                            var page = e.Sender as MVVMPage;

                            if (targetViewModel != null)
                            {
                                page.ViewModel = targetViewModel;
               
                            }
                            else
                            {
                                var solveV = page.GetDefaultViewModel();
                                if (solveV != null)
                                {

                                    page.ViewModel = solveV;
                                }

                            }

                            targetViewModel = (TTarget)page.ViewModel;
                            NavigateRequestContexts[key] = targetViewModel;
                        }
                        if (!t.Task.IsCompleted)
                        {

                            targetViewModel.AddDisposeAction(() =>
                            {
                                lock (NavigateRequestContexts)
                                {
                                    NavigateRequestContexts.Remove(key);
                                }

                            });
                            t.SetResult(null);
                        }
                    }
                    );



                frame.Navigate(newUriWithParameter);
                await t.Task;
                dis.DisposeWith(targetViewModel);
                return targetViewModel;
            }



			//public async Task<ShowAwaitableResult<TTarget>> ShowAndGetViewModel<TTarget>(TTarget targetViewModel = null, string viewMappingKey = null)
			//	where TTarget : class,IViewModel
			//{
			//	var item = ViewModelToViewMapperServiceLocator<TTarget>.Instance.Resolve(viewMappingKey, targetViewModel);
			//	Tuple<Uri, Func<IView>> uriData;
			//	if ((uriData = item as Tuple<Uri, Func<IView>>) != null) //only sl like page Can be registered as uri
			//	{
			//		Frame frame;
			//		if ((frame = Target as Frame) != null)
			//		{
			//			targetViewModel = await FrameNavigate<TTarget>(targetViewModel, uriData, frame);

			//			return new ShowAwaitableResult<TTarget> { Closing = targetViewModel.WaitForClose(), ViewModel = targetViewModel };

			//		}
			//		else
			//		{
			//			item = uriData.Item2();

			//		}
			//	}


			//	IView view = item as IView;
			//	targetViewModel = targetViewModel ?? view.ViewModel as TTarget;
			//	SetVMAfterLoad(targetViewModel, view);
			//	InternalShowView(view, Target, _navigator.CurrentBindingView.ViewModel);
			//	var tr = targetViewModel.WaitForClose();
			//	return new ShowAwaitableResult<TTarget> { Closing = targetViewModel.WaitForClose(), ViewModel = targetViewModel };
			//}

#endif


			private static void SetVMAfterLoad<TTarget>(TTarget targetViewModel, IView view) where TTarget : class, IViewModel
			{
				if (view == null)
				{
					throw new InvalidOperationException(
						string.Format(@"
Cannot find ANY mapping from View Model [{0}] to ANY View.
Please check startup function of this mapping is well configured and be proper called while application starting",
						targetViewModel.GetType().ToString()));
				}
				if (view.ViewType == ViewType.Page)
				{
					var pg = view as MVVMPage;

					pg.ViewModel = targetViewModel;
				}
				view.ViewModel = targetViewModel;

				//var frview = view as FrameworkElement;
				//RoutedEventHandler loadEvent = null;
				//loadEvent = (_1, _2) =>
				//{
				//    view.ViewModel = targetViewModel;
				//    frview.Loaded -= loadEvent;
				//};
				//frview.Loaded += loadEvent;

			}

#if NETFX_CORE




			public async Task Show<TTarget>(TTarget targetViewModel = null, string viewMappingKey = null)
				 where TTarget : class,IViewModel
			{


				var item = ViewModelToViewMapperServiceLocator<TTarget>.Instance.Resolve(viewMappingKey, targetViewModel);
				Type type;
				if ((type = item as Type) != null) //only MVVMPage Can be registered as Type
				{

					var frame = Target as Frame;
					if (frame != null)
					{
						targetViewModel = await FrameNavigate<TTarget>(targetViewModel, type, frame);

						await targetViewModel.WaitForClose();
						return;
					}

				}

				IView view = item as IView;
				targetViewModel = targetViewModel ?? view.ViewModel as TTarget;
				SetVMAfterLoad(targetViewModel, view);
				InternalShowView(view, Target, _navigator.CurrentBindingView.ViewModel);
				await targetViewModel.WaitForClose();


			}

			private static async Task<TTarget> FrameNavigate<TTarget>(TTarget targetViewModel, Type type, Windows.UI.Xaml.Controls.Frame frame) where TTarget : class, IViewModel
			{
				var parameter = new StageNavigationContext<TTarget>() { ViewModel = targetViewModel };
				var t = new TaskCompletionSource<object>();
				var dip = EventRouting.EventRouter.Instance
					 .GetEventObject<NavigationEventArgs>()

					 .Where(e =>
							 object.ReferenceEquals(e.EventArgs.Parameter, parameter))
					 .Subscribe(e =>
					 {

						 var page = e.Sender as MVVMPage;

						 if (parameter.ViewModel != null)
						 {
							 page.ViewModel = parameter.ViewModel;
						 }
						 else
						 {
							 var solveV = page.GetDefaultViewModel();
							 if (solveV != null)
							 {
								 targetViewModel = parameter.ViewModel = (TTarget)solveV;
							 }
						 }

						 if (targetViewModel == null)
						 {
							 targetViewModel = (TTarget)page.ViewModel;
						 }





						 parameter.ViewModel = targetViewModel;

						 if (!t.Task.IsCompleted)
						 {

							 t.SetResult(null); ;
						 }
					 });


				frame.Navigate(type, parameter);
				await t.Task;
				dip.DisposeWith(targetViewModel);
				return targetViewModel;
			}
			class StageNavigationContext<T> where T : IViewModel
			{
				public T ViewModel { get; set; }

			}
			public async Task<TResult> Show<TTarget, TResult>(TTarget targetViewModel = null, string viewMappingKey = null)
				where TTarget : class,IViewModel<TResult>
			{
				var item = ViewModelToViewMapperServiceLocator<TTarget>.Instance.Resolve(viewMappingKey, targetViewModel);
				Type type;
				if ((type = item as Type) != null) //only MVVMPage Can be registered as Type
				{
					Frame frame;
					if ((frame = Target as Frame) != null)
					{
						targetViewModel = await FrameNavigate<TTarget>(targetViewModel, type, frame);




						return await targetViewModel.WaitForCloseWithResult();
					}

				}


				IView view = item as IView;
				targetViewModel = targetViewModel ?? view.ViewModel as TTarget;
				SetVMAfterLoad(targetViewModel, view);
				InternalShowView(view, Target, _navigator.CurrentBindingView.ViewModel);
				return await targetViewModel.WaitForCloseWithResult();
			}



			public async Task<ShowAwaitableResult<TTarget>> ShowAndGetViewModel<TTarget>(TTarget targetViewModel = null, string viewMappingKey = null)
	where TTarget : class,IViewModel
			{
				var item = ViewModelToViewMapperServiceLocator<TTarget>.Instance.Resolve(viewMappingKey, targetViewModel);
				Type type;
				if ((type = item as Type) != null) //only MVVMPage Can be registered as Type
				{
					Frame frame;
					if ((frame = Target as Frame) != null)
					{
						targetViewModel = await FrameNavigate<TTarget>(targetViewModel, type, frame);

						return new ShowAwaitableResult<TTarget>
						{
							Closing = targetViewModel.WaitForClose(),
							ViewModel = targetViewModel
						};
					}

				}


				IView view = item as IView;

				targetViewModel = targetViewModel ?? view.ViewModel as TTarget;
				SetVMAfterLoad(targetViewModel, view);
				InternalShowView(view, Target, _navigator.CurrentBindingView.ViewModel);

				var tr = targetViewModel.WaitForClose();
				return new ShowAwaitableResult<TTarget> { Closing = tr, ViewModel = targetViewModel };
			}
#endif



			private void InternalShowView(IView view, FrameworkElement target, IViewModel sourceVM)
			{

				if (view is UserControl || view is Page)
				{

					if (target is ContentControl)
					{
						var targetCControl = target as ContentControl;
						var oldcontent = targetCControl.Content as IDisposable;


						targetCControl.Content = view;
					}
					else if (target is Panel)
					{
						var targetPanelControl = target as Panel;

						targetPanelControl.Children.Add(view as UIElement);
					}
					else
					{
						throw new InvalidOperationException(string.Format("This view {0} is not support show in {1} ", view.GetType(), target.GetType()));
					}
				}
#if WPF
				else if (view is MVVMWindow)
				{
					var viewWindow = view as MVVMWindow;

					//viewWindow.HorizontalAlignment = HorizontalAlignment.Center;
					//viewWindow.VerticalAlignment = VerticalAlignment.Center;
					var targetWindow = target as Window;
					if (targetWindow == null)
					{
						targetWindow = sourceVM.StageManager.CurrentBindingView as Window;

					}
					if (viewWindow.IsAutoOwnerSetNeeded)
					{
						viewWindow.Owner = targetWindow;
					}	  
					viewWindow.Show();

				}
#endif
			}


		}

		/// <summary>
		/// The abstract  for frame/contentcontrol. VM can access this class to Show other vm and vm's mapped view.
		/// </summary>
		public class StageManager : DependencyObject
		{


			static StageManager()
			{
				NavigatorBeaconsKey = "NavigatorBeaconsKey";
			}
			public StageManager(IViewModel viewModel)
			{
				_ViewModel = viewModel;


			}
			IViewModel _ViewModel;

			/// <summary>
			/// This Key is a prefix for register keys. 
			/// The stage registeration store the String-Element-Mapping in view's Resource Dictionary(Resource property). 
			/// This can help not to overwrite the resources already defined.
			/// </summary>
			public static string NavigatorBeaconsKey;



			IView _CurrentBindingView;
			/// <summary>
			/// Get the currently binded view of this stagemanager. A stagemanager is for a certain view. If viewmodel is not binded to a view, the whole thing cannot work.
			/// </summary>
			public IView CurrentBindingView
			{
				get
				{

					return _CurrentBindingView;
				}
				internal set
				{
					_CurrentBindingView = value;
				}
			}







			public void InitParent(Func<DependencyObject> parentLocator)
			{
				_parentLocator = parentLocator;
				DefaultStage = this[""];
			}



			Func<DependencyObject> _parentLocator;


			#region Attached Property


#if WPF
			[AttachedPropertyBrowsableForType(typeof(ContentControl))]
			[AttachedPropertyBrowsableForType(typeof(Frame))]
			[AttachedPropertyBrowsableForType(typeof(Window))]
#endif
			public static string GetBeacon(DependencyObject obj)
			{
				return (string)obj.GetValue(BeaconProperty);
			}

#if WPF
			[AttachedPropertyBrowsableForType(typeof(ContentControl))]
			[AttachedPropertyBrowsableForType(typeof(Frame))]
			[AttachedPropertyBrowsableForType(typeof(Window))]
#endif

			public static void SetBeacon(DependencyObject obj, string value)
			{
				obj.SetValue(BeaconProperty, value);
			}

			public static readonly DependencyProperty BeaconProperty =
				DependencyProperty.RegisterAttached("Beacon", typeof(string), typeof(StageManager), new PropertyMetadata(null,
					   (o, p) =>
					   {
						   var name = (p.NewValue as string);
						   var target = o as FrameworkElement;

						   target.Loaded +=
							   (_1, _2)
							   =>
							   {
								   StageManager.RegisterTargetBeacon(name, target);
							   };
					   }

					   ));





			#endregion


			internal FrameworkElement LocateTargetContainer(IView view, ref string targetContainerName, IViewModel sourceVM)
			{
				targetContainerName = targetContainerName ?? "";
				var viewele = view as FrameworkElement;
				FrameworkElement target = null;



				var dic = GetOrCreateBeacons(sourceVM.StageManager.CurrentBindingView as FrameworkElement);
				dic.TryGetValue(targetContainerName, out target);


				if (target == null)
				{
					target = _parentLocator() as FrameworkElement;
				}

				if (target == null)
				{
					var vieweleCt = viewele as ContentControl;
					if (vieweleCt != null)
					{
						target = vieweleCt.Content as FrameworkElement;
					}
				}
				return target;
			}




			private static Dictionary<string, FrameworkElement> GetOrCreateBeacons(FrameworkElement view)
			{
				Dictionary<string, FrameworkElement> dic;
#if NETFX_CORE
				if (!view.Resources.ContainsKey(NavigatorBeaconsKey))
#else
				if (!view.Resources.Contains(NavigatorBeaconsKey))
#endif
				{
					dic = new Dictionary<string, FrameworkElement>();
					view.Resources.Add(NavigatorBeaconsKey, dic);
				}
				else
					dic = view.Resources[NavigatorBeaconsKey] as Dictionary<string, FrameworkElement>;

				return dic;
			}

			public static void RegisterTargetBeacon(string name, FrameworkElement target)
			{
				var view = LocateIView(target);
				var beacons = GetOrCreateBeacons(view);


				beacons[name] = target;


			}

			private static FrameworkElement LocateIView(FrameworkElement target)
			{

				var view = target;

				while (view != null)
				{
					if (view is IView)
					{
						break;
					}
					view = view.Parent as FrameworkElement;

				}
				return view;
			}







			public Stage DefaultStage
			{
				get { return (Stage)GetValue(DefaultTargetProperty); }
				set { SetValue(DefaultTargetProperty, value); }
			}

			// Using a DependencyProperty as the backing store for DefaultTarget.  This enables animation, styling, binding, etc...
			public static readonly DependencyProperty DefaultTargetProperty =
				DependencyProperty.Register("DefaultTarget", typeof(Stage), typeof(StageManager), new PropertyMetadata(null));



			public Stage this[string beaconKey]
			{
				get
				{
					var fr = LocateTargetContainer(CurrentBindingView, ref beaconKey, _ViewModel);
					if (fr != null)
					{
						return new Stage(fr, beaconKey, this);
					}
					else
						return null;
				}
			}
		}




		public class PropertyBridge : FrameworkElement
		{
			public PropertyBridge()
			{
				base.Width = 0;
				base.Height = 0;
				base.Visibility = Visibility.Collapsed;

			}




			public object Source
			{
				private get { return (object)GetValue(SourceProperty); }
				set { SetValue(SourceProperty, value); }
			}

			// Using a DependencyProperty as the backing store for Source.  This enables animation, styling, binding, etc...
			public static readonly DependencyProperty SourceProperty =
				DependencyProperty.Register("Source", typeof(object), typeof(PropertyBridge), new PropertyMetadata(null,

					(o, a) =>
					{
						var pb = o as PropertyBridge;
						if (pb != null)
						{
							pb.Target = a.NewValue;
						}
					}
					));




			public object Target
			{
				get { return (object)GetValue(TargetProperty); }
				set { SetValue(TargetProperty, value); }
			}

			// Using a DependencyProperty as the backing store for Target.  This enables animation, styling, binding, etc...
			public static readonly DependencyProperty TargetProperty =
				DependencyProperty.Register("Target", typeof(object), typeof(PropertyBridge), new PropertyMetadata(null));





		}



	}
}
