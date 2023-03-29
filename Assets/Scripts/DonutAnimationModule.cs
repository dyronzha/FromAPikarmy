using UnityEngine;

namespace FromAPikarmy
{
	[RequireComponent(typeof(Animator))]
	public class DonutAnimationModule : MonoBehaviour
	{
		[SerializeField] private string _idleName;
		[SerializeField] private string _runName;
		[SerializeField] private string _eatenName;

		private bool _isRun = false;
		private int _idleNameHashID;
		private int _runNameHashID;
		private int _eatenNameHashID;
		private Animator _animator;


		public void PlayIdle()
		{
			_isRun = false;
			_animator.Play(_idleNameHashID, 0);
		}

		public void PlayRun()
		{
			if (_isRun)
			{
				return;
			}
			_isRun = true;
			_animator.Play(_runNameHashID, 0);
		}

		public void PlayEaten()
		{
			_animator.Play(_eatenNameHashID, 0);
		}

		private void Awake()
		{
			_animator = GetComponent<Animator>();
			_idleNameHashID = Animator.StringToHash(_idleName);
			_runNameHashID = Animator.StringToHash(_runName);
			_eatenNameHashID = Animator.StringToHash(_eatenName);
		}
	}
}
