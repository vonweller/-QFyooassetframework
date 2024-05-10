using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:f5380ebc-e9c9-48f5-bfec-ece7cb1fae5f
	public partial class UIGameStart
	{
		public const string Name = "UIGameStart";
		
		
		private UIGameStartData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			
			mData = null;
		}
		
		public UIGameStartData Data
		{
			get
			{
				return mData;
			}
		}
		
		UIGameStartData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UIGameStartData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
