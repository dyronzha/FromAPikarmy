using UnityEngine;

namespace FromAPikarmy
{
	[RequireComponent(typeof(Animator))]
	public class PikameeAnimationModule : MonoBehaviour
	{
		[SerializeField] private string _eatName;
		[SerializeField] private string _eatParameter;

		private bool _eatingDonut;
		private int _eatNameHash;
		private int _eatParameterHash;
		private float _eatingTimer;
		private Animator _animator;

		public void PlayEat()
		{
			_eatingDonut = true;
			_eatingTimer = 0f;
			float normalizedTime = _animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
			normalizedTime = normalizedTime - (int)normalizedTime;
			_animator.Play(_eatNameHash, 1, normalizedTime);
			_animator.SetBool(_eatParameterHash, true);
		}

		private void Awake()
		{
			_animator = GetComponent<Animator>();
			_eatNameHash = Animator.StringToHash(_eatName);
			_eatParameterHash = Animator.StringToHash(_eatParameter);
		}

		private void LateUpdate()
		{
			if (_eatingDonut)
			{
				_eatingTimer += Time.deltaTime;
				if (_eatingTimer >= 1.0f)
				{
					EndEat();
				}
			}
		}

		private void EndEat()
		{
			_eatingDonut = false;
			_eatingTimer = 0f;
			_animator.SetBool(_eatParameterHash, false);
		}
	}
}
