﻿using Polly;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ZqUtils.Extensions;
/****************************
* [Author] 张强
* [Date] 2018-09-11
* [Describe] Polly异常处理工具类
* **************************/
namespace ZqUtils.Helpers
{
    /// <summary>
    /// Polly异常处理工具类
    /// </summary>
    public class PollyHelper
    {
        #region WaitAndRetry    
        #region Sync
        /// <summary>
        /// Polly重试指定次数
        /// </summary>
        /// <param name="action"></param>
        /// <param name="actionException"></param>
        /// <param name="sleepDurations"></param>
        /// <param name="onRetry"></param>
        public static void WaitAndRetry<T>(Action action, Action<Exception> actionException, IEnumerable<TimeSpan> sleepDurations, Action<Exception, TimeSpan, int, Context> onRetry) where T : Exception
        {
            try
            {
                //异常重试事件
                if (onRetry == null)
                {
                    void OnRetry(Exception exception, TimeSpan timeSpan, int count, Context context) => LogHelper.Error(exception, $"异常：{exception.Message}，时间：{timeSpan}，重试次数：{count}，内容：{context.ToJson()}");
                    onRetry = OnRetry;
                }
                Policy
                    .Handle<T>()
                    .WaitAndRetry(sleepDurations, onRetry)
                    .Execute(action);
            }
            catch (Exception ex)
            {
                if (actionException != null)
                {
                    actionException(ex);
                }
                else
                {
                    LogHelper.Error(ex, "WaitAndRetry");
                }
            }
        }

        /// <summary>
        /// Polly重试指定次数
        /// </summary>
        /// <param name="action"></param>
        /// <param name="retryCount"></param>
        /// <param name="actionException"></param>
        /// <param name="sleepDurationProvider"></param>
        /// <param name="onRetry"></param>
        public static void WaitAndRetry<T>(Action action, int retryCount, Action<Exception> actionException, Func<int, TimeSpan> sleepDurationProvider, Action<Exception, TimeSpan, int, Context> onRetry) where T : Exception
        {
            try
            {
                //延迟机制，默认为3的重试次数次方
                if (sleepDurationProvider == null)
                {
                    TimeSpan SleepDurationProvider(int retryAttempt) => TimeSpan.FromSeconds(Math.Pow(3, retryAttempt));
                    sleepDurationProvider = SleepDurationProvider;
                }
                //异常重试事件
                if (onRetry == null)
                {
                    void OnRetry(Exception exception, TimeSpan timeSpan, int count, Context context) => LogHelper.Error(exception, $"异常：{exception.Message}，时间：{timeSpan}，重试次数：{count}，内容：{context.ToJson()}");
                    onRetry = OnRetry;
                }
                Policy
                    .Handle<T>()
                    .WaitAndRetry(retryCount, sleepDurationProvider, onRetry)
                    .Execute(action);
            }
            catch (Exception ex)
            {
                if (actionException != null)
                {
                    actionException(ex);
                }
                else
                {
                    LogHelper.Error(ex, "WaitAndRetry");
                }
            }
        }
        #endregion

        #region Async
        /// <summary>
        /// Polly重试指定次数
        /// </summary>
        /// <param name="action"></param>
        /// <param name="actionException"></param>
        /// <param name="sleepDurations"></param>
        /// <param name="onRetry"></param>
        public static async void WaitAndRetryAsync<T>(Func<Task> action, Action<Exception> actionException, IEnumerable<TimeSpan> sleepDurations, Action<Exception, TimeSpan, int, Context> onRetry) where T : Exception
        {
            try
            {
                //异常重试事件
                if (onRetry == null)
                {
                    void OnRetry(Exception exception, TimeSpan timeSpan, int count, Context context) => LogHelper.Error(exception, $"异常：{exception.Message}，时间：{timeSpan}，重试次数：{count}，内容：{context.ToJson()}");
                    onRetry = OnRetry;
                }
                await Policy
                        .Handle<T>()
                        .WaitAndRetryAsync(sleepDurations, onRetry)
                        .ExecuteAsync(action);
            }
            catch (Exception ex)
            {
                if (actionException != null)
                {
                    actionException(ex);
                }
                else
                {
                    LogHelper.Error(ex, "WaitAndRetry");
                }
            }
        }

        /// <summary>
        /// Polly重试指定次数
        /// </summary>
        /// <param name="action"></param>
        /// <param name="retryCount"></param>
        /// <param name="actionException"></param>
        /// <param name="sleepDurationProvider"></param>
        /// <param name="onRetry"></param>
        public static async void WaitAndRetryAsync<T>(Func<Task> action, int retryCount, Action<Exception> actionException, Func<int, TimeSpan> sleepDurationProvider, Action<Exception, TimeSpan, int, Context> onRetry) where T : Exception
        {
            try
            {
                //延迟机制
                if (sleepDurationProvider == null)
                {
                    TimeSpan SleepDurationProvider(int retryAttempt) => TimeSpan.FromSeconds(Math.Pow(3, retryAttempt));
                    sleepDurationProvider = SleepDurationProvider;
                }
                //异常重试事件
                if (onRetry == null)
                {
                    void OnRetry(Exception exception, TimeSpan timeSpan, int count, Context context) => LogHelper.Error(exception, $"异常：{exception.Message}，时间：{timeSpan}，重试次数：{count}，内容：{context.ToJson()}");
                    onRetry = OnRetry;
                }
                await Policy
                        .Handle<T>()
                        .WaitAndRetryAsync(retryCount, sleepDurationProvider, onRetry)
                        .ExecuteAsync(action);
            }
            catch (Exception ex)
            {
                if (actionException != null)
                {
                    actionException(ex);
                }
                else
                {
                    LogHelper.Error(ex, "WaitAndRetry");
                }
            }
        }
        #endregion
        #endregion

        #region WaitAndRetryForever
        #region Sync
        /// <summary>
        /// Polly永久重试机制
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        /// <param name="sleepDurationProvider"></param>
        /// <param name="onRetry"></param>
        public static void WaitAndRetryForever<T>(Action action, Func<int, TimeSpan> sleepDurationProvider, Action<Exception, TimeSpan> onRetry) where T : Exception
        {
            //延迟机制
            if (sleepDurationProvider == null)
            {
                TimeSpan SleepDurationProvider(int retryAttempt) => TimeSpan.FromSeconds(Math.Pow(3, retryAttempt));
                sleepDurationProvider = SleepDurationProvider;
            }
            //异常重试事件
            if (onRetry == null)
            {
                void OnRetry(Exception exception, TimeSpan timeSpan) => LogHelper.Error(exception, $"异常：{exception.Message}，时间：{timeSpan}");
                onRetry = OnRetry;
            }
            Policy
               .Handle<T>()
               .WaitAndRetryForever(sleepDurationProvider, onRetry)
               .Execute(action);
        }

        /// <summary>
        /// Polly永久重试机制
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        /// <param name="sleepDurationProvider"></param>
        /// <param name="onRetry"></param>
        public static void WaitAndRetryForever<T>(Action action, Func<int, Exception, Context, TimeSpan> sleepDurationProvider, Action<Exception, TimeSpan, Context> onRetry) where T : Exception
        {
            //延迟机制
            if (sleepDurationProvider == null)
            {
                TimeSpan SleepDurationProvider(int retryAttempt, Exception exception, Context context) => TimeSpan.FromSeconds(Math.Pow(3, retryAttempt));
                sleepDurationProvider = SleepDurationProvider;
            }
            //异常重试事件
            if (onRetry == null)
            {
                void OnRetry(Exception exception, TimeSpan timeSpan, Context context) => LogHelper.Error(exception, $"异常：{exception.Message}，时间：{timeSpan}，内容：{context.ToJson()}");
                onRetry = OnRetry;
            }
            Policy
               .Handle<T>()
               .WaitAndRetryForever(sleepDurationProvider, onRetry)
               .Execute(action);
        }
        #endregion

        #region Async
        /// <summary>
        /// Polly永久重试机制
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        /// <param name="sleepDurationProvider"></param>
        /// <param name="onRetry"></param>
        public static async void WaitAndRetryForeverAsync<T>(Func<Task> action, Func<int, TimeSpan> sleepDurationProvider, Action<Exception, TimeSpan> onRetry) where T : Exception
        {
            //延迟机制
            if (sleepDurationProvider == null)
            {
                TimeSpan SleepDurationProvider(int retryAttempt) => TimeSpan.FromSeconds(Math.Pow(3, retryAttempt));
                sleepDurationProvider = SleepDurationProvider;
            }
            //异常重试事件
            if (onRetry == null)
            {
                void OnRetry(Exception exception, TimeSpan timeSpan) => LogHelper.Error(exception, $"异常：{exception.Message}，时间：{timeSpan}");
                onRetry = OnRetry;
            }
            await Policy
                    .Handle<T>()
                    .WaitAndRetryForeverAsync(sleepDurationProvider, onRetry)
                    .ExecuteAsync(action);
        }

        /// <summary>
        /// Polly永久重试机制
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        /// <param name="sleepDurationProvider"></param>
        /// <param name="onRetryAsync"></param>
        public static async void WaitAndRetryForeverAsync<T>(Func<Task> action, Func<int, Exception, Context, TimeSpan> sleepDurationProvider, Func<Exception, TimeSpan, Context, Task> onRetryAsync) where T : Exception
        {
            //延迟机制
            if (sleepDurationProvider == null)
            {
                TimeSpan SleepDurationProvider(int retryAttempt, Exception exception, Context context) => TimeSpan.FromSeconds(Math.Pow(3, retryAttempt));
                sleepDurationProvider = SleepDurationProvider;
            }
            //异常重试事件
            if (onRetryAsync == null)
            {
                Task OnRetryAsync(Exception exception, TimeSpan timeSpan, Context context)
                {
                    LogHelper.Error(exception, $"异常：{exception.Message}，时间：{timeSpan}，内容：{context.ToJson()}");
                    return Task.FromResult(0);
                }
                onRetryAsync = OnRetryAsync;
            }
            await Policy
                    .Handle<T>()
                    .WaitAndRetryForeverAsync(sleepDurationProvider, onRetryAsync)
                    .ExecuteAsync(action);
        }
        #endregion
        #endregion
    }
}
