﻿#region Apache Licence, Version 2.0
/*
 Copyright 2013 Andrey Bulygin.

 Licensed under the Apache License, Version 2.0 (the "License"); 
 you may not use this file except in compliance with the License. 
 You may obtain a copy of the License at 

		http://www.apache.org/licenses/LICENSE-2.0

 Unless required by applicable law or agreed to in writing, software 
 distributed under the License is distributed on an "AS IS" BASIS, 
 WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
 See the License for the specific language governing permissions 
 and limitations under the License.
 */
#endregion

using System;
using System.Threading.Tasks;

namespace NBoosters.RedisBoost.Core.Misk
{
	internal static class Extensions
	{
		public static Task ContinueWithIfNoError<T>(this T task, Action<T> action)
			where T:Task
		{
			return task.ContinueWith(t =>
				{
					if (task.IsFaulted) throw task.Exception.UnwrapAggregation();
					action(task);
				});
			
		}
		public static Task<TResult> ContinueWithIfNoError<T, TResult>(this T task, Func<T, TResult> func)
			where T : Task
		{
			return task.ContinueWith(t =>
			{
				if (task.IsFaulted) throw task.Exception.UnwrapAggregation();
				return func(task);
			});
		}
		public static Exception UnwrapAggregation(this Exception ex)
		{
			while (ex is AggregateException) ex = ex.InnerException;
			return ex;
		}
	}
}