﻿using System;
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

using MVVMSidekick.Utilities;
#if NETFX_CORE
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Controls;
using System.Collections.Concurrent;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Controls.Primitives;


#elif WPF
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Collections.Concurrent;
using System.Windows.Navigation;

using MVVMSidekick.Views;
using System.Windows.Controls.Primitives;


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

namespace MVVMSidekick
{

    namespace Patterns
    {


        namespace ItemsAndSelection
        {
//            /// <summary>
//            /// A model that can bind to a ItemsControl just like ListBox or List View.  
//            /// Use ItemSelectionGroupProperty to bind it with single set.
//            /// </summary>
//            public  class ItemsAndSelectionGroup :DependencyObject 
//            {




//                public static object  GetItemSelectionGroup(DependencyObject obj)
//                {
//                    return (object)obj.GetValue(ItemsAndSelectionGroupProperty);
//                }

//                public static void SetItemSelectionGroup(DependencyObject obj, object value)
//                {
//                    obj.SetValue(ItemsAndSelectionGroupProperty, value);
//                }

//                public static readonly DependencyProperty ItemsAndSelectionGroupProperty =
//                    DependencyProperty.RegisterAttached("ItemsAndSelectionGroup", typeof(object), typeof(ItemsAndSelectionGroup), new PropertyMetadata(null,
//                        (o, s) =>
//                        {
//                            dynamic vm =o.GetValue (ItemsAndSelectionGroupProperty);

//                            if (vm is  Binding )
//                            {
//                                return;
//                            }

//                            var ls = o as ItemsControl;
//                            if (ls == null)
//                            {
//                                return;
//                            }
               
                            
                         
//                            if (vm == null)
//                            {
//                                return;
//                            }

                            

//                            vm.BindedTo = ls;
//                            var itemsBinding = new Binding()
//                            {
//                                Source = s.NewValue,
//                                Mode = BindingMode.OneWay,
//                                Path = new PropertyPath("Items")
//                            };

//                            BindingOperations.SetBinding(ls, ItemsControl.ItemsSourceProperty, itemsBinding);



//                            if (!(ls is Selector))
//                            {
//                                return;
//                            }



//                            var selectedBinding = new Binding()
//                            {
//                                Source = s.NewValue,
//                                Mode = BindingMode.TwoWay,
//                                Path = new PropertyPath("SelectedItem")
//                            };

//                            BindingOperations.SetBinding(ls, Selector.SelectedItemProperty, selectedBinding);


//                            var selectedindexBinding = new Binding()
//                            {
//                                Source = s.NewValue,
//                                Mode = BindingMode.TwoWay,
//                                Path = new PropertyPath("SelectedIndex")
//                            };

//                            BindingOperations.SetBinding(ls, Selector.SelectedIndexProperty, selectedindexBinding);



//                            var selectedValuePathBinding = new Binding()
//                            {
//                                Source = s.NewValue,
//                                Mode = BindingMode.TwoWay,
//                                Path = new PropertyPath("SelectedValuePath")
//                            };

//                            BindingOperations.SetBinding(ls, Selector.SelectedValuePathProperty, selectedValuePathBinding);

//                            var selectedValueBinding = new Binding()
//                            {
//                                Source = s.NewValue,
//                                Mode = BindingMode.TwoWay,
//                                Path = new PropertyPath("SelectedValue")
//                            };

//                            BindingOperations.SetBinding(ls, Selector.SelectedValueProperty, selectedValueBinding);
//#if SILVERLIGHT_5 || WINDOWS_PHONE_8
//                        if (!(ls is ListBox))
//#else
//                            if (!(ls is ListBox) && (!(ls is ListView)))
//#endif

//                            {
//                                return;
//                            }

//                            var selectionModeBinding = new Binding()
//                            {
//                                Source = s.NewValue,
//                                Mode = BindingMode.TwoWay,
//                                Path = new PropertyPath("SelectionMode")
//                            };

//                            BindingOperations.SetBinding(ls, ListBox.SelectionModeProperty, selectionModeBinding);


//                        }));






//            }


            public class ElementBinder:DependencyObject 
            {




                public static ElementBinder GetElementBinder(DependencyObject obj)
                {
                    return (ElementBinder)obj.GetValue(ElementBinderProperty);
                }

                public static void SetElementBinder(DependencyObject obj, ElementBinder value)
                {
                    obj.SetValue(ElementBinderProperty, value);
                }

                // Using a DependencyProperty as the backing store for ElementBinder.  This enables animation, styling, binding, etc...
                public static readonly DependencyProperty ElementBinderProperty =
                    DependencyProperty.RegisterAttached("ElementBinder", typeof(ElementBinder), typeof(ElementBinder), new PropertyMetadata(null,
                        (o, e) =>
                        {
                            if (e.NewValue  is ElementBinder)
                            {
                                var eb = e.NewValue as ElementBinder;
                                eb.Element = o ;
                            }
                        
                        }


                        ));






                public DependencyObject Element
                {
                    get { return (DependencyObject)GetValue(ElementProperty); }
                    set { SetValue(ElementProperty, value); }
                }

                // Using a DependencyProperty as the backing store for Element.  This enables animation, styling, binding, etc...
                public static readonly DependencyProperty ElementProperty =
                    DependencyProperty.Register("Element", typeof(DependencyObject), typeof(ElementBinder ), new PropertyMetadata(0));

                
       
                
          

                    
            }

            /// <summary>
            /// The abstraction of Items AndS electionGroup
            /// </summary>
            /// <typeparam name="TValue"></typeparam>
            /// <typeparam name="TCollection"></typeparam>
            /// <typeparam name="TList"></typeparam>
            public interface IItemsAndSelectionGroup
           
            {
                DependencyObject BindedTo { get; set; }
                string SelectedValuePath { get; set; }
                SelectionMode SelectionMode { get; set; }
                Object SelectedValue { get; set; }
                IEnumerable  Items { get; }
                int SelectedIndex { get; }
                Object  SelectedItem
                {
                    get;
                }
                IList  SelectedItems
                {
                    get;
                }
            }
            public class ItemsAndSelectionGroup<T> : BindableBase<ItemsAndSelectionGroup<T>>, IItemsAndSelectionGroup
            {

                public ItemsAndSelectionGroup()
                {
                    base.AddDisposeAction(() => BindedTo = null);

                }

                public DependencyObject BindedTo { get; set; }

                public SelectionMode SelectionMode
                {
                    get { return _SelectionModeLocator(this).Value; }
                    set { _SelectionModeLocator(this).SetValueAndTryNotify(value); }
                }
                #region Property SelectionMode SelectionMode Setup
                protected Property<SelectionMode> _SelectionMode = new Property<SelectionMode> { LocatorFunc = _SelectionModeLocator };
                static Func<BindableBase, ValueContainer<SelectionMode>> _SelectionModeLocator = RegisterContainerLocator<SelectionMode>("SelectionMode", model => model.Initialize("SelectionMode", ref model._SelectionMode, ref _SelectionModeLocator, _SelectionModeDefaultValueFactory));
                static Func<SelectionMode> _SelectionModeDefaultValueFactory = null;
                #endregion



                public string SelectedValuePath
                {
                    get { return _SelectedValuePathLocator(this).Value; }
                    set { _SelectedValuePathLocator(this).SetValueAndTryNotify(value); }
                }
                #region Property string SelectedValuePath Setup
                protected Property<string> _SelectedValuePath = new Property<string> { LocatorFunc = _SelectedValuePathLocator };
                static Func<BindableBase, ValueContainer<string>> _SelectedValuePathLocator = RegisterContainerLocator<string>("SelectedValuePath", model => model.Initialize("SelectedValuePath", ref model._SelectedValuePath, ref _SelectedValuePathLocator, _SelectedValuePathDefaultValueFactory));
                static Func<string> _SelectedValuePathDefaultValueFactory = null;
                #endregion


                public object SelectedValue
                {
                    get { return _SelectedValueLocator(this).Value; }
                    set { _SelectedValueLocator(this).SetValueAndTryNotify(value); }
                }
                #region Property object SelectedValue Setup
                protected Property<object> _SelectedValue = new Property<object> { LocatorFunc = _SelectedValueLocator };
                static Func<BindableBase, ValueContainer<object>> _SelectedValueLocator = RegisterContainerLocator<object>("SelectedValue", model => model.Initialize("SelectedValue", ref model._SelectedValue, ref _SelectedValueLocator, _SelectedValueDefaultValueFactory));
                static Func<object> _SelectedValueDefaultValueFactory = null;
                #endregion



                public ObservableCollection<T> Items
                {
                    get { return _ItemsLocator(this).Value; }
                    set { _ItemsLocator(this).SetValueAndTryNotify(value); }
                }
                #region Property ObservableCollection<T>  Items Setup
                protected Property<ObservableCollection<T>> _Items = new Property<ObservableCollection<T>> { LocatorFunc = _ItemsLocator };
                static Func<BindableBase, ValueContainer<ObservableCollection<T>>> _ItemsLocator = RegisterContainerLocator<ObservableCollection<T>>("Items", model => model.Initialize("Items", ref model._Items, ref _ItemsLocator, _ItemsDefaultValueFactory));
                static Func<ObservableCollection<T>> _ItemsDefaultValueFactory = () => new ObservableCollection<T>();
                #endregion



                public int SelectedIndex
                {
                    get { return _SelectedIndexLocator(this).Value; }
                    set { _SelectedIndexLocator(this).SetValueAndTryNotify(value); }
                }
                #region Property int SelectedIndex Setup
                protected Property<int> _SelectedIndex = new Property<int> { LocatorFunc = _SelectedIndexLocator };
                static Func<BindableBase, ValueContainer<int>> _SelectedIndexLocator = RegisterContainerLocator<int>("SelectedIndex", model => model.Initialize("SelectedIndex", ref model._SelectedIndex, ref _SelectedIndexLocator, _SelectedIndexDefaultValueFactory));
                static Func<int> _SelectedIndexDefaultValueFactory = null;
                #endregion



                public T SelectedItem
                {
                    get { return _SelectedItemLocator(this).Value; }
                    set { _SelectedItemLocator(this).SetValueAndTryNotify(value);
                    base.RaisePropertyChanged(() => new PropertyChangedEventArgs("SelectedItems"), "SelectedItems");
                    }
                }
                #region Property T SelectedItem Setup
                protected Property<T> _SelectedItem = new Property<T> { LocatorFunc = _SelectedItemLocator };
                static Func<BindableBase, ValueContainer<T>> _SelectedItemLocator = RegisterContainerLocator<T>("SelectedItem", model => model.Initialize("SelectedItem", ref model._SelectedItem, ref _SelectedItemLocator, _SelectedItemDefaultValueFactory));
                static Func<T> _SelectedItemDefaultValueFactory = null;
                #endregion


                public IList SelectedItems
                {
                    get
                    {
                        if (BindedTo != null)
                        {
                            dynamic x = BindedTo;
                            return x.SelectedItems as IList;
                        }
                        else
                        {
                            return null;
                        }
                    }
                }







              

                IEnumerable IItemsAndSelectionGroup.Items
                {
                    get { return Items; }
                }


                object IItemsAndSelectionGroup.SelectedItem
                {
                    get { return SelectedItem; }
                }

                IList IItemsAndSelectionGroup.SelectedItems
                {
                    get { return SelectedItems; }
                }
            }
        }


        namespace Tree
        {
            public interface ITreeItem<out TNodeValue, TState>
            {
                TNodeValue Value { get; }
                TState State { get; set; }
                ITreeItem<Object, TState> Parent { get; }
                ICollection<ITreeItem<object, TState>> Children { get; }
                Type NodeValueType { get; }
            }



            //[DataContract(IsReference=true) ] //if you want
            public abstract  class TreeItemModelBase<TNodeValue, TState, TSubType> :
                BindableBase<TSubType>,
                ITreeItem<TNodeValue, TState>
                where TSubType : TreeItemModelBase<TNodeValue, TState, TSubType>
            {
                public TreeItemModelBase()
                {

                }






                public Type NodeValueType
                {
                    get
                    {
                        return typeof(TNodeValue);
                    }
                }

                public TNodeValue Value
                {
                    get { return _ValueLocator(this).Value; }
                    set { _ValueLocator(this).SetValueAndTryNotify(value); }
                }
                #region Property TNodeValue Value Setup
                protected Property<TNodeValue> _Value = new Property<TNodeValue> { LocatorFunc = _ValueLocator };
                static Func<BindableBase, ValueContainer<TNodeValue>> _ValueLocator = RegisterContainerLocator<TNodeValue>("Value", model => model.Initialize("Value", ref model._Value, ref _ValueLocator, _ValueDefaultValueFactory));
                static Func<TNodeValue> _ValueDefaultValueFactory = null;
                #endregion



                public TState State
                {
                    get { return _StateLocator(this).Value; }
                    set { _StateLocator(this).SetValueAndTryNotify(value); }
                }
                #region Property TState State Setup
                protected Property<TState> _State = new Property<TState> { LocatorFunc = _StateLocator };
                static Func<BindableBase, ValueContainer<TState>> _StateLocator = RegisterContainerLocator<TState>("State", model => model.Initialize("State", ref model._State, ref _StateLocator, _StateDefaultValueFactory));
                static Func<TState> _StateDefaultValueFactory = null;
                #endregion



                public ITreeItem<object, TState> Parent
                {
                    get { return _ParentLocator(this).Value; }
                    set { _ParentLocator(this).SetValueAndTryNotify(value); }
                }
                #region Property INode<object, TState> Parent Setup
                protected Property<ITreeItem<object, TState>> _Parent = new Property<ITreeItem<object, TState>> { LocatorFunc = _ParentLocator };
                static Func<BindableBase, ValueContainer<ITreeItem<object, TState>>> _ParentLocator = RegisterContainerLocator<ITreeItem<object, TState>>("Parent", model => model.Initialize("Parent", ref model._Parent, ref _ParentLocator, _ParentDefaultValueFactory));
                static Func<ITreeItem<object, TState>> _ParentDefaultValueFactory = null;
                #endregion




                public ObservableCollection<ITreeItem<object, TState>> Children
                {
                    get { return _ChildrenLocator(this).Value; }
                    set { _ChildrenLocator(this).SetValueAndTryNotify(value); }
                }
                #region Property ObservableCollection<ITreeItem<object,TState >> Children Setup
                protected Property<ObservableCollection<ITreeItem<object, TState>>> _Children = new Property<ObservableCollection<ITreeItem<object, TState>>> { LocatorFunc = _ChildrenLocator };
                static Func<BindableBase, ValueContainer<ObservableCollection<ITreeItem<object, TState>>>> _ChildrenLocator = RegisterContainerLocator<ObservableCollection<ITreeItem<object, TState>>>("Children", model => model.Initialize("Children", ref model._Children, ref _ChildrenLocator, _ChildrenDefaultValueFactory));
                static Func<ObservableCollection<ITreeItem<object, TState>>> _ChildrenDefaultValueFactory = ()=>new ObservableCollection<ITreeItem<object, TState>>();
                #endregion




                ICollection<ITreeItem<object, TState>> ITreeItem<TNodeValue, TState>.Children
                {
                    get { return Children; }
                }
            }




            public class TreeViewItemModel<TValue> : TreeItemModelBase<TValue, TreeViewItemState, TreeViewItemModel<TValue>>
            {
             
            }


            public class TreeItemModel<TValue, TState> : TreeItemModelBase<TValue, TState, TreeItemModel<TValue, TState>>
            {

            }







            //[DataContract(IsReference=true) ] //if you want
            public class TreeViewItemState : BindableBase<TreeViewItemState>
            {
                public TreeViewItemState()
                {


                }

                public bool IsSelected
                {
                    get { return _IsSelectedLocator(this).Value; }
                    set { _IsSelectedLocator(this).SetValueAndTryNotify(value); }
                }
                #region Property bool IsSelected Setup
                protected Property<bool> _IsSelected = new Property<bool> { LocatorFunc = _IsSelectedLocator };
                static Func<BindableBase, ValueContainer<bool>> _IsSelectedLocator = RegisterContainerLocator<bool>("IsSelected", model => model.Initialize("IsSelected", ref model._IsSelected, ref _IsSelectedLocator, _IsSelectedDefaultValueFactory));
                static Func<bool> _IsSelectedDefaultValueFactory = null;
                #endregion


                public bool IsChecked
                {
                    get { return _IsCheckedLocator(this).Value; }
                    set { _IsCheckedLocator(this).SetValueAndTryNotify(value); }
                }
                #region Property bool IsChecked Setup
                protected Property<bool> _IsChecked = new Property<bool> { LocatorFunc = _IsCheckedLocator };
                static Func<BindableBase, ValueContainer<bool>> _IsCheckedLocator = RegisterContainerLocator<bool>("IsChecked", model => model.Initialize("IsChecked", ref model._IsChecked, ref _IsCheckedLocator, _IsCheckedDefaultValueFactory));
                static Func<bool> _IsCheckedDefaultValueFactory = null;
                #endregion


                public bool CanBeSelected
                {
                    get { return _CanBeSelectedLocator(this).Value; }
                    set { _CanBeSelectedLocator(this).SetValueAndTryNotify(value); }
                }
                #region Property bool CanBeSelected Setup
                protected Property<bool> _CanBeSelected = new Property<bool> { LocatorFunc = _CanBeSelectedLocator };
                static Func<BindableBase, ValueContainer<bool>> _CanBeSelectedLocator = RegisterContainerLocator<bool>("CanBeSelected", model => model.Initialize("CanBeSelected", ref model._CanBeSelected, ref _CanBeSelectedLocator, _CanBeSelectedDefaultValueFactory));
                static Func<bool> _CanBeSelectedDefaultValueFactory = null;
                #endregion




            }

        }







    }


}
