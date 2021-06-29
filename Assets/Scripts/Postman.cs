using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Frictionless;

namespace Messages
{
	namespace Gameplay
	{
		public class OnGameWon {}
		public class OnGameLost {}
	}

	namespace UI
	{
		public class OnPlayButtonClick 	{}
		public class OnStepButtonClick 
		{
			public int Speed;
			public OnStepButtonClick(int speed)
			{
				Speed = speed;
			}
		}
	}

	public class Postman : MonoBehaviour
	{
		void Awake()
		{
		}

		public static Postman Get()
		{
			return FindObjectOfType<Postman>();
		}
	}
}
