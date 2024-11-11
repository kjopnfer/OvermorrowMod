using ReLogic.Content;
using System.Threading;
using System;
using Terraria;

namespace OvermorrowMod.Common.Utilities
{
    public static class SystemUtils
    {
        /// <summary>
        /// Executes the specified action on the main thread of the application.
        /// If called from a non-main thread, the action is queued and waits until execution on the main thread.
        /// If already on the main thread, the action is executed immediately.
        /// </summary>
        /// <param name="action">The action to be executed on the main thread.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="action"/> is null.</exception>
        public static void InvokeOnMainThread(Action action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action), "Action cannot be null.");

            if (AssetRepository.IsMainThread)
            {
                action();  // Execute immediately if already on the main thread
            }
            else
            {
                var evt = new ManualResetEvent(false);
                Main.QueueMainThreadAction(() =>
                {
                    action();
                    evt.Set();  // Signal that the action is complete
                });
                evt.WaitOne();  // Wait for the action to complete on the main thread
            }
        }
    }
}