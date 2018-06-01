/* 
 * MIT License
 * 
 * Copyright(c) 2018 thrzn41
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Thrzn41.Util;

namespace Thrzn41.WebexTeams.Version1
{

    /// <summary>
    /// Webhook notification manager.
    /// </summary>
    public class WebhookNotificationManager : IDisposable
    {

        /// <summary>
        /// Notification info.
        /// </summary>
        private class NotificationInfo
        {

            /// <summary>
            /// Event validator.
            /// </summary>
            public EventValidator Validator { get; private set; }

            /// <summary>
            /// Event function.
            /// </summary>
            public Action<WebhookEventData> Func { get; private set; }

            /// <summary>
            /// Event async function.
            /// </summary>
            public Func<WebhookEventData, Task> FuncAsync { get; private set; }


            /// <summary>
            /// Creates Notification info.
            /// </summary>
            /// <param name="validator">Event validator.</param>
            /// <param name="func">Event function.</param>
            public NotificationInfo(EventValidator validator, Action<WebhookEventData> func)
            {
                this.Validator = validator;
                this.Func      = func;
                this.FuncAsync = null;
            }

            /// <summary>
            /// Creates Notification info.
            /// </summary>
            /// <param name="validator">Event validator.</param>
            /// <param name="funcAsync">Event async function.</param>
            public NotificationInfo(EventValidator validator, Func<WebhookEventData, Task> funcAsync)
            {
                this.Validator = validator;
                this.Func      = null;
                this.FuncAsync = funcAsync;
            }

        }




        /// <summary>
        /// Notification info database.
        /// </summary>
        private Dictionary<string, NotificationInfo> notificationInfoDatabase;

        /// <summary>
        /// Lock for notificationInfoDatabase;
        /// </summary>
        private SlimLock lockForDatabase;




        /// <summary>
        /// Creates Webhook notification manager.
        /// </summary>
        public WebhookNotificationManager()
        {
            this.notificationInfoDatabase = new Dictionary<string, NotificationInfo>();

            this.lockForDatabase = new SlimLock();
        }


        /// <summary>
        /// Adds notification to the manager.
        /// </summary>
        /// <param name="webhook">Webhook to be added.</param>
        /// <param name="func">Function to be notified on receiving event.</param>
        public void AddNotification(Webhook webhook, Action<WebhookEventData> func)
        {
            this.lockForDatabase.ExecuteInWriterLock(
                () =>
                {
                    this.notificationInfoDatabase.Add(
                            webhook.Id,
                            new NotificationInfo(webhook.CreateEventValidator(), func)
                        );
                });
        }

        /// <summary>
        /// Adds notification to the manager.
        /// </summary>
        /// <param name="webhook">Webhook to be added.</param>
        /// <param name="funcAsync">Async function to be notified on receiving event.</param>
        public void AddNotification(Webhook webhook, Func<WebhookEventData, Task> funcAsync)
        {
            this.lockForDatabase.ExecuteInWriterLock(
                () =>
                {
                    this.notificationInfoDatabase.Add(
                            webhook.Id,
                            new NotificationInfo(webhook.CreateEventValidator(), funcAsync)
                        );
                });
        }


        /// <summary>
        /// Removes notification info.
        /// </summary>
        /// <param name="webhook">Webhook to be removed.</param>
        public void RemoveNotification(Webhook webhook)
        {
            this.lockForDatabase.ExecuteInWriterLock(
                () =>
                {
                    NotificationInfo info = null;

                    if( this.notificationInfoDatabase.TryGetValue(webhook.Id, out info) )
                    {
                        using (info.Validator)
                        {
                            this.notificationInfoDatabase.Remove(webhook.Id);
                        }
                    }
                });
        }


        /// <summary>
        /// Validates and notifies event.
        /// </summary>
        /// <param name="data">The data to be validated and notified.</param>
        /// <param name="xTeamsSignature">Signature that is notified on event.</param>
        /// <param name="encoding">Encoding of data.</param>
        /// <returns>true if the data is valid, false if the data is invalid.</returns>
        public bool ValidateAndNotify(byte[] data, string xTeamsSignature, Encoding encoding)
        {
            bool result = false;

            if (data != null && data.Length > 0 && !String.IsNullOrEmpty(xTeamsSignature) && encoding != null)
            {
                var webhookEventData = TeamsData.FromJsonString<WebhookEventData>(encoding.GetString(data));

                if( !String.IsNullOrEmpty(webhookEventData.Id) )
                {
                    var notificationInfo = this.lockForDatabase.ExecuteInReaderLock<NotificationInfo>(
                        () =>
                        {
                            NotificationInfo info;

                            if( !this.notificationInfoDatabase.TryGetValue(webhookEventData.Id, out info) )
                            {
                                info = null;
                            }

                            return info;
                        });

                    if(notificationInfo != null)
                    {
                        if( notificationInfo.Validator.Validate(data, xTeamsSignature) )
                        {
                            if(notificationInfo.Func != null)
                            {
                                notificationInfo.Func(webhookEventData);
                            }
                            else if(notificationInfo.FuncAsync != null)
                            {
                                notificationInfo.FuncAsync(webhookEventData);
                            }

                            result = true;
                        }
                    }
                }
            }

            return result;
        }




        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        /// <summary>
        /// Dispose.
        /// </summary>
        /// <param name="disposing">disposing.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    using (this.lockForDatabase)
                    {
                        this.lockForDatabase.ExecuteInWriterLock(
                            () =>
                            {
                                foreach (var item in this.notificationInfoDatabase.Keys)
                                {
                                    using (this.notificationInfoDatabase[item].Validator)
                                    {
                                        // Disposed.
                                    }
                                }

                                this.notificationInfoDatabase.Clear();
                            });

                        // Disposed.
                    }
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~WebhookNotificationManager() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        /// <summary>
        /// Dispose.
        /// </summary>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion



    }

}
