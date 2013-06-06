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

namespace NBoosters.RedisBoost
{
	public struct MSetArgs
	{
		internal readonly string KeyOrField;
		internal readonly bool IsArray;
		internal readonly object Value;

		public MSetArgs(string keyOrField, byte[] value)
		{
			KeyOrField = keyOrField;
			Value = value;
			IsArray = true;
		}
		public MSetArgs(string keyOrField, object value)
		{
			KeyOrField = keyOrField;
			Value = value;
			IsArray = false;
		}
	}
}