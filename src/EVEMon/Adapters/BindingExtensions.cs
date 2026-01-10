using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq.Expressions;
using System.Windows.Forms;

namespace EVEMon.Adapters
{
    /// <summary>
    /// Extension methods for easier ViewModel binding in WinForms controls.
    /// </summary>
    public static class BindingExtensions
    {
        /// <summary>
        /// Binds a Label's Text property to a ViewModel property.
        /// </summary>
        public static IDisposable BindTo<TViewModel, TValue>(
            this Label label,
            TViewModel viewModel,
            Expression<Func<TViewModel, TValue>> vmProperty) where TViewModel : INotifyPropertyChanged
        {
            return ViewModelAdapter.BindText(viewModel, vmProperty, label);
        }

        /// <summary>
        /// Binds a Label's Text property to a ViewModel property with formatting.
        /// </summary>
        public static IDisposable BindTo<TViewModel, TValue>(
            this Label label,
            TViewModel viewModel,
            Expression<Func<TViewModel, TValue>> vmProperty,
            string format) where TViewModel : INotifyPropertyChanged
        {
            return ViewModelAdapter.BindWithConverter(
                viewModel,
                vmProperty,
                label,
                nameof(Label.Text),
                value => string.Format(format, value));
        }

        /// <summary>
        /// Binds a ProgressBar's Value property to a ViewModel property.
        /// </summary>
        public static IDisposable BindTo(
            this ProgressBar progressBar,
            INotifyPropertyChanged viewModel,
            Expression<Func<INotifyPropertyChanged, int>> vmProperty)
        {
            return ViewModelAdapter.BindProgress(viewModel, vmProperty, progressBar);
        }

        /// <summary>
        /// Binds a Control's Visible property to a ViewModel boolean property.
        /// </summary>
        public static IDisposable BindVisibleTo<TViewModel>(
            this Control control,
            TViewModel viewModel,
            Expression<Func<TViewModel, bool>> vmProperty) where TViewModel : INotifyPropertyChanged
        {
            return ViewModelAdapter.BindVisible(viewModel, vmProperty, control);
        }

        /// <summary>
        /// Binds a Control's Visible property to a ViewModel boolean property (inverted).
        /// </summary>
        public static IDisposable BindVisibleToInverse<TViewModel>(
            this Control control,
            TViewModel viewModel,
            Expression<Func<TViewModel, bool>> vmProperty) where TViewModel : INotifyPropertyChanged
        {
            return ViewModelAdapter.BindWithConverter(
                viewModel,
                vmProperty,
                control,
                nameof(Control.Visible),
                value => !value);
        }

        /// <summary>
        /// Binds a Control's Enabled property to a ViewModel boolean property.
        /// </summary>
        public static IDisposable BindEnabledTo<TViewModel>(
            this Control control,
            TViewModel viewModel,
            Expression<Func<TViewModel, bool>> vmProperty) where TViewModel : INotifyPropertyChanged
        {
            return ViewModelAdapter.BindEnabled(viewModel, vmProperty, control);
        }

        /// <summary>
        /// Binds a Control's ForeColor to a ViewModel property with a converter.
        /// </summary>
        public static IDisposable BindForeColorTo<TViewModel, TValue>(
            this Control control,
            TViewModel viewModel,
            Expression<Func<TViewModel, TValue>> vmProperty,
            Func<TValue, Color> converter) where TViewModel : INotifyPropertyChanged
        {
            return ViewModelAdapter.BindWithConverter(
                viewModel,
                vmProperty,
                control,
                nameof(Control.ForeColor),
                converter);
        }

        /// <summary>
        /// Binds a Control's BackColor to a ViewModel property with a converter.
        /// </summary>
        public static IDisposable BindBackColorTo<TViewModel, TValue>(
            this Control control,
            TViewModel viewModel,
            Expression<Func<TViewModel, TValue>> vmProperty,
            Func<TValue, Color> converter) where TViewModel : INotifyPropertyChanged
        {
            return ViewModelAdapter.BindWithConverter(
                viewModel,
                vmProperty,
                control,
                nameof(Control.BackColor),
                converter);
        }

        /// <summary>
        /// Binds a ToolStripStatusLabel's Text property to a ViewModel property.
        /// </summary>
        public static IDisposable BindTo<TViewModel, TValue>(
            this ToolStripStatusLabel label,
            TViewModel viewModel,
            Expression<Func<TViewModel, TValue>> vmProperty) where TViewModel : INotifyPropertyChanged
        {
            return BindToolStripItem(label, viewModel, vmProperty, nameof(ToolStripStatusLabel.Text));
        }

        /// <summary>
        /// Binds a ToolStripItem property to a ViewModel property.
        /// </summary>
        private static IDisposable BindToolStripItem<TViewModel, TValue>(
            ToolStripItem item,
            TViewModel viewModel,
            Expression<Func<TViewModel, TValue>> vmProperty,
            string itemProperty) where TViewModel : INotifyPropertyChanged
        {
            if (viewModel == null)
                throw new ArgumentNullException(nameof(viewModel));
            if (vmProperty == null)
                throw new ArgumentNullException(nameof(vmProperty));
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            var vmPropertyName = GetPropertyName(vmProperty);
            var vmPropertyInfo = typeof(TViewModel).GetProperty(vmPropertyName);
            var itemPropertyInfo = item.GetType().GetProperty(itemProperty);

            if (vmPropertyInfo == null)
                throw new ArgumentException($"Property '{vmPropertyName}' not found on {typeof(TViewModel).Name}");
            if (itemPropertyInfo == null)
                throw new ArgumentException($"Property '{itemProperty}' not found on {item.GetType().Name}");

            // Action to update the item
            void UpdateItem()
            {
                var value = vmPropertyInfo.GetValue(viewModel);
                if (itemPropertyInfo.PropertyType == typeof(string) && value != null && !(value is string))
                {
                    value = value.ToString();
                }
                itemPropertyInfo.SetValue(item, value);
            }

            // Set initial value
            UpdateItem();

            // Subscribe to property changes
            PropertyChangedEventHandler handler = (s, e) =>
            {
                if (e.PropertyName == vmPropertyName || string.IsNullOrEmpty(e.PropertyName))
                {
                    var owner = item.GetCurrentParent();
                    if (owner != null && owner.InvokeRequired)
                    {
                        owner.BeginInvoke(new Action(UpdateItem));
                    }
                    else
                    {
                        UpdateItem();
                    }
                }
            };

            viewModel.PropertyChanged += handler;

            return new ToolStripBindingDisposable(viewModel, handler);
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

        private sealed class ToolStripBindingDisposable : IDisposable
        {
            private INotifyPropertyChanged _viewModel;
            private PropertyChangedEventHandler _handler;
            private bool _disposed;

            public ToolStripBindingDisposable(INotifyPropertyChanged viewModel, PropertyChangedEventHandler handler)
            {
                _viewModel = viewModel;
                _handler = handler;
            }

            public void Dispose()
            {
                if (_disposed)
                    return;

                _disposed = true;
                if (_viewModel != null && _handler != null)
                {
                    _viewModel.PropertyChanged -= _handler;
                }
                _viewModel = null;
                _handler = null;
            }
        }
    }
}
