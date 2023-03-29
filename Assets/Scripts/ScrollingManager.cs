using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FromAPikarmy
{
	public class ScrollingManager : MonoBehaviour
	{
		[SerializeField] private int _scrollingSpeed;

		public Vector3 ScrollVector => new Vector3(-Time.deltaTime * _scrollingSpeed, 0, 0);

		private static ScrollingManager _instance;

		public static ScrollingManager Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = GameObject.FindObjectOfType<ScrollingManager>();
				}
				return _instance;
			}
		}

		private void Awake()
		{
			if (_instance == null)
			{
				_instance = this;
			}
		}

		private void OnDestroy()
		{
			_instance = null;
		}
	}
}


