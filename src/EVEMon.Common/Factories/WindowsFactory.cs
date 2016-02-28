using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using EVEMon.Common.Helpers;

namespace EVEMon.Common.Factories
{
    /// <summary>
    /// This factory allows us keep unique instances of 
    /// </summary>
    public static class WindowsFactory
    {
        private static readonly Object s_syncLock = new object();
        private static readonly List<Form> s_taggedWindows = new List<Form>();
        private static readonly List<Form> s_uniqueWindows = new List<Form>();

        /// <summary>
        /// Gets the window displayed as unique if it exists, null otherwise.
        /// </summary>
        /// <returns></returns>
        public static TForm GetUnique<TForm>()
            where TForm : Form
        {
            lock (s_syncLock)
            {
                return s_uniqueWindows.OfType<TForm>()
                    .FirstOrDefault(uniqueWindow => uniqueWindow != null && !uniqueWindow.IsDisposed);
            }
        }

        /// <summary>
        /// Show the unique window. 
        /// When none exist, it is created using the default constructor.
        /// When it already exists, it is bringed to front, or show when hidden.
        /// </summary>
        /// <returns></returns>
        public static TForm ShowUnique<TForm>()
            where TForm : Form => ShowUnique(Create<TForm>);

        /// <summary>
        /// Show the unique window.
        /// When none exist, it is created using the provided callback.
        /// When it already exists, it is bringed to front, or show when hidden.
        /// </summary>
        /// <typeparam name="TForm"></typeparam>
        /// <param name="creation"></param>
        /// <returns></returns>
        private static TForm ShowUnique<TForm>(Func<TForm> creation)
            where TForm : Form
        {
            if (creation == null)
                throw new ArgumentNullException("creation");

            lock (s_syncLock)
            {
                try
                {
                    // Does it already exist ?
                    foreach (TForm existingUniqueWindow in s_uniqueWindows.OfType<TForm>()
                        .Where(existingUniqueWindow => existingUniqueWindow != null && !existingUniqueWindow.IsDisposed))
                    {
                        // Bring to front or show
                        if (existingUniqueWindow.Visible)
                            existingUniqueWindow.BringToFront();
                        else
                            existingUniqueWindow.Show();

                        // Give focus and return
                        existingUniqueWindow.Activate();
                        return existingUniqueWindow;
                    }
                }
                    // Catch exception when the window is being disposed
                catch (ObjectDisposedException ex)
                {
                    ExceptionHandler.LogException(ex, true);
                }

                // Create the window and subscribe to its closing for cleanup
                TForm uniqueWindow = creation();
                uniqueWindow.FormClosing += (sender, args) =>
                {
                    lock (s_syncLock)
                    {
                        s_uniqueWindows.Remove((TForm)sender);
                    }
                };
                s_uniqueWindows.Add(uniqueWindow);

                // Show and return
                uniqueWindow.Show();
                return uniqueWindow;
            }
        }

        /// <summary>
        /// Gets the existing form associated with the given tag.
        /// </summary>
        /// <typeparam name="TForm"></typeparam>
        /// <typeparam name="TTag"></typeparam>
        /// <param name="tag"></param>
        /// <returns></returns>
        public static TForm GetByTag<TForm, TTag>(TTag tag)
            where TForm : Form
            where TTag : class
        {
            Object otag = tag;

            lock (s_syncLock)
            {
                return s_taggedWindows.OfType<TForm>()
                    .FirstOrDefault(existingWindow => existingWindow.Tag == otag && !existingWindow.IsDisposed);
            }
        }

        /// <summary>
        /// Show the window with the given tag.
        /// When none exist, it is created using the public constructor accepting an argument of type <see cref="TTag"/>,
        /// or the default constructor if the previous one does not exist.
        /// When it already exists, it is brought to front, or shown when hidden.
        /// </summary>
        /// <typeparam name="TForm"></typeparam>
        /// <typeparam name="TTag"></typeparam>
        /// <param name="tag"></param>
        /// <returns></returns>
        public static TForm ShowByTag<TForm, TTag>(TTag tag)
            where TForm : Form
            where TTag : class
            => ShowByTag(tag, Create<TForm, TTag>);

        /// <summary>
        /// Show the window with the given tag.
        /// When none exist, it is created using the provided callback, and the provided tag is then associated with it.
        /// When it already exists, it is brought to front, or shown when hidden.
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="creation"></param>
        /// <returns></returns>
        private static TForm ShowByTag<TForm, TTag>(TTag tag, Func<TTag, TForm> creation)
            where TForm : Form
            where TTag : class
        {
            if (creation == null)
                throw new ArgumentNullException("creation");

            Object otag = tag;

            lock (s_syncLock)
            {
                // Does it already exist ?
                foreach (TForm existingWindow in s_taggedWindows.OfType<TForm>()
                    .Where(existingWindow => existingWindow.Tag == otag && !existingWindow.IsDisposed))
                {
                    try
                    {
                        // Bring to front or show
                        if (existingWindow.Visible)
                            existingWindow.BringToFront();
                        else
                            existingWindow.Show();

                        // Give focus and return
                        existingWindow.Activate();
                        return existingWindow;
                    }
                        // Catch exception when the window was disposed
                    catch (ObjectDisposedException ex)
                    {
                        ExceptionHandler.LogException(ex, true);
                    }
                }

                // Create the window and attach the tag
                TForm window = creation.Invoke(tag);
                window.Tag = otag;

                // Store it and subscribe to closing for clean up
                s_taggedWindows.Add(window);
                window.FormClosing += (sender, args) =>
                {
                    lock (s_syncLock)
                    {
                        s_taggedWindows.Remove((TForm)sender);
                    }
                };

                // Show and return
                window.Show();
                return window;
            }
        }

        /// <summary>
        /// Call the default constructor.
        /// </summary>
        /// <typeparam name="TForm"></typeparam>
        /// <returns></returns>
        private static TForm Create<TForm>()
            where TForm : Form
        {
            ConstructorInfo constructorInfo = typeof(TForm).GetConstructor(Type.EmptyTypes);
            return (TForm)constructorInfo?.Invoke(null);
        }

        /// <summary>
        /// Call the public constructor with the provided argument type.
        /// </summary>
        /// <typeparam name="TForm"></typeparam>
        /// <typeparam name="TArg"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        private static TForm Create<TForm, TArg>(TArg data)
            where TForm : Form
        {
            // Search a public instance constructor with a single argument of type TArg
            ConstructorInfo ctor = typeof(TForm).GetConstructor(new[] { typeof(TArg) });
            if (ctor != null)
                return (TForm)ctor.Invoke(new Object[] { data });

            // Failed, use the default constructor
            return Create<TForm>();
        }

        /// <summary>
        /// Close the window with the given tag.
        /// </summary>
        /// <typeparam name="TForm"></typeparam>
        /// <typeparam name="TTag"></typeparam>
        /// <param name="form"></param>
        /// <param name="tag"></param>
        private static void CloseByTag<TForm, TTag>(TForm form, TTag tag)
            where TForm : Form
            where TTag : class
        {
            Object otag = tag;

            lock (s_syncLock)
            {
                // While we find windows to close...
                while (true)
                {
                    // Search all the disposed windows or windows with the same tag
                    bool isDisposed = false;
                    TForm formToRemove = null;
                    foreach (TForm existingWindow in s_taggedWindows
                        .Where(taggedWindow => taggedWindow == form).Cast<TForm>())
                    {
                        try
                        {
                            if (existingWindow.Tag != otag)
                                continue;

                            formToRemove = existingWindow;
                            break;
                        }
                            // Catch exception when the window was disposed - we will remove it also by the way
                        catch (ObjectDisposedException ex)
                        {
                            ExceptionHandler.LogException(ex, true);
                            formToRemove = existingWindow;
                            isDisposed = true;
                        }
                    }

                    // Returns if nothing found on this cycle
                    if (formToRemove == null)
                        return;

                    if (isDisposed)
                        s_taggedWindows.Remove(formToRemove);
                    else
                        formToRemove.Close();
                }
            }
        }

        /// <summary>
        /// Gets and closes the window with the given tag.
        /// </summary>
        /// <typeparam name="TForm">The type of the form.</typeparam>
        /// <typeparam name="TTag">The type of the tag.</typeparam>
        /// <param name="tag">The tag.</param>
        public static void GetAndCloseByTag<TForm, TTag>(TTag tag)
            where TForm : Form
            where TTag : class
        {
            TForm window = GetByTag<TForm, TTag>(tag);

            if (window != null)
                CloseByTag(window, tag);
        }

        /// <summary>
        /// Closes all tagged windows.
        /// </summary>
        public static void CloseAllTagged()
        {
            lock (s_syncLock)
            {
                List<Form> formsToClose = s_taggedWindows.ToList();
                foreach (Form existingWindow in formsToClose.Where(form => !form.IsDisposed))
                {
                    existingWindow.Close();
                }
                s_taggedWindows.Clear();
            }
        }
    }
}