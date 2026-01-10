using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Windows.Forms;

namespace EVEMon.Adapters
{
    /// <summary>
    /// Provides data binding between ViewModels and WinForms controls.
    /// Enables the Strangler Fig migration pattern by allowing gradual
    /// adoption of MVVM in existing WinForms code.
    /// </summary>
    public static class ViewModelAdapter
    {
        /// <summary>
        /// Binds a ViewModel property to a Control property.
        /// </summary>
        /// <typeparam name="TViewModel">The ViewModel type.</typeparam>
        /// <typeparam name="TValue">The property value type.</typeparam>
        /// <param name="viewModel">The ViewModel instance.</param>
        /// <param name="vmProperty">Expression selecting the ViewModel property.</param>
        /// <param name="control">The WinForms control.</param>
        /// <param name="controlProperty">The control property name to bind.</param>
        /// <returns>An IDisposable that removes the binding when disposed.</returns>
        public static IDisposable Bind<TViewModel, TValue>(
            TViewModel viewModel,
            Expression<Func<TViewModel, TValue>> vmProperty,
            Control control,
            string controlProperty) where TViewModel : INotifyPropertyChanged
        {
            if (viewModel == null)
                throw new ArgumentNullException(nameof(viewModel));
            if (vmProperty == null)
                throw new ArgumentNullException(nameof(vmProperty));
            if (control == null)
                throw new ArgumentNullException(nameof(control));
            if (string.IsNullOrEmpty(controlProperty))
                throw new ArgumentNullException(nameof(controlProperty));

            var vmPropertyName = GetPropertyName(vmProperty);
            var vmPropertyInfo = typeof(TViewModel).GetProperty(vmPropertyName);
            var controlPropertyInfo = control.GetType().GetProperty(controlProperty);

            if (vmPropertyInfo == null)
                throw new ArgumentException($"Property '{vmPropertyName}' not found on {typeof(TViewModel).Name}");
            if (controlPropertyInfo == null)
                throw new ArgumentException($"Property '{controlProperty}' not found on {control.GetType().Name}");

            // Set initial value
            UpdateControlProperty(viewModel, vmPropertyInfo, control, controlPropertyInfo);

            // Subscribe to property changes
            PropertyChangedEventHandler handler = (s, e) =>
            {
                if (e.PropertyName == vmPropertyName || string.IsNullOrEmpty(e.PropertyName))
                {
                    if (control.InvokeRequired)
                    {
                        control.BeginInvoke(new Action(() =>
                            UpdateControlProperty(viewModel, vmPropertyInfo, control, controlPropertyInfo)));
                    }
                    else
                    {
                        UpdateControlProperty(viewModel, vmPropertyInfo, control, controlPropertyInfo);
                    }
                }
            };

            viewModel.PropertyChanged += handler;

            return new BindingDisposable(() => viewModel.PropertyChanged -= handler);
        }

        /// <summary>
        /// Binds a ViewModel property to a Label's Text property.
        /// </summary>
        public static IDisposable BindText<TViewModel, TValue>(
            TViewModel viewModel,
            Expression<Func<TViewModel, TValue>> vmProperty,
            Label label) where TViewModel : INotifyPropertyChanged
        {
            return Bind(viewModel, vmProperty, label, nameof(Label.Text));
        }

        /// <summary>
        /// Binds a ViewModel property to a Control's Text property.
        /// </summary>
        public static IDisposable BindText<TViewModel, TValue>(
            TViewModel viewModel,
            Expression<Func<TViewModel, TValue>> vmProperty,
            Control control) where TViewModel : INotifyPropertyChanged
        {
            return Bind(viewModel, vmProperty, control, nameof(Control.Text));
        }

        /// <summary>
        /// Binds a ViewModel boolean property to a Control's Visible property.
        /// </summary>
        public static IDisposable BindVisible<TViewModel>(
            TViewModel viewModel,
            Expression<Func<TViewModel, bool>> vmProperty,
            Control control) where TViewModel : INotifyPropertyChanged
        {
            return Bind(viewModel, vmProperty, control, nameof(Control.Visible));
        }

        /// <summary>
        /// Binds a ViewModel boolean property to a Control's Enabled property.
        /// </summary>
        public static IDisposable BindEnabled<TViewModel>(
            TViewModel viewModel,
            Expression<Func<TViewModel, bool>> vmProperty,
            Control control) where TViewModel : INotifyPropertyChanged
        {
            return Bind(viewModel, vmProperty, control, nameof(Control.Enabled));
        }

        /// <summary>
        /// Binds a ViewModel property to a ProgressBar's Value property.
        /// </summary>
        public static IDisposable BindProgress<TViewModel>(
            TViewModel viewModel,
            Expression<Func<TViewModel, int>> vmProperty,
            ProgressBar progressBar) where TViewModel : INotifyPropertyChanged
        {
            return Bind(viewModel, vmProperty, progressBar, nameof(ProgressBar.Value));
        }

        /// <summary>
        /// Binds a ViewModel property with a custom converter.
        /// </summary>
        /// <typeparam name="TViewModel">The ViewModel type.</typeparam>
        /// <typeparam name="TSource">The source property type.</typeparam>
        /// <typeparam name="TTarget">The target control property type.</typeparam>
        /// <param name="viewModel">The ViewModel instance.</param>
        /// <param name="vmProperty">Expression selecting the ViewModel property.</param>
        /// <param name="control">The WinForms control.</param>
        /// <param name="controlProperty">The control property name.</param>
        /// <param name="converter">Function to convert from source to target type.</param>
        /// <returns>An IDisposable that removes the binding when disposed.</returns>
        public static IDisposable BindWithConverter<TViewModel, TSource, TTarget>(
            TViewModel viewModel,
            Expression<Func<TViewModel, TSource>> vmProperty,
            Control control,
            string controlProperty,
            Func<TSource, TTarget> converter) where TViewModel : INotifyPropertyChanged
        {
            if (viewModel == null)
                throw new ArgumentNullException(nameof(viewModel));
            if (vmProperty == null)
                throw new ArgumentNullException(nameof(vmProperty));
            if (control == null)
                throw new ArgumentNullException(nameof(control));
            if (converter == null)
                throw new ArgumentNullException(nameof(converter));

            var vmPropertyName = GetPropertyName(vmProperty);
            var vmPropertyInfo = typeof(TViewModel).GetProperty(vmPropertyName);
            var controlPropertyInfo = control.GetType().GetProperty(controlProperty);

            if (vmPropertyInfo == null)
                throw new ArgumentException($"Property '{vmPropertyName}' not found on {typeof(TViewModel).Name}");
            if (controlPropertyInfo == null)
                throw new ArgumentException($"Property '{controlProperty}' not found on {control.GetType().Name}");

            // Action to update the control
            void UpdateControl()
            {
                var sourceValue = (TSource)vmPropertyInfo.GetValue(viewModel);
                var targetValue = converter(sourceValue);
                controlPropertyInfo.SetValue(control, targetValue);
            }

            // Set initial value
            UpdateControl();

            // Subscribe to property changes
            PropertyChangedEventHandler handler = (s, e) =>
            {
                if (e.PropertyName == vmPropertyName || string.IsNullOrEmpty(e.PropertyName))
                {
                    if (control.InvokeRequired)
                    {
                        control.BeginInvoke(new Action(UpdateControl));
                    }
                    else
                    {
                        UpdateControl();
                    }
                }
            };

            viewModel.PropertyChanged += handler;

            return new BindingDisposable(() => viewModel.PropertyChanged -= handler);
        }

        /// <summary>
        /// Creates a binding that executes an action when a property changes.
        /// </summary>
        public static IDisposable OnPropertyChanged<TViewModel, TValue>(
            TViewModel viewModel,
            Expression<Func<TViewModel, TValue>> vmProperty,
            Action<TValue> action) where TViewModel : INotifyPropertyChanged
        {
            if (viewModel == null)
                throw new ArgumentNullException(nameof(viewModel));
            if (vmProperty == null)
                throw new ArgumentNullException(nameof(vmProperty));
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            var vmPropertyName = GetPropertyName(vmProperty);
            var vmPropertyInfo = typeof(TViewModel).GetProperty(vmPropertyName);

            if (vmPropertyInfo == null)
                throw new ArgumentException($"Property '{vmPropertyName}' not found on {typeof(TViewModel).Name}");

            // Execute action with initial value
            action((TValue)vmPropertyInfo.GetValue(viewModel));

            // Subscribe to property changes
            PropertyChangedEventHandler handler = (s, e) =>
            {
                if (e.PropertyName == vmPropertyName || string.IsNullOrEmpty(e.PropertyName))
                {
                    action((TValue)vmPropertyInfo.GetValue(viewModel));
                }
            };

            viewModel.PropertyChanged += handler;

            return new BindingDisposable(() => viewModel.PropertyChanged -= handler);
        }

        private static void UpdateControlProperty<TViewModel>(
            TViewModel viewModel,
            PropertyInfo vmPropertyInfo,
            Control control,
            PropertyInfo controlPropertyInfo)
        {
            var value = vmPropertyInfo.GetValue(viewModel);

            // Convert to string if target is Text property and source is not string
            if (controlPropertyInfo.PropertyType == typeof(string) && value != null && !(value is string))
            {
                value = value.ToString();
            }

            controlPropertyInfo.SetValue(control, value);
        }

        private static string GetPropertyName<TViewModel, TValue>(Expression<Func<TViewModel, TValue>> expression)
        {
            if (expression.Body is MemberExpression memberExpression)
            {
                return memberExpression.Member.Name;
            }

            if (expression.Body is UnaryExpression unaryExpression &&
                unaryExpression.Operand is MemberExpression operandMember)
            {
                return operandMember.Member.Name;
            }

            throw new ArgumentException("Expression must be a member access expression", nameof(expression));
        }

        /// <summary>
        /// Disposable that removes a binding when disposed.
        /// </summary>
        private sealed class BindingDisposable : IDisposable
        {
            private Action _disposeAction;
            private bool _disposed;

            public BindingDisposable(Action disposeAction)
            {
                _disposeAction = disposeAction;
            }

            public void Dispose()
            {
                if (_disposed)
                    return;

                _disposed = true;
                _disposeAction?.Invoke();
                _disposeAction = null;
            }
        }
    }

    /// <summary>
    /// Manages multiple bindings and disposes them together.
    /// </summary>
    public sealed class BindingCollection : IDisposable
    {
        private readonly List<IDisposable> _bindings = new List<IDisposable>();
        private bool _disposed;

        /// <summary>
        /// Adds a binding to the collection.
        /// </summary>
        public void Add(IDisposable binding)
        {
            if (binding == null)
                return;

            if (_disposed)
            {
                binding.Dispose();
                return;
            }

            _bindings.Add(binding);
        }

        /// <summary>
        /// Gets the number of bindings in the collection.
        /// </summary>
        public int Count => _bindings.Count;

        /// <summary>
        /// Disposes all bindings.
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;

            foreach (var binding in _bindings)
            {
                try
                {
                    binding?.Dispose();
                }
                catch
                {
                    // Ignore disposal exceptions
                }
            }

            _bindings.Clear();
        }
    }
}
