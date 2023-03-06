using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FromAPikarmy
{
	public partial class Tomoe
	{
		public abstract class TomoeState
		{

			public TomoeState()
			{

			}
			public abstract void Update();
		}

		public class TomoeFlyState : TomoeState
		{
			
			public override void Update()
			{

			}
		}
	}

    
}

